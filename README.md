# URL Shortener

A REST API that converts long URLs into short, shareable links. Built with .NET 9 and Apache Cassandra.

## Tech Stack

- **.NET 9** — Web API
- **Apache Cassandra** — Primary database (NoSQL)
- **Redis** — Caching
- **MediatR** — CQRS pattern
- **FluentValidation** — Input validation
- **Docker Compose** — Container orchestration

## Architecture

Clean Architecture with 5 layers:
- **Domain** — Entities
- **Application** — CQRS commands/queries, interfaces, DTOs
- **Infrastructure** — Redis cache, Base62 service, background worker
- **Persistence** — Cassandra repositories
- **Api** — Controllers, middleware

## Features

- Create short URLs with optional custom alias
- Temporary URLs with automatic expiration (Cassandra TTL)
- Redirect to original URL (302)
- Click tracking and analytics
- Redis caching for fast redirects
- Background service for expired URL cleanup

## Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | /api/urls | Create short URL |
| GET | /api/urls/{shortCode} | Get URL details |
| PUT | /api/urls/{shortCode} | Update URL |
| DELETE | /api/urls/{shortCode} | Delete URL |
| GET | /{shortCode} | Redirect to original URL |

## Setup

**Prerequisites:** Docker Desktop, .NET 9 SDK

**1. Clone the repo**
```bash
git clone https://github.com/Lashaia/UrlShortener.git
cd UrlShortener
```

**2. Start Cassandra and Redis**
```bash
docker-compose up -d
```

**3. Run the API**
```bash
cd src/UrlShortener.Api
dotnet run
```

**4. Open Scalar UI**
```
http://localhost:5116/scalar/v1
```

## Database

Cassandra runs on port `9042`. To inspect data:
```bash
docker exec -it cassandra cqlsh
USE url_shortener;
SELECT * FROM urls;
```
