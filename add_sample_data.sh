# Sample Data for automotive_search Index
# Execute these curl commands to populate your Elasticsearch index

# Sample 1 - Honda Accord Purchase
curl -X POST "localhost:9200/automotive_search/_doc/transport_2" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 101,
  "transport_id": 2,
  "purchase_id": 2,
  "buyer_id": 789,
  "seller_id": 234,
  "offer_id": 2,
  "vin": "1HGCV1F36LA012345",
  "make": "Honda",
  "model": "Accord",
  "trim": "Sport",
  "year": 2022,
  "offer_amount": 26500.00,
  "bid_amount": 25800.00,
  "offer_status": "accepted",
  "purchase_status": "completed",
  "transport_status": "delivered",
  "city": "Miami",
  "state": "FL",
  "search_text": "2022 Honda Accord Sport sedan Miami Florida delivery"
}'

# Sample 2 - Ford F-150 Pending Transport
curl -X POST "localhost:9200/automotive_search/_doc/transport_3" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 202,
  "transport_id": 3,
  "purchase_id": 3,
  "buyer_id": 321,
  "seller_id": 456,
  "offer_id": 3,
  "vin": "1FTFW1E84MKE12345",
  "make": "Ford",
  "model": "F-150",
  "trim": "XLT",
  "year": 2024,
  "offer_amount": 42000.00,
  "bid_amount": 41200.00,
  "offer_status": "pending",
  "purchase_status": "pending",
  "transport_status": "scheduled",
  "city": "Dallas",
  "state": "TX",
  "search_text": "2024 Ford F-150 XLT pickup truck Dallas Texas transport scheduled"
}'

# Sample 3 - Tesla Model 3 In Transit
curl -X POST "localhost:9200/automotive_search/_doc/transport_4" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 303,
  "transport_id": 4,
  "purchase_id": 4,
  "buyer_id": 654,
  "seller_id": 789,
  "offer_id": 4,
  "vin": "5YJ3E1EA9MF012345",
  "make": "Tesla",
  "model": "Model 3",
  "trim": "Long Range",
  "year": 2023,
  "offer_amount": 38900.00,
  "bid_amount": 38000.00,
  "offer_status": "accepted",
  "purchase_status": "in_progress",
  "transport_status": "in_transit",
  "city": "San Francisco",
  "state": "CA",
  "search_text": "2023 Tesla Model 3 Long Range electric vehicle San Francisco California shipping"
}'

# Sample 4 - Chevrolet Silverado Rejected Offer
curl -X POST "localhost:9200/automotive_search/_doc/transport_5" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 404,
  "transport_id": 5,
  "purchase_id": 5,
  "buyer_id": 987,
  "seller_id": 147,
  "offer_id": 5,
  "vin": "1GC4YUE79MF123456",
  "make": "Chevrolet",
  "model": "Silverado",
  "trim": "LTZ",
  "year": 2023,
  "offer_amount": 45000.00,
  "bid_amount": 43500.00,
  "offer_status": "rejected",
  "purchase_status": "cancelled",
  "transport_status": "not_scheduled",
  "city": "Houston",
  "state": "TX",
  "search_text": "2023 Chevrolet Silverado LTZ truck Houston Texas cancelled"
}'

# Sample 5 - BMW X5 Completed Delivery
curl -X POST "localhost:9200/automotive_search/_doc/transport_6" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 505,
  "transport_id": 6,
  "purchase_id": 6,
  "buyer_id": 258,
  "seller_id": 369,
  "offer_id": 6,
  "vin": "5UXCR6C0XML123456",
  "make": "BMW",
  "model": "X5",
  "trim": "xDrive40i",
  "year": 2024,
  "offer_amount": 62000.00,
  "bid_amount": 61500.00,
  "offer_status": "accepted",
  "purchase_status": "completed",
  "transport_status": "delivered",
  "city": "New York",
  "state": "NY",
  "search_text": "2024 BMW X5 xDrive40i luxury SUV New York delivery completed"
}'

# Sample 6 - Jeep Wrangler Pending Purchase
curl -X POST "localhost:9200/automotive_search/_doc/transport_7" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 606,
  "transport_id": 7,
  "purchase_id": 7,
  "buyer_id": 741,
  "seller_id": 852,
  "offer_id": 7,
  "vin": "1C4HJXDG5MW123456",
  "make": "Jeep",
  "model": "Wrangler",
  "trim": "Rubicon",
  "year": 2023,
  "offer_amount": 48500.00,
  "bid_amount": 47800.00,
  "offer_status": "pending",
  "purchase_status": "pending",
  "transport_status": "not_scheduled",
  "city": "Denver",
  "state": "CO",
  "search_text": "2023 Jeep Wrangler Rubicon 4x4 off-road Denver Colorado pending"
}'

# Sample 7 - Mercedes-Benz C-Class In Progress
curl -X POST "localhost:9200/automotive_search/_doc/transport_8" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 707,
  "transport_id": 8,
  "purchase_id": 8,
  "buyer_id": 963,
  "seller_id": 159,
  "offer_id": 8,
  "vin": "55SWF4KB5MU123456",
  "make": "Mercedes-Benz",
  "model": "C-Class",
  "trim": "C 300",
  "year": 2023,
  "offer_amount": 45800.00,
  "bid_amount": 45200.00,
  "offer_status": "accepted",
  "purchase_status": "in_progress",
  "transport_status": "in_transit",
  "city": "Atlanta",
  "state": "GA",
  "search_text": "2023 Mercedes-Benz C-Class C300 sedan luxury Atlanta Georgia shipping"
}'

# Sample 8 - Nissan Altima Cancelled
curl -X POST "localhost:9200/automotive_search/_doc/transport_9" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 808,
  "transport_id": 9,
  "purchase_id": 9,
  "buyer_id": 357,
  "seller_id": 951,
  "offer_id": 9,
  "vin": "1N4BL4BV8MN123456",
  "make": "Nissan",
  "model": "Altima",
  "trim": "SV",
  "year": 2022,
  "offer_amount": 24500.00,
  "bid_amount": 23800.00,
  "offer_status": "rejected",
  "purchase_status": "cancelled",
  "transport_status": "not_scheduled",
  "city": "Seattle",
  "state": "WA",
  "search_text": "2022 Nissan Altima SV sedan Seattle Washington cancelled offer"
}'

# Sample 9 - Audi Q7 Scheduled Transport
curl -X POST "localhost:9200/automotive_search/_doc/transport_10" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 909,
  "transport_id": 10,
  "purchase_id": 10,
  "buyer_id": 159,
  "seller_id": 753,
  "offer_id": 10,
  "vin": "WA1AVAFY5MD123456",
  "make": "Audi",
  "model": "Q7",
  "trim": "Premium Plus",
  "year": 2024,
  "offer_amount": 58900.00,
  "bid_amount": 58200.00,
  "offer_status": "accepted",
  "purchase_status": "confirmed",
  "transport_status": "scheduled",
  "city": "Boston",
  "state": "MA",
  "search_text": "2024 Audi Q7 Premium Plus luxury SUV Boston Massachusetts transport scheduled"
}'

# Sample 10 - Ram 1500 Delivered
curl -X POST "localhost:9200/automotive_search/_doc/transport_11" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 111,
  "transport_id": 11,
  "purchase_id": 11,
  "buyer_id": 486,
  "seller_id": 297,
  "offer_id": 11,
  "vin": "1C6SRFFT4MN123456",
  "make": "Ram",
  "model": "1500",
  "trim": "Laramie",
  "year": 2023,
  "offer_amount": 49500.00,
  "bid_amount": 48900.00,
  "offer_status": "accepted",
  "purchase_status": "completed",
  "transport_status": "delivered",
  "city": "Chicago",
  "state": "IL",
  "search_text": "2023 Ram 1500 Laramie pickup truck Chicago Illinois delivered completed"
}'

# Sample 11 - Hyundai Sonata Pending
curl -X POST "localhost:9200/automotive_search/_doc/transport_12" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 222,
  "transport_id": 12,
  "purchase_id": 12,
  "buyer_id": 624,
  "seller_id": 813,
  "offer_id": 12,
  "vin": "5NPE24AF3MH123456",
  "make": "Hyundai",
  "model": "Sonata",
  "trim": "SEL Plus",
  "year": 2023,
  "offer_amount": 27800.00,
  "bid_amount": 27200.00,
  "offer_status": "pending",
  "purchase_status": "pending",
  "transport_status": "not_scheduled",
  "city": "Portland",
  "state": "OR",
  "search_text": "2023 Hyundai Sonata SEL Plus sedan Portland Oregon pending offer"
}'

# Sample 12 - Subaru Outback In Transit
curl -X POST "localhost:9200/automotive_search/_doc/transport_13" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 333,
  "transport_id": 13,
  "purchase_id": 13,
  "buyer_id": 918,
  "seller_id": 426,
  "offer_id": 13,
  "vin": "4S4BTACC5M3123456",
  "make": "Subaru",
  "model": "Outback",
  "trim": "Limited XT",
  "year": 2024,
  "offer_amount": 39200.00,
  "bid_amount": 38600.00,
  "offer_status": "accepted",
  "purchase_status": "in_progress",
  "transport_status": "in_transit",
  "city": "Salt Lake City",
  "state": "UT",
  "search_text": "2024 Subaru Outback Limited XT wagon AWD Salt Lake City Utah shipping"
}'

# Sample 13 - Lexus RX Delivered
curl -X POST "localhost:9200/automotive_search/_doc/transport_14" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 444,
  "transport_id": 14,
  "purchase_id": 14,
  "buyer_id": 537,
  "seller_id": 642,
  "offer_id": 14,
  "vin": "2T2HZMDA5MC123456",
  "make": "Lexus",
  "model": "RX",
  "trim": "350 F Sport",
  "year": 2023,
  "offer_amount": 52800.00,
  "bid_amount": 52100.00,
  "offer_status": "accepted",
  "purchase_status": "completed",
  "transport_status": "delivered",
  "city": "Phoenix",
  "state": "AZ",
  "search_text": "2023 Lexus RX 350 F Sport luxury SUV Phoenix Arizona delivered"
}'

# Sample 14 - Mazda CX-5 Scheduled
curl -X POST "localhost:9200/automotive_search/_doc/transport_15" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 555,
  "transport_id": 15,
  "purchase_id": 15,
  "buyer_id": 246,
  "seller_id": 891,
  "offer_id": 15,
  "vin": "JM3KFBDM5M0123456",
  "make": "Mazda",
  "model": "CX-5",
  "trim": "Grand Touring",
  "year": 2023,
  "offer_amount": 32500.00,
  "bid_amount": 31900.00,
  "offer_status": "accepted",
  "purchase_status": "confirmed",
  "transport_status": "scheduled",
  "city": "Las Vegas",
  "state": "NV",
  "search_text": "2023 Mazda CX-5 Grand Touring SUV Las Vegas Nevada transport scheduled"
}'

# Sample 15 - Volkswagen Jetta Rejected
curl -X POST "localhost:9200/automotive_search/_doc/transport_16" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 666,
  "transport_id": 16,
  "purchase_id": 16,
  "buyer_id": 372,
  "seller_id": 684,
  "offer_id": 16,
  "vin": "3VWC57BU8MM123456",
  "make": "Volkswagen",
  "model": "Jetta",
  "trim": "SEL Premium",
  "year": 2022,
  "offer_amount": 26800.00,
  "bid_amount": 26100.00,
  "offer_status": "rejected",
  "purchase_status": "cancelled",
  "transport_status": "not_scheduled",
  "city": "Philadelphia",
  "state": "PA",
  "search_text": "2022 Volkswagen Jetta SEL Premium sedan Philadelphia Pennsylvania cancelled"
}'

# Sample 16 - Kia Telluride In Progress
curl -X POST "localhost:9200/automotive_search/_doc/transport_17" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 777,
  "transport_id": 17,
  "purchase_id": 17,
  "buyer_id": 819,
  "seller_id": 273,
  "offer_id": 17,
  "vin": "5XYP5DHC0MG123456",
  "make": "Kia",
  "model": "Telluride",
  "trim": "SX Prestige",
  "year": 2024,
  "offer_amount": 48900.00,
  "bid_amount": 48200.00,
  "offer_status": "accepted",
  "purchase_status": "in_progress",
  "transport_status": "in_transit",
  "city": "Minneapolis",
  "state": "MN",
  "search_text": "2024 Kia Telluride SX Prestige SUV Minneapolis Minnesota shipping in transit"
}'

# Sample 17 - GMC Sierra Delivered
curl -X POST "localhost:9200/automotive_search/_doc/transport_18" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 888,
  "transport_id": 18,
  "purchase_id": 18,
  "buyer_id": 495,
  "seller_id": 138,
  "offer_id": 18,
  "vin": "1GTU9EED5MZ123456",
  "make": "GMC",
  "model": "Sierra",
  "trim": "Denali",
  "year": 2023,
  "offer_amount": 61500.00,
  "bid_amount": 60800.00,
  "offer_status": "accepted",
  "purchase_status": "completed",
  "transport_status": "delivered",
  "city": "Nashville",
  "state": "TN",
  "search_text": "2023 GMC Sierra Denali luxury pickup truck Nashville Tennessee delivered"
}'

# Sample 18 - Acura MDX Scheduled
curl -X POST "localhost:9200/automotive_search/_doc/transport_19" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 999,
  "transport_id": 19,
  "purchase_id": 19,
  "buyer_id": 561,
  "seller_id": 927,
  "offer_id": 19,
  "vin": "5J8YD4H86ML123456",
  "make": "Acura",
  "model": "MDX",
  "trim": "Type S",
  "year": 2024,
  "offer_amount": 68900.00,
  "bid_amount": 68200.00,
  "offer_status": "accepted",
  "purchase_status": "confirmed",
  "transport_status": "scheduled",
  "city": "San Diego",
  "state": "CA",
  "search_text": "2024 Acura MDX Type S luxury SUV performance San Diego California scheduled"
}'

# Sample 19 - Dodge Challenger Pending
curl -X POST "localhost:9200/automotive_search/_doc/transport_20" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 123,
  "transport_id": 20,
  "purchase_id": 20,
  "buyer_id": 684,
  "seller_id": 315,
  "offer_id": 20,
  "vin": "2C3CDZC98MH123456",
  "make": "Dodge",
  "model": "Challenger",
  "trim": "R/T Scat Pack",
  "year": 2023,
  "offer_amount": 46800.00,
  "bid_amount": 46100.00,
  "offer_status": "pending",
  "purchase_status": "pending",
  "transport_status": "not_scheduled",
  "city": "Charlotte",
  "state": "NC",
  "search_text": "2023 Dodge Challenger R/T Scat Pack muscle car Charlotte North Carolina pending"
}'

# Sample 20 - Porsche Cayenne Delivered
curl -X POST "localhost:9200/automotive_search/_doc/transport_21" \
-H 'Content-Type: application/json' \
-d'{
  "carrier_id": 234,
  "transport_id": 21,
  "purchase_id": 21,
  "buyer_id": 729,
  "seller_id": 486,
  "offer_id": 21,
  "vin": "WP1AB2A59MLA12345",
  "make": "Porsche",
  "model": "Cayenne",
  "trim": "S",
  "year": 2024,
  "offer_amount": 89900.00,
  "bid_amount": 89200.00,
  "offer_status": "accepted",
  "purchase_status": "completed",
  "transport_status": "delivered",
  "city": "Austin",
  "state": "TX",
  "search_text": "2024 Porsche Cayenne S luxury performance SUV Austin Texas delivered completed"
}'

echo ""
echo "✅ Successfully added 20 sample records to automotive_search index"
echo ""
echo "To verify, run:"
echo "curl -X GET 'localhost:9200/automotive_search/_count'"