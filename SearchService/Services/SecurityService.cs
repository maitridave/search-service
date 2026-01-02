using Nest;
using SearchService.Models.RequestModels;

namespace SearchService.Services;

public interface ISecurityService
{
    QueryContainer BuildSecurityFilter(string userRole, string userId, string entityType);
    bool ValidateAccess(string userRole, string userId, string entityType, string resourceId);
}

public class SecurityService : ISecurityService
{
    private readonly ILogger<SecurityService> _logger;

    public SecurityService(ILogger<SecurityService> logger)
    {
        _logger = logger;
    }

    public QueryContainer BuildSecurityFilter(string userRole, string userId, string entityType)
    {
        if (string.IsNullOrWhiteSpace(userRole) || string.IsNullOrWhiteSpace(userId))
        {
            _logger.LogWarning("Missing user role or user ID for security filter");
            return null;
        }

        var role = ParseUserRole(userRole);

        return role switch
        {
            UserRole.Seller => BuildSellerFilter(userId, entityType),
            UserRole.Buyer => BuildBuyerFilter(userId, entityType),
            UserRole.Carrier => BuildCarrierFilter(userId, entityType),
            UserRole.Agent => BuildAgentFilter(userId, entityType),
            _ => null
        };
    }

    public bool ValidateAccess(string userRole, string userId, string entityType, string resourceId)
    {
        if (string.IsNullOrWhiteSpace(userRole) || string.IsNullOrWhiteSpace(userId))
        {
            return false;
        }

        var role = ParseUserRole(userRole);

        // Agents have full access
        if (role == UserRole.Agent)
        {
            return true;
        }

        // Additional validation logic based on role and entity type
        return true;
    }

    private QueryContainer BuildSellerFilter(string userId, string entityType)
    {
        if (entityType == "offers")
        {
            // Sellers can only see their own offers
            return new TermQuery
            {
                Field = "sellerId",
                Value = userId
            };
        }

        // Sellers cannot access other entity types
        return new MatchNoneQuery();
    }

    private QueryContainer BuildBuyerFilter(string userId, string entityType)
    {
        if (entityType == "offers")
        {
            // Buyers can see all available offers
            return new TermQuery
            {
                Field = "status",
                Value = "available"
            };
        }

        if (entityType == "purchases")
        {
            // Buyers can only see their own purchases
            return new TermQuery
            {
                Field = "buyerId",
                Value = userId
            };
        }

        // Buyers cannot access transport data
        return new MatchNoneQuery();
    }

    private QueryContainer BuildCarrierFilter(string userId, string entityType)
    {
        if (entityType == "transports")
        {
            // Carriers can only see their assigned transports
            return new TermQuery
            {
                Field = "carrierId",
                Value = userId
            };
        }

        if (entityType == "offers")
        {
            // Carriers can see offers related to their transports
            // This would typically require a join or enrichment
            return new MatchAllQuery();
        }

        // Carriers cannot access purchase data directly
        return new MatchNoneQuery();
    }

    private QueryContainer BuildAgentFilter(string userId, string entityType)
    {
        // Agents have access to all data
        return new MatchAllQuery();
    }

    private UserRole ParseUserRole(string role)
    {
        return role.ToLower() switch
        {
            "seller" => UserRole.Seller,
            "buyer" => UserRole.Buyer,
            "carrier" => UserRole.Carrier,
            "agent" => UserRole.Agent,
            _ => UserRole.Agent
        };
    }
}
