# Quick Start Guide - Search Service

This guide will help you get the Search Service up and running in minutes.

## Prerequisites

- Docker & Docker Compose installed
- Python 3.8+ (for test data generation)
- 4GB+ RAM available for Docker
- **For local development:** [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and Visual Studio / Rider

## Option A: Run with Docker (Recommended for Quick Start)

### Step 1: Start All Services

```bash
# Clone the repository (if not already done)
git clone https://github.com/maitridave/search-service.git
cd search-service

# Start all services (this may take 1-2 minutes on first run)
docker-compose up -d

# Check service status
docker-compose ps
```

Expected output:
```
NAME                COMMAND                  STATUS
elasticsearch       "/bin/tini -- /usr..."   Up (healthy)
kibana              "/bin/tini -- /usr..."   Up (healthy)
rabbitmq            "docker-entrypoint..."   Up (healthy)
search-service      "dotnet SearchServi..."   Up (healthy)
```

## Step 2: Verify Services are Running

```bash
# Check Search Service Health
curl http://localhost:5000/health

# Check Elasticsearch
curl http://localhost:9200/_cluster/health

# Access Swagger UI in browser
open http://localhost:5000
```

## Step 3: Generate Test Data

```bash
# Install Python dependencies
pip install requests

# Generate sample data (1000 offers, 500 purchases, 300 transports)
python3 scripts/generate_data.py
```

This will take a few minutes and index all documents into Elasticsearch.

## Step 4: Test Search Functionality

### Using curl:

```bash
# Search for Toyota vehicles
curl "http://localhost:5000/api/search/offers?query=Toyota&userRole=agent&userId=AGENT-001"

# Autocomplete suggestions
curl "http://localhost:5000/api/search/autocomplete?query=Toy&entityType=offers&size=5"

# Unified search across all entities
curl "http://localhost:5000/api/search/unified?query=2023&userRole=agent&userId=AGENT-001"
```

### Using Swagger UI:

1. Open http://localhost:5000 in your browser
2. Click on any endpoint to expand it
3. Click "Try it out"
4. Fill in the parameters
5. Click "Execute"

## Step 5: Run Load Tests

```bash
# Run performance tests
python3 scripts/load_test.py
```

This will execute 100 search requests with 10 concurrent connections and show performance metrics.

## Step 6: Explore with Kibana

```bash
# Open Kibana in browser
open http://localhost:5601

# Go to Dev Tools and run:
GET offers/_search
{
  "query": {
    "match_all": {}
  }
}
```

---

## Option B: Run Locally with Visual Studio / Rider

### Step 1: Open Solution

```bash
# Clone the repository
git clone https://github.com/maitridave/search-service.git
cd search-service

# Open the solution file
# - Double-click SearchService.sln, or
# - From Visual Studio: File > Open > Project/Solution > select SearchService.sln
# - From Rider: Open SearchService.sln
```

### Step 2: Start Dependencies

```bash
# Start only Elasticsearch, Kibana, and RabbitMQ (not the search service)
docker-compose up -d elasticsearch rabbitmq kibana

# Wait for services to be ready (30-60 seconds)
docker-compose ps
```

### Step 3: Run from IDE

**Visual Studio:**
1. Set `SearchService` as the startup project
2. Press F5 or click "Start Debugging"
3. The service will start on http://localhost:5000

**Rider:**
1. Select `SearchService` run configuration
2. Click the Run button or press Shift+F10
3. The service will start on http://localhost:5000

**Or via Command Line:**
```bash
dotnet run --project SearchService/SearchService.csproj
```

### Step 4: Generate Test Data & Test

Follow Steps 3-5 from Option A above to generate data and test the service.

---

## Common Search Examples

### 1. Role-based Search

**Seller** (sees only their offers):
```bash
curl "http://localhost:5000/api/search/offers?query=*&userRole=seller&userId=SELLER-1234"
```

**Buyer** (sees all available offers):
```bash
curl "http://localhost:5000/api/search/offers?query=Toyota&userRole=buyer&userId=BUYER-5678"
```

**Carrier** (sees their assigned transports):
```bash
curl "http://localhost:5000/api/search/transports?query=*&userRole=carrier&userId=CARRIER-9012"
```

**Agent** (sees everything):
```bash
curl "http://localhost:5000/api/search/unified?query=Honda&userRole=agent&userId=AGENT-001"
```

### 2. Advanced Search

**Fuzzy search** (finds "Toyoto" when searching for "Toyota"):
```bash
curl "http://localhost:5000/api/search/offers?query=Toyoto&enableFuzzy=true&userRole=agent&userId=AGENT-001"
```

**With filters**:
```bash
curl "http://localhost:5000/api/search/offers?query=*&filters[status]=available&userRole=agent&userId=AGENT-001"
```

**With pagination**:
```bash
curl "http://localhost:5000/api/search/offers?query=*&page=2&pageSize=20&userRole=agent&userId=AGENT-001"
```

## Troubleshooting

### Services not starting?
```bash
# Check logs
docker-compose logs search-service
docker-compose logs elasticsearch

# Restart services
docker-compose restart
```

### No search results?
```bash
# Check if indices exist
curl http://localhost:9200/_cat/indices

# Check document count
curl http://localhost:9200/offers/_count

# Re-generate data
python3 scripts/generate_data.py
```

### Elasticsearch connection issues?
```bash
# Wait 30-60 seconds for Elasticsearch to fully start
# Then check health
curl http://localhost:9200/_cluster/health

# Increase Docker memory if needed (4GB recommended)
```

## Stop Services

```bash
# Stop all services
docker-compose down

# Stop and remove all data
docker-compose down -v
```

## Next Steps

- Explore the [full README](README.md) for detailed documentation
- Check out the [API documentation](http://localhost:5000) via Swagger
- View logs: `docker-compose logs -f search-service`
- Monitor Elasticsearch: http://localhost:5601 (Kibana)
- Monitor RabbitMQ: http://localhost:15672 (guest/guest)

## Need Help?

- Check the logs: `docker-compose logs`
- Verify services: `docker-compose ps`
- Restart: `docker-compose restart`
- Full reset: `docker-compose down -v && docker-compose up -d`
