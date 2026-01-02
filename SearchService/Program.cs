using Serilog;
using SearchService.Infrastructure;
using SearchService.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/search-service-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Search Service API",
        Version = "v1",
        Description = "Enterprise-grade unified search API using .NET 8 and Elasticsearch"
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register Elasticsearch client
var elasticClient = ElasticsearchConfiguration.CreateClient(builder.Configuration);
builder.Services.AddSingleton(elasticClient);

// Register services
builder.Services.AddScoped<IElasticsearchService, ElasticsearchService>();
builder.Services.AddScoped<ISearchIntelligenceService, SearchIntelligenceService>();
builder.Services.AddScoped<ISecurityService, SecurityService>();
builder.Services.AddScoped<IIndexingService, IndexingService>();

// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<SearchService.HealthChecks.ElasticsearchHealthCheck>("elasticsearch");

var app = builder.Build();

// Initialize Elasticsearch indices with retry logic
await InitializeElasticsearchWithRetry(elasticClient);

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Search Service API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

app.UseSerilogRequestLogging();

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health");

Log.Information("Search Service starting...");

try
{
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static async Task InitializeElasticsearchWithRetry(Nest.IElasticClient elasticClient, int maxRetries = 10, int delaySeconds = 5)
{
    var attempt = 0;
    
    while (attempt < maxRetries)
    {
        try
        {
            Log.Information("Initializing Elasticsearch indices... (Attempt {Attempt}/{MaxRetries})", attempt + 1, maxRetries);
            
            // First, check if Elasticsearch is responding
            var pingResponse = await elasticClient.PingAsync();
            if (!pingResponse.IsValid)
            {
                throw new Exception("Elasticsearch is not responding to ping");
            }
            
            // Now create indices
            await ElasticsearchConfiguration.CreateIndicesAsync(elasticClient);
            Log.Information("Elasticsearch indices initialized successfully");
            return; // Success, exit the retry loop
        }
        catch (Exception ex)
        {
            attempt++;
            Log.Warning(ex, "Failed to initialize Elasticsearch indices (Attempt {Attempt}/{MaxRetries}). " +
                           "Retrying in {DelaySeconds} seconds...", attempt, maxRetries, delaySeconds);
            
            if (attempt >= maxRetries)
            {
                Log.Error(ex, "Failed to initialize Elasticsearch indices after {MaxRetries} attempts. " +
                             "The application will continue but search functionality may not work properly.", maxRetries);
                return;
            }
            
            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }
    }
}

