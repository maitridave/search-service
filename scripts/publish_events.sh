#!/bin/bash

echo "=========================================="
echo "Search Service - Mock Event Publisher"
echo "=========================================="

# RabbitMQ connection details
RABBITMQ_HOST="${RABBITMQ_HOST:-localhost}"
RABBITMQ_PORT="${RABBITMQ_PORT:-5672}"
RABBITMQ_USER="${RABBITMQ_USER:-guest}"
RABBITMQ_PASS="${RABBITMQ_PASS:-guest}"

# Number of events to publish
NUM_EVENTS="${NUM_EVENTS:-10}"

echo ""
echo "Publishing $NUM_EVENTS mock events to RabbitMQ..."
echo ""

# Function to generate a random VIN
generate_vin() {
    cat /dev/urandom | tr -dc 'A-HJ-NPR-Z0-9' | fold -w 17 | head -n 1
}

# Function to publish offer event
publish_offer() {
    local offer_id="OFF-$RANDOM"
    local seller_id="SELLER-$RANDOM"
    local vin=$(generate_vin)
    
    local payload=$(cat <<EOF
{
  "offerId": "$offer_id",
  "sellerId": "$seller_id",
  "vin": "$vin",
  "make": "Toyota",
  "model": "Camry",
  "year": 2023,
  "offerAmount": 25000.00,
  "location": {
    "city": "New York",
    "state": "NY",
    "country": "USA"
  },
  "condition": "New",
  "status": "available",
  "createdAt": "$(date -u +"%Y-%m-%dT%H:%M:%SZ")",
  "updatedAt": "$(date -u +"%Y-%m-%dT%H:%M:%SZ")"
}
EOF
)

    echo "📤 Publishing Offer: $offer_id"
    
    # Using RabbitMQ HTTP API
    curl -s -u "$RABBITMQ_USER:$RABBITMQ_PASS" \
        -H "Content-Type: application/json" \
        -X POST "http://$RABBITMQ_HOST:15672/api/exchanges/%2F/amq.default/publish" \
        -d "{
            \"properties\":{},
            \"routing_key\":\"offers-queue\",
            \"payload\":\"$(echo $payload | base64 -w 0)\",
            \"payload_encoding\":\"base64\"
        }" > /dev/null
    
    if [ $? -eq 0 ]; then
        echo "   ✓ Published successfully"
    else
        echo "   ✗ Failed to publish"
    fi
}

# Publish events
for i in $(seq 1 $NUM_EVENTS); do
    publish_offer
    sleep 0.5
done

echo ""
echo "=========================================="
echo "✅ Mock event publishing complete!"
echo "=========================================="
