#!/bin/bash

# Script to start MSSQL Server Express in Docker and run EF Core migrations

set -e  # Exit on error

echo "=========================================="
echo "Starting MSSQL Server Express in Docker"
echo "=========================================="

# Check if .env file exists
if [ ! -f .env ]; then
    echo "ERROR: .env file not found!"
    echo "Please create a .env file based on .env.example"
    echo "Example: cp .env.example .env"
    exit 1
fi

# Load environment variables from .env
set -a  # automatically export all variables
source .env
set +a  # stop automatically exporting

# Check if MSSQL_SA_PASSWORD is set
if [ -z "$MSSQL_SA_PASSWORD" ]; then
    echo "ERROR: MSSQL_SA_PASSWORD not set in .env file!"
    exit 1
fi

# Check if CONNECTION_STRING is set
if [ -z "$CONNECTION_STRING" ]; then
    echo "ERROR: CONNECTION_STRING not set in .env file!"
    exit 1
fi

# Start Docker containers
echo ""
echo "Starting Docker containers..."
docker-compose up -d

# Wait for SQL Server to be ready
echo ""
echo "Waiting for SQL Server to be ready..."
sleep 10

# Check if SQL Server is ready by checking the logs
MAX_RETRIES=30
RETRY_COUNT=0
until docker logs mssql-express 2>&1 | grep -q "Recovery is complete"; do
    RETRY_COUNT=$((RETRY_COUNT + 1))
    if [ $RETRY_COUNT -ge $MAX_RETRIES ]; then
        echo "ERROR: SQL Server failed to start after $MAX_RETRIES attempts"
        docker logs mssql-express --tail 20
        exit 1
    fi
    echo "Waiting for SQL Server to be ready... (Attempt $RETRY_COUNT/$MAX_RETRIES)"
    sleep 2
done

echo ""
echo "SQL Server is ready!"

# Drop and recreate the database
echo ""
echo "=========================================="
echo "Dropping and Recreating Database"
echo "=========================================="

cd api

echo "Dropping existing database if it exists..."
dotnet ef database drop --force

# Run EF Core migrations
echo ""
echo "=========================================="
echo "Running EF Core Migrations"
echo "=========================================="

# Check if migrations directory exists
if [ ! -d "Migrations" ]; then
    echo "No migrations found. Creating initial migration..."
    dotnet ef migrations add InitialCreate
fi

# Apply migrations to the database
echo ""
echo "Applying migrations to database..."
dotnet ef database update

echo ""
echo "=========================================="
echo "Database setup complete!"
echo "=========================================="
echo ""
echo "MSSQL Server is running on localhost:1433"
echo "Connection string available in .env file"
echo ""
echo "To stop the database, run:"
echo "  docker-compose down"
echo ""
