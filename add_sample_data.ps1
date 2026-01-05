# PowerShell Script to Add Sample Data to Elasticsearch
# Save as: add_sample_data.ps1
# Run with: .\add_sample_data.ps1

$baseUri = "http://localhost:9200/automotive_search/_doc"

# Sample 2 - Honda Accord
$data2 = @{
    carrier_id = 101
    transport_id = 2
    purchase_id = 2
    buyer_id = 789
    seller_id = 234
    offer_id = 2
    vin = "1HGCV1F36LA012345"
    make = "Honda"
    model = "Accord"
    trim = "Sport"
    year = 2022
    offer_amount = 26500.00
    bid_amount = 25800.00
    offer_status = "accepted"
    purchase_status = "completed"
    transport_status = "delivered"
    city = "Miami"
    state = "FL"
    search_text = "2022 Honda Accord Sport sedan Miami Florida delivery"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_2" -Method Post -Body $data2 -ContentType "application/json"
Write-Host "✓ Added Honda Accord" -ForegroundColor Green

# Sample 3 - Ford F-150
$data3 = @{
    carrier_id = 202
    transport_id = 3
    purchase_id = 3
    buyer_id = 321
    seller_id = 456
    offer_id = 3
    vin = "1FTFW1E84MKE12345"
    make = "Ford"
    model = "F-150"
    trim = "XLT"
    year = 2024
    offer_amount = 42000.00
    bid_amount = 41200.00
    offer_status = "pending"
    purchase_status = "pending"
    transport_status = "scheduled"
    city = "Dallas"
    state = "TX"
    search_text = "2024 Ford F-150 XLT pickup truck Dallas Texas transport scheduled"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_3" -Method Post -Body $data3 -ContentType "application/json"
Write-Host "✓ Added Ford F-150" -ForegroundColor Green

# Sample 4 - Tesla Model 3
$data4 = @{
    carrier_id = 303
    transport_id = 4
    purchase_id = 4
    buyer_id = 654
    seller_id = 789
    offer_id = 4
    vin = "5YJ3E1EA9MF012345"
    make = "Tesla"
    model = "Model 3"
    trim = "Long Range"
    year = 2023
    offer_amount = 38900.00
    bid_amount = 38000.00
    offer_status = "accepted"
    purchase_status = "in_progress"
    transport_status = "in_transit"
    city = "San Francisco"
    state = "CA"
    search_text = "2023 Tesla Model 3 Long Range electric vehicle San Francisco California shipping"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_4" -Method Post -Body $data4 -ContentType "application/json"
Write-Host "✓ Added Tesla Model 3" -ForegroundColor Green

# Sample 5 - Chevrolet Silverado
$data5 = @{
    carrier_id = 404
    transport_id = 5
    purchase_id = 5
    buyer_id = 987
    seller_id = 147
    offer_id = 5
    vin = "1GC4YUE79MF123456"
    make = "Chevrolet"
    model = "Silverado"
    trim = "LTZ"
    year = 2023
    offer_amount = 45000.00
    bid_amount = 43500.00
    offer_status = "rejected"
    purchase_status = "cancelled"
    transport_status = "not_scheduled"
    city = "Houston"
    state = "TX"
    search_text = "2023 Chevrolet Silverado LTZ truck Houston Texas cancelled"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_5" -Method Post -Body $data5 -ContentType "application/json"
Write-Host "✓ Added Chevrolet Silverado" -ForegroundColor Green

# Sample 6 - BMW X5
$data6 = @{
    carrier_id = 505
    transport_id = 6
    purchase_id = 6
    buyer_id = 258
    seller_id = 369
    offer_id = 6
    vin = "5UXCR6C0XML123456"
    make = "BMW"
    model = "X5"
    trim = "xDrive40i"
    year = 2024
    offer_amount = 62000.00
    bid_amount = 61500.00
    offer_status = "accepted"
    purchase_status = "completed"
    transport_status = "delivered"
    city = "New York"
    state = "NY"
    search_text = "2024 BMW X5 xDrive40i luxury SUV New York delivery completed"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_6" -Method Post -Body $data6 -ContentType "application/json"
Write-Host "✓ Added BMW X5" -ForegroundColor Green

# Sample 7 - Jeep Wrangler
$data7 = @{
    carrier_id = 606
    transport_id = 7
    purchase_id = 7
    buyer_id = 741
    seller_id = 852
    offer_id = 7
    vin = "1C4HJXDG5MW123456"
    make = "Jeep"
    model = "Wrangler"
    trim = "Rubicon"
    year = 2023
    offer_amount = 48500.00
    bid_amount = 47800.00
    offer_status = "pending"
    purchase_status = "pending"
    transport_status = "not_scheduled"
    city = "Denver"
    state = "CO"
    search_text = "2023 Jeep Wrangler Rubicon 4x4 off-road Denver Colorado pending"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_7" -Method Post -Body $data7 -ContentType "application/json"
Write-Host "✓ Added Jeep Wrangler" -ForegroundColor Green

# Sample 8 - Mercedes-Benz C-Class
$data8 = @{
    carrier_id = 707
    transport_id = 8
    purchase_id = 8
    buyer_id = 963
    seller_id = 159
    offer_id = 8
    vin = "55SWF4KB5MU123456"
    make = "Mercedes-Benz"
    model = "C-Class"
    trim = "C 300"
    year = 2023
    offer_amount = 45800.00
    bid_amount = 45200.00
    offer_status = "accepted"
    purchase_status = "in_progress"
    transport_status = "in_transit"
    city = "Atlanta"
    state = "GA"
    search_text = "2023 Mercedes-Benz C-Class C300 sedan luxury Atlanta Georgia shipping"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_8" -Method Post -Body $data8 -ContentType "application/json"
Write-Host "✓ Added Mercedes-Benz C-Class" -ForegroundColor Green

# Sample 9 - Nissan Altima
$data9 = @{
    carrier_id = 808
    transport_id = 9
    purchase_id = 9
    buyer_id = 357
    seller_id = 951
    offer_id = 9
    vin = "1N4BL4BV8MN123456"
    make = "Nissan"
    model = "Altima"
    trim = "SV"
    year = 2022
    offer_amount = 24500.00
    bid_amount = 23800.00
    offer_status = "rejected"
    purchase_status = "cancelled"
    transport_status = "not_scheduled"
    city = "Seattle"
    state = "WA"
    search_text = "2022 Nissan Altima SV sedan Seattle Washington cancelled offer"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_9" -Method Post -Body $data9 -ContentType "application/json"
Write-Host "✓ Added Nissan Altima" -ForegroundColor Green

# Sample 10 - Audi Q7
$data10 = @{
    carrier_id = 909
    transport_id = 10
    purchase_id = 10
    buyer_id = 159
    seller_id = 753
    offer_id = 10
    vin = "WA1AVAFY5MD123456"
    make = "Audi"
    model = "Q7"
    trim = "Premium Plus"
    year = 2024
    offer_amount = 58900.00
    bid_amount = 58200.00
    offer_status = "accepted"
    purchase_status = "confirmed"
    transport_status = "scheduled"
    city = "Boston"
    state = "MA"
    search_text = "2024 Audi Q7 Premium Plus luxury SUV Boston Massachusetts transport scheduled"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_10" -Method Post -Body $data10 -ContentType "application/json"
Write-Host "✓ Added Audi Q7" -ForegroundColor Green

# Sample 11 - Ram 1500
$data11 = @{
    carrier_id = 111
    transport_id = 11
    purchase_id = 11
    buyer_id = 486
    seller_id = 297
    offer_id = 11
    vin = "1C6SRFFT4MN123456"
    make = "Ram"
    model = "1500"
    trim = "Laramie"
    year = 2023
    offer_amount = 49500.00
    bid_amount = 48900.00
    offer_status = "accepted"
    purchase_status = "completed"
    transport_status = "delivered"
    city = "Chicago"
    state = "IL"
    search_text = "2023 Ram 1500 Laramie pickup truck Chicago Illinois delivered completed"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_11" -Method Post -Body $data11 -ContentType "application/json"
Write-Host "✓ Added Ram 1500" -ForegroundColor Green

# Sample 12 - Hyundai Sonata
$data12 = @{
    carrier_id = 222
    transport_id = 12
    purchase_id = 12
    buyer_id = 624
    seller_id = 813
    offer_id = 12
    vin = "5NPE24AF3MH123456"
    make = "Hyundai"
    model = "Sonata"
    trim = "SEL Plus"
    year = 2023
    offer_amount = 27800.00
    bid_amount = 27200.00
    offer_status = "pending"
    purchase_status = "pending"
    transport_status = "not_scheduled"
    city = "Portland"
    state = "OR"
    search_text = "2023 Hyundai Sonata SEL Plus sedan Portland Oregon pending offer"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_12" -Method Post -Body $data12 -ContentType "application/json"
Write-Host "✓ Added Hyundai Sonata" -ForegroundColor Green

# Sample 13 - Subaru Outback
$data13 = @{
    carrier_id = 333
    transport_id = 13
    purchase_id = 13
    buyer_id = 918
    seller_id = 426
    offer_id = 13
    vin = "4S4BTACC5M3123456"
    make = "Subaru"
    model = "Outback"
    trim = "Limited XT"
    year = 2024
    offer_amount = 39200.00
    bid_amount = 38600.00
    offer_status = "accepted"
    purchase_status = "in_progress"
    transport_status = "in_transit"
    city = "Salt Lake City"
    state = "UT"
    search_text = "2024 Subaru Outback Limited XT wagon AWD Salt Lake City Utah shipping"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_13" -Method Post -Body $data13 -ContentType "application/json"
Write-Host "✓ Added Subaru Outback" -ForegroundColor Green

# Sample 14 - Lexus RX
$data14 = @{
    carrier_id = 444
    transport_id = 14
    purchase_id = 14
    buyer_id = 537
    seller_id = 642
    offer_id = 14
    vin = "2T2HZMDA5MC123456"
    make = "Lexus"
    model = "RX"
    trim = "350 F Sport"
    year = 2023
    offer_amount = 52800.00
    bid_amount = 52100.00
    offer_status = "accepted"
    purchase_status = "completed"
    transport_status = "delivered"
    city = "Phoenix"
    state = "AZ"
    search_text = "2023 Lexus RX 350 F Sport luxury SUV Phoenix Arizona delivered"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_14" -Method Post -Body $data14 -ContentType "application/json"
Write-Host "✓ Added Lexus RX" -ForegroundColor Green

# Sample 15 - Mazda CX-5
$data15 = @{
    carrier_id = 555
    transport_id = 15
    purchase_id = 15
    buyer_id = 246
    seller_id = 891
    offer_id = 15
    vin = "JM3KFBDM5M0123456"
    make = "Mazda"
    model = "CX-5"
    trim = "Grand Touring"
    year = 2023
    offer_amount = 32500.00
    bid_amount = 31900.00
    offer_status = "accepted"
    purchase_status = "confirmed"
    transport_status = "scheduled"
    city = "Las Vegas"
    state = "NV"
    search_text = "2023 Mazda CX-5 Grand Touring SUV Las Vegas Nevada transport scheduled"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_15" -Method Post -Body $data15 -ContentType "application/json"
Write-Host "✓ Added Mazda CX-5" -ForegroundColor Green

# Sample 16 - Volkswagen Jetta
$data16 = @{
    carrier_id = 666
    transport_id = 16
    purchase_id = 16
    buyer_id = 372
    seller_id = 684
    offer_id = 16
    vin = "3VWC57BU8MM123456"
    make = "Volkswagen"
    model = "Jetta"
    trim = "SEL Premium"
    year = 2022
    offer_amount = 26800.00
    bid_amount = 26100.00
    offer_status = "rejected"
    purchase_status = "cancelled"
    transport_status = "not_scheduled"
    city = "Philadelphia"
    state = "PA"
    search_text = "2022 Volkswagen Jetta SEL Premium sedan Philadelphia Pennsylvania cancelled"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_16" -Method Post -Body $data16 -ContentType "application/json"
Write-Host "✓ Added Volkswagen Jetta" -ForegroundColor Green

# Sample 17 - Kia Telluride
$data17 = @{
    carrier_id = 777
    transport_id = 17
    purchase_id = 17
    buyer_id = 819
    seller_id = 273
    offer_id = 17
    vin = "5XYP5DHC0MG123456"
    make = "Kia"
    model = "Telluride"
    trim = "SX Prestige"
    year = 2024
    offer_amount = 48900.00
    bid_amount = 48200.00
    offer_status = "accepted"
    purchase_status = "in_progress"
    transport_status = "in_transit"
    city = "Minneapolis"
    state = "MN"
    search_text = "2024 Kia Telluride SX Prestige SUV Minneapolis Minnesota shipping in transit"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_17" -Method Post -Body $data17 -ContentType "application/json"
Write-Host "✓ Added Kia Telluride" -ForegroundColor Green

# Sample 18 - GMC Sierra
$data18 = @{
    carrier_id = 888
    transport_id = 18
    purchase_id = 18
    buyer_id = 495
    seller_id = 138
    offer_id = 18
    vin = "1GTU9EED5MZ123456"
    make = "GMC"
    model = "Sierra"
    trim = "Denali"
    year = 2023
    offer_amount = 61500.00
    bid_amount = 60800.00
    offer_status = "accepted"
    purchase_status = "completed"
    transport_status = "delivered"
    city = "Nashville"
    state = "TN"
    search_text = "2023 GMC Sierra Denali luxury pickup truck Nashville Tennessee delivered"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_18" -Method Post -Body $data18 -ContentType "application/json"
Write-Host "✓ Added GMC Sierra" -ForegroundColor Green

# Sample 19 - Acura MDX
$data19 = @{
    carrier_id = 999
    transport_id = 19
    purchase_id = 19
    buyer_id = 561
    seller_id = 927
    offer_id = 19
    vin = "5J8YD4H86ML123456"
    make = "Acura"
    model = "MDX"
    trim = "Type S"
    year = 2024
    offer_amount = 68900.00
    bid_amount = 68200.00
    offer_status = "accepted"
    purchase_status = "confirmed"
    transport_status = "scheduled"
    city = "San Diego"
    state = "CA"
    search_text = "2024 Acura MDX Type S luxury SUV performance San Diego California scheduled"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_19" -Method Post -Body $data19 -ContentType "application/json"
Write-Host "✓ Added Acura MDX" -ForegroundColor Green

# Sample 20 - Dodge Challenger
$data20 = @{
    carrier_id = 123
    transport_id = 20
    purchase_id = 20
    buyer_id = 684
    seller_id = 315
    offer_id = 20
    vin = "2C3CDZC98MH123456"
    make = "Dodge"
    model = "Challenger"
    trim = "R/T Scat Pack"
    year = 2023
    offer_amount = 46800.00
    bid_amount = 46100.00
    offer_status = "pending"
    purchase_status = "pending"
    transport_status = "not_scheduled"
    city = "Charlotte"
    state = "NC"
    search_text = "2023 Dodge Challenger R/T Scat Pack muscle car Charlotte North Carolina pending"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_20" -Method Post -Body $data20 -ContentType "application/json"
Write-Host "✓ Added Dodge Challenger" -ForegroundColor Green

# Sample 21 - Porsche Cayenne
$data21 = @{
    carrier_id = 234
    transport_id = 21
    purchase_id = 21
    buyer_id = 729
    seller_id = 486
    offer_id = 21
    vin = "WP1AB2A59MLA12345"
    make = "Porsche"
    model = "Cayenne"
    trim = "S"
    year = 2024
    offer_amount = 89900.00
    bid_amount = 89200.00
    offer_status = "accepted"
    purchase_status = "completed"
    transport_status = "delivered"
    city = "Austin"
    state = "TX"
    search_text = "2024 Porsche Cayenne S luxury performance SUV Austin Texas delivered completed"
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseUri/transport_21" -Method Post -Body $data21 -ContentType "application/json"
Write-Host "✓ Added Porsche Cayenne" -ForegroundColor Green

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "✅ Successfully added 20 sample records!" -ForegroundColor Green
Write-Host "========================================`n" -ForegroundColor Cyan

# Verify count
Write-Host "Verifying data..." -ForegroundColor Yellow
$count = Invoke-RestMethod -Uri "http://localhost:9200/automotive_search/_count" -Method Get
Write-Host "Total documents in index: $($count.count)" -ForegroundColor Green