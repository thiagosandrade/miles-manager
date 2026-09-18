# MilesManager

MilesManager is a starter monorepo for mileage tracking with:

- Backend: ASP.NET Core, EF Core, PostgreSQL, CQRS
- Frontend: Angular 22
- Containers: Docker and Docker Compose

## Layout

```text
MilesManager/
├── backend/
├── frontend/
├── docker/
├── docs/
├── .github/
│   └── workflows/
├── local-build-and-push.ps1
├── docker-compose.yml
├── .editorconfig
├── .gitignore
└── README.md
```

## Backend

- `backend/MilesManager.sln`
- `backend/src/MilesManager.Api`
- `backend/src/MilesManager.Application`
- `backend/src/MilesManager.Domain`
- `backend/src/MilesManager.Infrastructure`
- `backend/tests/MilesManager.Tests`

## Frontend

Angular workspace files live under `frontend/`.

## Running Locally

There are two ways to run MilesManager locally.

### Run with Docker

Use Docker Compose when you want to run the application using containers:

```bash
docker compose up --build
```

- Frontend: `http://localhost:4200`
- API: `http://localhost:5080`

### Run Frontend Locally

To run the Angular frontend directly on your machine:

```bash
cd frontend
npm run start
```

### Run Backend from Visual Studio

The backend can be run directly from Visual Studio using the backend solution:

```text
backend/MilesManager.sln
```

## Google Cloud Deployment

The `local-build-and-push.ps1` script is intended **only for deploying to Google Cloud**.

Run it from the repository root:

```powershell
.\local-build-and-push.ps1
```

The script:

1. Builds the backend Docker image.
2. Builds the frontend Docker image.
3. Pushes both images to Google Artifact Registry.
4. Connects to the Google Cloud VM.
5. Pulls the latest images and recreates the application containers.

This script is **not required for local development**. For local development, use Docker Compose, `npm run start` for the frontend, or run the backend from Visual Studio as described above.

## Notes

- The backend is scaffolded for .NET 10 and EF Core 10.
- Angular 22 is the currently supported latest major release family.
- Add migrations before relying on the database schema in production.
