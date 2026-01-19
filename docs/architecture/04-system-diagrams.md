# System Diagrams

This document contains comprehensive system diagrams illustrating the architecture of the offnd.at backend system.

## Component Diagram

The Component diagram shows the internal structure of the offnd.at backend system:

```mermaid
C4Component
    title Component Diagram - offnd.at Backend

    Container_Boundary(api, "API Service") {
        Component(endpoints, "API Endpoints", "ASP.NET Core Minimal API", "Handles HTTP requests")
        Component(middleware, "Middleware Pipeline", "ASP.NET Core", "Authentication, rate limiting, exception handling")
    }
    
    Container_Boundary(worker, "Events Worker") {
        Component(consumers, "Event Consumers", "MassTransit", "Processes integration events")
    }
    
    Container_Boundary(migrator, "Migration Runner") {
        Component(migrations, "Migration Orchestrator", "EF Core", "Applies database migrations")
    }
    
    Container_Boundary(application, "Application Layer") {
        Component(commands, "Command Handlers", "MediatR", "Executes write operations")
        Component(queries, "Query Handlers", "MediatR", "Executes read operations")
        Component(domainEventHandlers, "Domain Event Handlers", "MediatR", "Reacts to domain events")
        Component(validators, "Validators", "FluentValidation", "Validates requests")
    }
    
    Container_Boundary(domain, "Domain Layer") {
        Component(entities, "Entities & Aggregates", "C#", "Link aggregate root")
        Component(valueObjects, "Value Objects", "C#", "Phrase, Url, etc.")
        Component(domainEvents, "Domain Events", "C#", "LinkCreated, LinkVisited")
        Component(repoInterfaces, "Repository Interfaces", "C#", "ILinkRepository")
    }
    
    Container_Boundary(infrastructure, "Infrastructure Layer") {
        Component(messaging, "Messaging", "MassTransit", "RabbitMQ integration")
        Component(caching, "Caching", "IMemoryCache", "In-memory cache")
        Component(telemetry, "Telemetry", "OpenTelemetry", "Metrics, traces, logs")
        Component(resilience, "Resilience", "Polly", "Retry policies")
        Component(phraseGen, "Phrase Generator", "Custom", "Generates profane phrases")
    }
    
    Container_Boundary(persistence, "Persistence Layer") {
        Component(dbContext, "DbContext", "EF Core", "Database context")
        Component(repositories, "Repositories", "EF Core", "Data access implementations")
        Component(configurations, "Entity Configurations", "EF Core", "Entity mappings")
    }
    
    ContainerDb(db, "PostgreSQL", "Database", "Stores links")
    ContainerQueue(mq, "RabbitMQ", "Message Broker", "Event queue")
    
    Rel(endpoints, middleware, "Uses")
    Rel(endpoints, commands, "Sends commands")
    Rel(endpoints, queries, "Sends queries")
    
    Rel(consumers, domainEventHandlers, "Triggers")
    
    Rel(migrations, dbContext, "Uses")
    
    Rel(commands, validators, "Validates with")
    Rel(commands, entities, "Creates/Updates")
    Rel(commands, repoInterfaces, "Uses")
    Rel(commands, phraseGen, "Uses")
    
    Rel(queries, repoInterfaces, "Uses")
    
    Rel(domainEventHandlers, messaging, "Publishes to")
    
    Rel(entities, domainEvents, "Raises")
    Rel(entities, valueObjects, "Contains")
    
    Rel(repositories, dbContext, "Uses")
    Rel(repositories, repoInterfaces, "Implements")
    
    Rel(dbContext, db, "Reads/Writes", "TCP")
    Rel(messaging, mq, "Publishes/Consumes", "AMQP")
    
    UpdateLayoutConfig($c4ShapeInRow="3", $c4BoundaryInRow="1")
```

## Request Flow Diagrams

### Generate Link Flow

```mermaid
sequenceDiagram
    actor User
    participant API as API Endpoint
    participant MW as Middleware
    participant MR as MediatR
    participant Val as Validator
    participant Handler as Command Handler
    participant PG as Phrase Generator
    participant Repo as Repository
    participant DB as PostgreSQL
    participant Domain as Link Aggregate
    participant EventPub as Event Publisher
    participant MQ as RabbitMQ

    User->>API: POST /api/v1/links
    API->>MW: Process request
    MW->>MW: Rate limiting
    MW->>MW: Authentication
    API->>MR: Send GenerateLinkCommand
    MR->>Val: Validate command
    
    alt Validation fails
        Val-->>API: Validation error
        API-->>User: 400 Bad Request
    end
    
    MR->>Handler: Route to handler
    Handler->>Handler: Validate URL
    Handler->>Handler: Validate Language/Theme/Format
    
    loop Retry until unique phrase
        Handler->>PG: Generate phrase
        PG-->>Handler: Phrase
        Handler->>Repo: Check if phrase exists
        Repo->>DB: Query by phrase
        DB-->>Repo: Result
        Repo-->>Handler: Exists?
    end
    
    Handler->>Domain: Create Link aggregate
    Domain->>Domain: Raise LinkCreatedDomainEvent
    Handler->>Repo: Insert link
    Repo->>DB: Save changes
    DB->>EventPub: Trigger domain event handlers
    EventPub->>MQ: Publish LinkCreatedIntegrationEvent
    DB-->>Handler: Success
    Handler-->>MR: Result<GenerateLinkResponse>
    MR-->>API: Response
    API-->>User: 201 Created
```

### Redirect Flow

```mermaid
sequenceDiagram
    actor User
    participant API as API Endpoint
    participant Cache as Link Visit Cache
    participant MR as MediatR
    participant Handler as Query Handler
    participant Repo as Repository
    participant DB as PostgreSQL
    participant Domain as Link Aggregate
    participant EventPub as Event Publisher
    participant MQ as RabbitMQ

    User->>API: GET /{phrase}
    API->>MR: Send GetLinkByPhraseQuery
    MR->>Handler: Route to handler
    Handler->>Repo: Get link by phrase
    Repo->>DB: Query
    DB-->>Repo: Link
    Repo-->>Handler: Maybe<Link>
    
    alt Link not found
        Handler-->>API: Failure result
        API-->>User: 404 Not Found
    end
    
    Handler-->>API: Success result
    API->>Cache: Check if already visited
    
    alt Not visited yet
        API->>MR: Send VisitLinkCommand
        MR->>Domain: RecordVisit()
        Domain->>Domain: Raise LinkVisitedDomainEvent
        Domain->>EventPub: Publish domain event
        EventPub->>MQ: Publish LinkVisitedIntegrationEvent
        API->>Cache: Mark as visited
    end
    
    API-->>User: 302 Redirect to target URL
```

### Background Event Processing Flow

```mermaid
sequenceDiagram
    participant MQ as RabbitMQ
    participant Worker as Events Worker
    participant Consumer as Event Consumer
    participant Handler as Integration Event Handler
    participant Service as Application Service
    participant DB as PostgreSQL

    MQ->>Worker: LinkVisitedIntegrationEvent
    Worker->>Consumer: Consume event
    Consumer->>Handler: Handle event
    Handler->>Service: Process visit
    Service->>DB: Update statistics
    DB-->>Service: Success
    Service-->>Handler: Complete
    Handler-->>Consumer: Success
    Consumer-->>Worker: Acknowledge
    Worker-->>MQ: ACK
    
    Note over MQ,Worker: If processing fails, message is retried
```

## Deployment Architecture

```mermaid
graph TB
    subgraph "Docker Compose - Local Development"
        API[API Service<br/>:8090]
        Worker[Events Worker]
        Migrator[Migration Runner]
        
        subgraph "Infrastructure"
            PG[(PostgreSQL<br/>:5432)]
            RMQ[RabbitMQ<br/>:5672, :15672]
            OTEL[OpenTelemetry<br/>Collector]
        end
    end
    
    subgraph "External Services"
        GitHub[GitHub API]
    end
    
    API --> PG
    API --> RMQ
    API --> OTEL
    API --> GitHub
    
    Worker --> RMQ
    Worker --> PG
    Worker --> OTEL
    
    Migrator --> PG
    
    style API fill:#4CAF50
    style Worker fill:#2196F3
    style Migrator fill:#FF9800
    style PG fill:#336791
    style RMQ fill:#FF6600
```

## Data Flow Diagram

```mermaid
flowchart LR
    User[User Request]
    
    subgraph API Layer
        Endpoint[API Endpoint]
        Auth[Authentication]
        RateLimit[Rate Limiter]
    end
    
    subgraph Application Layer
        Command[Command/Query]
        Validation[Validation]
        Handler[Handler]
    end
    
    subgraph Domain Layer
        Aggregate[Link Aggregate]
        DomainEvent[Domain Event]
    end
    
    subgraph Infrastructure
        Cache[(Cache)]
        DB[(Database)]
        Queue[(Message Queue)]
    end
    
    User --> Endpoint
    Endpoint --> Auth
    Auth --> RateLimit
    RateLimit --> Command
    Command --> Validation
    Validation --> Handler
    Handler --> Aggregate
    Handler --> Cache
    Aggregate --> DomainEvent
    Handler --> DB
    DomainEvent --> Queue
    
    style Aggregate fill:#4CAF50
    style Handler fill:#2196F3
    style DB fill:#336791
    style Queue fill:#FF6600
```

## Layer Dependencies

```mermaid
graph TD
    API[API Service]
    Worker[Events Worker]
    Migrator[Migration Runner]
    
    App[Application Layer]
    Domain[Domain Layer]
    Infra[Infrastructure Layer]
    Persist[Persistence Layer]
    Contracts[Contracts]
    
    API --> App
    API --> Contracts
    Worker --> App
    Migrator --> Persist
    
    App --> Domain
    App --> Contracts
    
    Infra --> App
    Infra --> Domain
    
    Persist --> App
    Persist --> Domain
    
    style Domain fill:#4CAF50
    style App fill:#2196F3
    style Infra fill:#FF9800
    style Persist fill:#FF9800
    
    classDef service fill:#9C27B0,stroke:#333,stroke-width:2px
    class API,Worker,Migrator service
```

## Technology Stack Diagram

```mermaid
mindmap
  root((offnd.at<br/>Backend))
    Runtime
      .NET 10
      C# 13
    Data
      PostgreSQL 17
      EF Core 10
      In-Memory Cache
    Messaging
      RabbitMQ 4
      MassTransit
    Patterns
      CQRS - MediatR
      DDD
      Result Pattern
      Repository Pattern
    Infrastructure
      OpenTelemetry
      Serilog
      Polly
      FluentValidation
    API
      ASP.NET Core
      Minimal API
      Scalar OpenAPI
    Testing
      NUnit
      Testcontainers
      Bogus
      FluentAssertions
```

## Observability Architecture

```mermaid
graph LR
    subgraph Application
        API[API Service]
        Worker[Events Worker]
    end
    
    subgraph Instrumentation
        Metrics[Metrics<br/>Counters, Gauges]
        Traces[Distributed Traces<br/>Activities]
        Logs[Structured Logs<br/>Serilog]
    end
    
    subgraph Collection
        OTEL[OpenTelemetry<br/>Collector]
    end
    
    subgraph Backends
        Prometheus[Prometheus<br/>Metrics]
        Jaeger[Jaeger<br/>Traces]
        Loki[Loki<br/>Logs]
    end
    
    API --> Metrics
    API --> Traces
    API --> Logs
    
    Worker --> Metrics
    Worker --> Traces
    Worker --> Logs
    
    Metrics --> OTEL
    Traces --> OTEL
    Logs --> OTEL
    
    OTEL --> Prometheus
    OTEL --> Jaeger
    OTEL --> Loki
    
    style OTEL fill:#4CAF50
    style API fill:#2196F3
    style Worker fill:#2196F3
```

## Event-Driven Architecture Flow

```mermaid
graph TB
    subgraph "Domain Layer"
        Aggregate[Link Aggregate]
        DomainEvent[Domain Event<br/>LinkCreatedDomainEvent]
    end
    
    subgraph "Application Layer"
        DomainHandler[Domain Event Handler]
        IntegrationEvent[Integration Event<br/>LinkCreatedIntegrationEvent]
    end
    
    subgraph "Infrastructure"
        MassTransit[MassTransit Publisher]
        RabbitMQ[(RabbitMQ)]
    end
    
    subgraph "Background Processing"
        Consumer[Event Consumer]
        EventHandler[Integration Event Handler]
    end
    
    Aggregate -->|Raises| DomainEvent
    DomainEvent -->|Triggers| DomainHandler
    DomainHandler -->|Creates| IntegrationEvent
    IntegrationEvent -->|Publishes via| MassTransit
    MassTransit -->|Sends to| RabbitMQ
    RabbitMQ -->|Delivers to| Consumer
    Consumer -->|Invokes| EventHandler
    
    style DomainEvent fill:#4CAF50
    style IntegrationEvent fill:#2196F3
    style RabbitMQ fill:#FF6600
```

## Caching Strategy

```mermaid
graph TD
    Request[Incoming Request]
    Cache{Cache Hit?}
    Memory[(In-Memory Cache)]
    DB[(PostgreSQL)]
    Response[Response]
    
    Request --> Cache
    Cache -->|Yes| Response
    Cache -->|No| DB
    DB --> Memory
    Memory --> Response
    
    style Cache fill:#4CAF50
    style Memory fill:#2196F3
    style DB fill:#336791
```

## Diagram Legend

### Component Types

| Symbol | Meaning |
|--------|---------|
| 🟢 Green | Domain/Core Layer |
| 🔵 Blue | Application Layer |
| 🟠 Orange | Infrastructure/Persistence |
| 🟣 Purple | Services/Entry Points |
| 🔴 Red | External Systems |

### Relationship Types

| Arrow | Meaning |
|-------|---------|
| `-->` | Dependency/Uses |
| `==>` | Data Flow |
| `-.->` | Async/Event |
| `-->>` | Return/Response |
