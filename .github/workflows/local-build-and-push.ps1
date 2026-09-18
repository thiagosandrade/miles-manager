$ErrorActionPreference = "Stop"

# ---------------------------------------------------------
# Configuration
# ---------------------------------------------------------

$Project = "impactful-shard-137923"
$Zone = "us-central1-a"
$Instance = "instance-milesmanager"

$Registry = "us-central1-docker.pkg.dev"
$Repository = "miles-manager-containers"

$BackendImage = "$Registry/$Project/$Repository/backend:latest"
$FrontendImage = "$Registry/$Project/$Repository/frontend:latest"

$AppDir = "/home/thiagosoares_andrade/miles-manager"

# ---------------------------------------------------------
# Helper
# ---------------------------------------------------------

function Run-Step {
    param (
        [string]$Message,
        [scriptblock]$Command
    )

    Write-Host ""
    Write-Host "==================================================" -ForegroundColor DarkGray
    Write-Host $Message -ForegroundColor Cyan
    Write-Host "==================================================" -ForegroundColor DarkGray

    & $Command

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Step failed: $Message"
        exit $LASTEXITCODE
    }
}

# ---------------------------------------------------------
# Check we're at the repository root
# ---------------------------------------------------------

if (-not (Test-Path ".\backend")) {
    Write-Error "backend folder not found. Run this script from C:\GIT\miles-manager."
    exit 1
}

if (-not (Test-Path ".\frontend")) {
    Write-Error "frontend folder not found. Run this script from C:\GIT\miles-manager."
    exit 1
}

# ---------------------------------------------------------
# Authenticate Docker with Google Artifact Registry
# ---------------------------------------------------------

Run-Step "Configuring Docker authentication for Artifact Registry" {
    gcloud auth configure-docker $Registry --quiet
}

# ---------------------------------------------------------
# Build backend
# ---------------------------------------------------------

Run-Step "Building backend Docker image" {
    docker build `
        -f .\backend\src\MilesManager.Api\Dockerfile `
        -t $BackendImage `
        .\backend
}

# ---------------------------------------------------------
# Build frontend
# ---------------------------------------------------------

Run-Step "Building frontend Docker image" {
    docker build `
        -t $FrontendImage `
        .\frontend
}

# ---------------------------------------------------------
# Push backend
# ---------------------------------------------------------

Run-Step "Pushing backend image to Artifact Registry" {
    docker push $BackendImage
}

# ---------------------------------------------------------
# Push frontend
# ---------------------------------------------------------

Run-Step "Pushing frontend image to Artifact Registry" {
    docker push $FrontendImage
}

# ---------------------------------------------------------
# Deploy VM
# ---------------------------------------------------------

Run-Step "Updating Google Cloud VM" {
    gcloud compute ssh $Instance `
        --zone $Zone `
        --project $Project `
        --command "cd $AppDir && git pull --ff-only origin main && docker compose -f docker-compose.vm.yml pull && docker compose -f docker-compose.vm.yml up -d --force-recreate --remove-orphans && docker compose -f docker-compose.vm.yml ps"
}

# ---------------------------------------------------------
# Done
# ---------------------------------------------------------

Write-Host ""
Write-Host "==================================================" -ForegroundColor Green
Write-Host "Deployment completed successfully!" -ForegroundColor Green
Write-Host "==================================================" -ForegroundColor Green