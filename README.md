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

## Run with Docker

```bash
docker compose up --build
```

- Frontend: `http://localhost:4200`
- API: `http://localhost:5080`

## Notes

- The backend is scaffolded for .NET 10 and EF Core 10.
- Angular 22 is the currently supported latest major release family.
- Add migrations before relying on the database schema in production.
