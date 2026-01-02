# API Examples - Search Service

This document provides comprehensive examples of using the Search Service API.

## Table of Contents

1. [Search Endpoints](#search-endpoints)
2. [Index Management](#index-management)
3. [Health & Monitoring](#health--monitoring)
4. [Role-based Access Examples](#role-based-access-examples)

---

## Search Endpoints

### 1. Unified Search (Cross-entity)

Search across all entity types simultaneously:

```bash
curl -X GET "http://localhost:5000/api/search/unified" \
  -H "Content-Type: application/json" \
  -d '{
    "query": "Toyota",
    "userRole": "agent",
    "userId": "AGENT-001",
    "page": 1,
    "pageSize": 10,
    "enableFuzzy": true,
    "enableHighlighting": true
  }'
```

Response:
```json
{
  "totalHits": 45,
  "maxScore": 2.5,
  "results": [
    {
      "document": { ... },
      "score": 2.5,
      "highlights": {
        "make": ["<em>Toyota</em>"]
      }
    }
  ],
  "facets": {},
  "suggestions": [],
  "elapsedMilliseconds": 120
}
```

### 2. Search Offers

Search only offer documents:

```bash
# Simple search
curl "http://localhost:5000/api/search/offers?query=Camry&userRole=buyer&userId=BUYER-001"

# With filters and pagination
curl "http://localhost:5000/api/search/offers?query=Toyota&userRole=agent&userId=AGENT-001&page=1&pageSize=20"

# Search by VIN
curl "http://localhost:5000/api/search/offers?query=1HGBH41JXMN109186&userRole=agent&userId=AGENT-001"

# Search by year
curl "http://localhost:5000/api/search/offers?query=2023&userRole=buyer&userId=BUYER-001"

# Search by location
curl "http://localhost:5000/api/search/offers?query=New%20York&userRole=agent&userId=AGENT-001"
```

### 3. Search Purchases

Search purchase records:

```bash
# Search by buyer name
curl "http://localhost:5000/api/search/purchases?query=John%20Smith&userRole=agent&userId=AGENT-001"

# Search by purchase ID
curl "http://localhost:5000/api/search/purchases?query=PUR-12345&userRole=buyer&userId=BUYER-001"

# Buyer sees only their purchases
curl "http://localhost:5000/api/search/purchases?query=*&userRole=buyer&userId=BUYER-001"
```

### 4. Search Transports

Search transport records:

```bash
# Search by VIN
curl "http://localhost:5000/api/search/transports?query=1HGBH41JXMN109186&userRole=agent&userId=AGENT-001"

# Search by location
curl "http://localhost:5000/api/search/transports?query=Los%20Angeles&userRole=agent&userId=AGENT-001"

# Carrier sees only their transports
curl "http://localhost:5000/api/search/transports?query=*&userRole=carrier&userId=CARRIER-001"
```

### 5. Autocomplete

Get search suggestions:

```bash
# Autocomplete for offers
curl "http://localhost:5000/api/search/autocomplete?query=Toy&entityType=offers&size=5"

# Response
{
  "suggestions": [
    {
      "text": "Toyota",
      "score": 10.5
    },
    {
      "text": "Toyota Camry",
      "score": 8.2
    }
  ]
}

# Autocomplete for purchases
curl "http://localhost:5000/api/search/autocomplete?query=PUR&entityType=purchases&size=5"
```

### 6. Aggregations (Faceted Search)

Get category counts for filtering:

```bash
curl "http://localhost:5000/api/search/aggregations?userRole=agent&userId=AGENT-001"

# Response
{
  "facets": {
    "by_make": [
      { "key": "Toyota", "count": 125 },
      { "key": "Honda", "count": 98 },
      { "key": "Ford", "count": 87 }
    ],
    "by_status": [
      { "key": "available", "count": 210 },
      { "key": "sold", "count": 78 },
      { "key": "pending", "count": 22 }
    ],
    "by_condition": [
      { "key": "New", "count": 150 },
      { "key": "Used", "count": 100 },
      { "key": "Certified", "count": 60 }
    ]
  }
}
```

---

## Index Management

### 1. Index Single Offer

```bash
curl -X POST "http://localhost:5000/api/index/offers" \
  -H "Content-Type: application/json" \
  -d '{
    "offerId": "OFF-12345",
    "sellerId": "SELLER-001",
    "vin": "1HGBH41JXMN109186",
    "make": "Toyota",
    "model": "Camry",
    "year": 2023,
    "offerAmount": 28500.00,
    "location": {
      "city": "New York",
      "state": "NY",
      "country": "USA"
    },
    "condition": "New",
    "status": "available",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }'

# Response
{
  "success": true,
  "message": "Offer indexed successfully",
  "documentId": "OFF-12345"
}
```

### 2. Index Single Purchase

```bash
curl -X POST "http://localhost:5000/api/index/purchases" \
  -H "Content-Type: application/json" \
  -d '{
    "purchaseId": "PUR-67890",
    "buyerId": "BUYER-001",
    "offerId": "OFF-12345",
    "purchaseDate": "2024-01-15T10:30:00Z",
    "amount": 28000.00,
    "status": "completed",
    "buyerDetails": {
      "name": "John Smith",
      "contact": "+1-555-123-4567"
    },
    "paymentMethod": "Credit Card",
    "createdAt": "2024-01-15T10:30:00Z",
    "updatedAt": "2024-01-15T10:30:00Z"
  }'
```

### 3. Index Single Transport

```bash
curl -X POST "http://localhost:5000/api/index/transports" \
  -H "Content-Type: application/json" \
  -d '{
    "transportId": "TRN-11111",
    "carrierId": "CARRIER-001",
    "purchaseId": "PUR-67890",
    "pickupLocation": "New York, NY",
    "deliveryLocation": "Los Angeles, CA",
    "scheduleDate": "2024-01-20T08:00:00Z",
    "actualPickupDate": null,
    "expectedDeliveryDate": "2024-01-25T18:00:00Z",
    "status": "scheduled",
    "vehicleDetails": {
      "vin": "1HGBH41JXMN109186",
      "make": "Toyota",
      "model": "Camry",
      "year": 2023
    },
    "createdAt": "2024-01-16T12:00:00Z",
    "updatedAt": "2024-01-16T12:00:00Z"
  }'
```

### 4. Bulk Index Documents

```bash
curl -X POST "http://localhost:5000/api/index/bulk" \
  -H "Content-Type: application/json" \
  -d '{
    "entityType": "offers",
    "documents": [
      {
        "offerId": "OFF-10001",
        "sellerId": "SELLER-002",
        "vin": "2HGBH41JXMN109187",
        "make": "Honda",
        "model": "Accord",
        "year": 2023,
        "offerAmount": 26500.00,
        "location": {
          "city": "Chicago",
          "state": "IL",
          "country": "USA"
        },
        "condition": "New",
        "status": "available",
        "createdAt": "2024-01-01T00:00:00Z",
        "updatedAt": "2024-01-01T00:00:00Z"
      },
      {
        "offerId": "OFF-10002",
        "sellerId": "SELLER-003",
        "vin": "3HGBH41JXMN109188",
        "make": "Ford",
        "model": "F-150",
        "year": 2022,
        "offerAmount": 35000.00,
        "location": {
          "city": "Houston",
          "state": "TX",
          "country": "USA"
        },
        "condition": "Used",
        "status": "available",
        "createdAt": "2024-01-01T00:00:00Z",
        "updatedAt": "2024-01-01T00:00:00Z"
      }
    ]
  }'
```

### 5. Delete Document

```bash
# Delete an offer
curl -X DELETE "http://localhost:5000/api/index/offers/OFF-12345"

# Delete a purchase
curl -X DELETE "http://localhost:5000/api/index/purchases/PUR-67890"

# Delete a transport
curl -X DELETE "http://localhost:5000/api/index/transports/TRN-11111"

# Response
{
  "success": true,
  "message": "Document deleted successfully"
}
```

---

## Health & Monitoring

### 1. Health Check

```bash
curl "http://localhost:5000/api/health"

# Response
{
  "status": "Healthy",
  "components": {
    "Elasticsearch": "Healthy",
    "Indices": "Healthy",
    "API": "Healthy"
  },
  "timestamp": "2024-01-01T12:00:00Z"
}
```

### 2. Metrics

```bash
curl "http://localhost:5000/api/health/metrics"

# Response
{
  "cluster": {
    "name": "docker-cluster",
    "status": "green",
    "nodes": 1,
    "indices": 3
  },
  "timestamp": "2024-01-01T12:00:00Z"
}
```

---

## Role-based Access Examples

### Seller Role

Sellers can only see their own offers:

```bash
# This returns only offers where sellerId = "SELLER-001"
curl "http://localhost:5000/api/search/offers?query=*&userRole=seller&userId=SELLER-001"

# Trying to access purchases returns empty results
curl "http://localhost:5000/api/search/purchases?query=*&userRole=seller&userId=SELLER-001"
```

### Buyer Role

Buyers can see all available offers and their own purchases:

```bash
# See all available offers
curl "http://localhost:5000/api/search/offers?query=Toyota&userRole=buyer&userId=BUYER-001"

# See only their purchases
curl "http://localhost:5000/api/search/purchases?query=*&userRole=buyer&userId=BUYER-001"

# Cannot access transports
curl "http://localhost:5000/api/search/transports?query=*&userRole=buyer&userId=BUYER-001"
```

### Carrier Role

Carriers can see their assigned transports and related offers:

```bash
# See only their transports
curl "http://localhost:5000/api/search/transports?query=*&userRole=carrier&userId=CARRIER-001"

# Can see related offers
curl "http://localhost:5000/api/search/offers?query=Toyota&userRole=carrier&userId=CARRIER-001"

# Cannot see purchases
curl "http://localhost:5000/api/search/purchases?query=*&userRole=carrier&userId=CARRIER-001"
```

### Agent Role

Agents have full access to all data:

```bash
# Can search everything
curl "http://localhost:5000/api/search/unified?query=Toyota&userRole=agent&userId=AGENT-001"
curl "http://localhost:5000/api/search/offers?query=*&userRole=agent&userId=AGENT-001"
curl "http://localhost:5000/api/search/purchases?query=*&userRole=agent&userId=AGENT-001"
curl "http://localhost:5000/api/search/transports?query=*&userRole=agent&userId=AGENT-001"
```

---

## Advanced Query Examples

### 1. Fuzzy Search (Typo Tolerance)

```bash
# Search for "Toyoto" (misspelled) - still finds "Toyota"
curl "http://localhost:5000/api/search/offers?query=Toyoto&enableFuzzy=true&userRole=agent&userId=AGENT-001"

# Disable fuzzy matching
curl "http://localhost:5000/api/search/offers?query=Toyoto&enableFuzzy=false&userRole=agent&userId=AGENT-001"
```

### 2. Multi-field Search

Searches across multiple fields automatically:

```bash
# Searches make, model, VIN, location simultaneously
curl "http://localhost:5000/api/search/offers?query=New%20York%20Toyota&userRole=agent&userId=AGENT-001"
```

### 3. Pagination

```bash
# First page (results 1-10)
curl "http://localhost:5000/api/search/offers?query=*&page=1&pageSize=10&userRole=agent&userId=AGENT-001"

# Second page (results 11-20)
curl "http://localhost:5000/api/search/offers?query=*&page=2&pageSize=10&userRole=agent&userId=AGENT-001"

# Large page size (results 1-50)
curl "http://localhost:5000/api/search/offers?query=*&page=1&pageSize=50&userRole=agent&userId=AGENT-001"
```

### 4. Highlighting

Enable search term highlighting in results:

```bash
curl "http://localhost:5000/api/search/offers?query=Toyota&enableHighlighting=true&userRole=agent&userId=AGENT-001"

# Response includes highlights
{
  "results": [
    {
      "document": { ... },
      "score": 2.5,
      "highlights": {
        "make": ["<em>Toyota</em>"],
        "model": ["<em>Toyota</em> Camry"]
      }
    }
  ]
}
```

---

## Testing with Python

```python
import requests

BASE_URL = "http://localhost:5000/api"

# Search offers
response = requests.get(f"{BASE_URL}/search/offers", params={
    "query": "Toyota",
    "userRole": "agent",
    "userId": "AGENT-001",
    "page": 1,
    "pageSize": 10
})

print(f"Status: {response.status_code}")
print(f"Results: {response.json()}")

# Index a document
offer = {
    "offerId": "OFF-99999",
    "sellerId": "SELLER-999",
    "vin": "TEST123456789ABCD",
    "make": "Tesla",
    "model": "Model 3",
    "year": 2024,
    "offerAmount": 45000.00,
    "location": {
        "city": "San Francisco",
        "state": "CA",
        "country": "USA"
    },
    "condition": "New",
    "status": "available",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
}

response = requests.post(f"{BASE_URL}/index/offers", json=offer)
print(f"Index Status: {response.status_code}")
print(f"Response: {response.json()}")
```

---

## Notes

- All date fields use ISO 8601 format: `YYYY-MM-DDTHH:MM:SSZ`
- VINs must be exactly 17 characters
- Search queries support wildcards: `*` matches any sequence
- Empty query (`query=*`) returns all documents (subject to role-based filtering)
- Pagination starts at page 1
- Maximum page size is 100 documents
