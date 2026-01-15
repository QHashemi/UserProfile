#!/bin/bash

# Exit immediately if a command fails
set -e

echo "Starting Docker Compose..."


# Apply EF Core migrations
echo "Applying pending migrations..."
dotnet ef database update --project UserProfile/UserProfile.csproj --startup-project UserProfile/UserProfile.csproj


# 1. Build the containers
docker-compose build

# 2. Start the database in detached mode first
echo "Starting database container..."
docker-compose up -d db

# 3. Wait for the database to be healthy
echo "Waiting for database to become healthy..."
until docker inspect --format='{{.State.Health.Status}}' userprofile-db | grep -q "healthy"; do
    echo "Database is not ready yet. Waiting 5 seconds..."
    sleep 5
done

echo "Database is healthy."

# 4. Run EF Core migrations inside the API container
echo "Applying pending EF Core migrations..."
docker-compose run --rm api dotnet ef database update --project UserProfile/UserProfile.csproj --startup-project UserProfile/UserProfile.csproj

# 5. Start the API container
echo "Starting API container..."
docker-compose up -d api

echo "All services are up and running!"
