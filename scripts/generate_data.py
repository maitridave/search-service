#!/usr/bin/env python3
"""
Data Generator for Search Service
Generates realistic test data for offers, purchases, and transports
"""

import json
import random
import string
from datetime import datetime, timedelta
import requests

# Configuration
API_BASE_URL = "http://localhost:5000/api/index"
NUM_OFFERS = 1000
NUM_PURCHASES = 500
NUM_TRANSPORTS = 300

# Sample data
MAKES = ["Toyota", "Honda", "Ford", "Chevrolet", "BMW", "Mercedes-Benz", "Audi", "Tesla", "Nissan", "Volkswagen"]
MODELS = {
    "Toyota": ["Camry", "Corolla", "RAV4", "Highlander"],
    "Honda": ["Civic", "Accord", "CR-V", "Pilot"],
    "Ford": ["F-150", "Mustang", "Explorer", "Escape"],
    "Chevrolet": ["Silverado", "Malibu", "Equinox", "Tahoe"],
    "BMW": ["3 Series", "5 Series", "X3", "X5"],
    "Mercedes-Benz": ["C-Class", "E-Class", "GLC", "GLE"],
    "Audi": ["A4", "A6", "Q5", "Q7"],
    "Tesla": ["Model 3", "Model S", "Model X", "Model Y"],
    "Nissan": ["Altima", "Sentra", "Rogue", "Pathfinder"],
    "Volkswagen": ["Jetta", "Passat", "Tiguan", "Atlas"]
}

CITIES = ["New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose"]
STATES = ["NY", "CA", "IL", "TX", "AZ", "PA", "TX", "CA", "TX", "CA"]
CONDITIONS = ["New", "Used", "Certified"]
STATUSES = ["available", "sold", "pending", "reserved"]
PAYMENT_METHODS = ["Credit Card", "Bank Transfer", "PayPal", "Financing"]

def generate_vin():
    """Generate a random VIN"""
    chars = string.ascii_uppercase.replace('I', '').replace('O', '').replace('Q', '') + string.digits
    return ''.join(random.choices(chars, k=17))

def generate_offer():
    """Generate a random offer"""
    make = random.choice(MAKES)
    model = random.choice(MODELS[make])
    city_idx = random.randint(0, len(CITIES) - 1)
    
    return {
        "offerId": f"OFF-{random.randint(10000, 99999)}",
        "sellerId": f"SELLER-{random.randint(1000, 9999)}",
        "vin": generate_vin(),
        "make": make,
        "model": model,
        "year": random.randint(2015, 2024),
        "offerAmount": round(random.uniform(15000, 75000), 2),
        "location": {
            "city": CITIES[city_idx],
            "state": STATES[city_idx],
            "country": "USA"
        },
        "condition": random.choice(CONDITIONS),
        "status": random.choice(STATUSES),
        "createdAt": (datetime.now() - timedelta(days=random.randint(1, 365))).isoformat(),
        "updatedAt": datetime.now().isoformat()
    }

def generate_purchase(offer_id):
    """Generate a random purchase"""
    return {
        "purchaseId": f"PUR-{random.randint(10000, 99999)}",
        "buyerId": f"BUYER-{random.randint(1000, 9999)}",
        "offerId": offer_id,
        "purchaseDate": (datetime.now() - timedelta(days=random.randint(1, 180))).isoformat(),
        "amount": round(random.uniform(15000, 75000), 2),
        "status": random.choice(["completed", "pending", "cancelled"]),
        "buyerDetails": {
            "name": f"Buyer {random.randint(1000, 9999)}",
            "contact": f"+1-{random.randint(200, 999)}-{random.randint(200, 999)}-{random.randint(1000, 9999)}"
        },
        "paymentMethod": random.choice(PAYMENT_METHODS),
        "createdAt": (datetime.now() - timedelta(days=random.randint(1, 180))).isoformat(),
        "updatedAt": datetime.now().isoformat()
    }

def generate_transport(purchase_id):
    """Generate a random transport"""
    pickup_idx = random.randint(0, len(CITIES) - 1)
    delivery_idx = random.randint(0, len(CITIES) - 1)
    schedule_date = datetime.now() + timedelta(days=random.randint(1, 30))
    
    return {
        "transportId": f"TRN-{random.randint(10000, 99999)}",
        "carrierId": f"CARRIER-{random.randint(1000, 9999)}",
        "purchaseId": purchase_id,
        "pickupLocation": f"{CITIES[pickup_idx]}, {STATES[pickup_idx]}",
        "deliveryLocation": f"{CITIES[delivery_idx]}, {STATES[delivery_idx]}",
        "scheduleDate": schedule_date.isoformat(),
        "actualPickupDate": None if random.random() > 0.5 else (schedule_date + timedelta(days=1)).isoformat(),
        "expectedDeliveryDate": (schedule_date + timedelta(days=random.randint(3, 10))).isoformat(),
        "status": random.choice(["scheduled", "in-transit", "delivered", "cancelled"]),
        "vehicleDetails": {
            "vin": generate_vin(),
            "make": random.choice(MAKES),
            "model": random.choice(MODELS[random.choice(MAKES)]),
            "year": random.randint(2015, 2024)
        },
        "createdAt": (datetime.now() - timedelta(days=random.randint(1, 90))).isoformat(),
        "updatedAt": datetime.now().isoformat()
    }

def index_documents(entity_type, documents):
    """Index documents via API"""
    url = f"{API_BASE_URL}/{entity_type}"
    
    for doc in documents:
        try:
            response = requests.post(url, json=doc, timeout=10)
            if response.status_code == 200:
                print(f"✓ Indexed {entity_type}: {doc.get(entity_type[:-1] + 'Id', 'unknown')}")
            else:
                print(f"✗ Failed to index {entity_type}: {response.status_code}")
        except Exception as e:
            print(f"✗ Error indexing {entity_type}: {e}")

def main():
    print("=" * 60)
    print("Search Service Data Generator")
    print("=" * 60)
    
    # Generate and index offers
    print(f"\n📦 Generating {NUM_OFFERS} offers...")
    offers = [generate_offer() for _ in range(NUM_OFFERS)]
    print(f"✓ Generated {len(offers)} offers")
    
    print("\n📤 Indexing offers...")
    index_documents("offers", offers)
    
    # Generate and index purchases
    print(f"\n📦 Generating {NUM_PURCHASES} purchases...")
    purchases = [generate_purchase(random.choice(offers)["offerId"]) for _ in range(NUM_PURCHASES)]
    print(f"✓ Generated {len(purchases)} purchases")
    
    print("\n📤 Indexing purchases...")
    index_documents("purchases", purchases)
    
    # Generate and index transports
    print(f"\n📦 Generating {NUM_TRANSPORTS} transports...")
    transports = [generate_transport(random.choice(purchases)["purchaseId"]) for _ in range(NUM_TRANSPORTS)]
    print(f"✓ Generated {len(transports)} transports")
    
    print("\n📤 Indexing transports...")
    index_documents("transports", transports)
    
    print("\n" + "=" * 60)
    print("✅ Data generation complete!")
    print(f"   - {NUM_OFFERS} offers")
    print(f"   - {NUM_PURCHASES} purchases")
    print(f"   - {NUM_TRANSPORTS} transports")
    print("=" * 60)

if __name__ == "__main__":
    main()
