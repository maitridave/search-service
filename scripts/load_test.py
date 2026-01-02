#!/usr/bin/env python3
"""
Load Testing Script for Search Service
Tests concurrent search requests and measures performance
"""

import time
import statistics
import requests
from concurrent.futures import ThreadPoolExecutor, as_completed

# Configuration
API_BASE_URL = "http://localhost:5000/api/search"
NUM_REQUESTS = 100
CONCURRENCY = 10

# Test queries
TEST_QUERIES = [
    {"query": "Toyota", "userRole": "agent", "userId": "AGENT-001"},
    {"query": "Camry", "userRole": "buyer", "userId": "BUYER-001"},
    {"query": "Ford F-150", "userRole": "seller", "userId": "SELLER-001"},
    {"query": "2023", "userRole": "agent", "userId": "AGENT-001"},
    {"query": "New York", "userRole": "buyer", "userId": "BUYER-002"},
    {"query": "BMW", "userRole": "agent", "userId": "AGENT-001"},
    {"query": "Tesla Model 3", "userRole": "buyer", "userId": "BUYER-003"},
    {"query": "Honda Civic", "userRole": "agent", "userId": "AGENT-001"},
]

def make_search_request(endpoint, query_params):
    """Make a search request and measure response time"""
    start_time = time.time()
    
    try:
        response = requests.get(f"{API_BASE_URL}/{endpoint}", params=query_params, timeout=30)
        elapsed = time.time() - start_time
        
        if response.status_code == 200:
            data = response.json()
            return {
                "success": True,
                "elapsed": elapsed * 1000,  # Convert to milliseconds
                "hits": data.get("totalHits", 0)
            }
        else:
            return {
                "success": False,
                "elapsed": elapsed * 1000,
                "error": f"HTTP {response.status_code}"
            }
    except Exception as e:
        elapsed = time.time() - start_time
        return {
            "success": False,
            "elapsed": elapsed * 1000,
            "error": str(e)
        }

def run_load_test(endpoint, num_requests, concurrency):
    """Run load test with specified concurrency"""
    print(f"\n🔥 Load Testing: {endpoint}")
    print(f"   Requests: {num_requests}")
    print(f"   Concurrency: {concurrency}")
    print("-" * 60)
    
    results = []
    start_time = time.time()
    
    with ThreadPoolExecutor(max_workers=concurrency) as executor:
        futures = []
        
        for i in range(num_requests):
            query = TEST_QUERIES[i % len(TEST_QUERIES)].copy()
            futures.append(executor.submit(make_search_request, endpoint, query))
        
        for future in as_completed(futures):
            result = future.result()
            results.append(result)
            
            status = "✓" if result["success"] else "✗"
            print(f"{status} {len(results)}/{num_requests} - {result['elapsed']:.2f}ms")
    
    total_time = time.time() - start_time
    
    # Calculate statistics
    successful = [r for r in results if r["success"]]
    failed = [r for r in results if not r["success"]]
    
    if successful:
        response_times = [r["elapsed"] for r in successful]
        
        print("\n" + "=" * 60)
        print("📊 Performance Results")
        print("=" * 60)
        print(f"Total Requests:       {num_requests}")
        print(f"Successful:           {len(successful)} ({len(successful)/num_requests*100:.1f}%)")
        print(f"Failed:               {len(failed)} ({len(failed)/num_requests*100:.1f}%)")
        print(f"Total Time:           {total_time:.2f}s")
        print(f"Requests/Second:      {num_requests/total_time:.2f}")
        print("\nResponse Times:")
        print(f"  Min:                {min(response_times):.2f}ms")
        print(f"  Max:                {max(response_times):.2f}ms")
        print(f"  Mean:               {statistics.mean(response_times):.2f}ms")
        print(f"  Median:             {statistics.median(response_times):.2f}ms")
        print(f"  95th Percentile:    {statistics.quantiles(response_times, n=20)[18]:.2f}ms")
        print(f"  99th Percentile:    {statistics.quantiles(response_times, n=100)[98]:.2f}ms")
        print("=" * 60)
    else:
        print("\n❌ All requests failed!")

def test_all_endpoints():
    """Test all search endpoints"""
    print("=" * 60)
    print("Search Service Load Testing")
    print("=" * 60)
    
    endpoints = ["unified", "offers", "purchases", "transports"]
    
    for endpoint in endpoints:
        run_load_test(endpoint, NUM_REQUESTS, CONCURRENCY)
        time.sleep(2)  # Brief pause between tests
    
    print("\n✅ Load testing complete!")

if __name__ == "__main__":
    test_all_endpoints()
