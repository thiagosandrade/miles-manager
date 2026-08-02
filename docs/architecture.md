# Architecture

This repository starts with a simple layered backend:

- Domain: entities and core business concepts
- Application: CQRS commands, queries, and contracts
- Infrastructure: EF Core and PostgreSQL persistence
- API: HTTP endpoints and composition root

The frontend is a standalone Angular shell intended for feature modules and route-based pages.
