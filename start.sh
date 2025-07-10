#!/bin/bash

echo "Starting TrendLens Sales Data Analyzer..."
echo

echo "[1/4] Restoring NuGet packages..."
dotnet restore
if [ $? -ne 0 ]; then
    echo "ERROR: Failed to restore packages"
    exit 1
fi

echo "[2/4] Building solution..."
dotnet build --configuration Release --no-restore
if [ $? -ne 0 ]; then
    echo "ERROR: Build failed"
    exit 1
fi

echo "[3/4] Running tests..."
dotnet test --no-build --configuration Release --verbosity minimal
if [ $? -ne 0 ]; then
    echo "WARNING: Some tests failed"
fi

echo "[4/4] Starting web application..."
echo
echo "TrendLens will be available at:"
echo "- Web UI: https://localhost:5001"
echo "- API: https://localhost:5001/swagger"
echo
echo "Press Ctrl+C to stop the application"
echo

cd src/TrendLens.Web
dotnet run --configuration Release --no-build
