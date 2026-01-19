# offnd-at/backend-core

[![Build Status](https://img.shields.io/github/actions/workflow/status/offnd-at/backend-core/build-and-publish.yml?branch=master)](https://github.com/offnd-at/backend-core/actions)
[![GitHub Release](https://img.shields.io/github/v/release/offnd-at/backend-core)](https://github.com/offnd-at/backend-core/releases)

A robust, good-enough-performance monolithic backend for [offnd.at](https://offnd.at) - a profanity-first URL shortener that generates memorable (and offensive) phrases instead of random strings. Built with **Clean Architecture** and **Domain-Driven Design (DDD)**.

**Live Demo**
- https://offnd.at

**API Reference**
- https://api.offnd.at/docs

## Technology Stack

| Category | Technology |
| :--- | :--- |
| **Language** | C# 13 / .NET 10 |
| **Database** | PostgreSQL 17 / EF Core 10 |
| **Messaging** | MassTransit with RabbitMQ 4 |
| **Patterns** | CQRS (MediatR), Result Pattern, DDD |
| **Validation** | FluentValidation |
| **Testing** | NUnit, Bogus, FluentAssertions, Testcontainers |
| **Observability** | OpenTelemetry, Serilog |

## Architecture

The project follows the principles of Clean Architecture and DDD to ensure maintainability and scalability. For more details on the architecture, there is a [docs](./docs/) folder - full disclosure, it is AI-generated, so it is likely hilariously overcomplicated and verbose. But there is knowledge in there.

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker & Docker Compose](https://www.docker.com/products/docker-desktop)

### Quick Start

1. **Spin up local infrastructure**:
   ```powershell
   docker-compose up -d
   ```

2. **Explore the API**:
   Open `http://localhost:8090/docs/` to see the Scalar API reference.

## Contributing

Contributions are welcome! Feel free to create an issue or submit a pull request. You can also reach out to the project author at [contact@offnd.at](mailto:contact@offnd.at) with any questions or suggestions. 

If you'd like to help translate the short link profanity into your language, please check [offnd-at/vocabularies](https://github.com/offnd-at/vocabularies)!

---

Built with 🤬 by [ghawliczek](https://github.com/ghawliczek).
