# VkBank
An ASP.NET Web API project that showcases the implementation of advanced architectural patterns and technologies for building scalable and maintainable web applications.

---

## Onion Architecture
The project follows the Onion Architecture pattern, organizing code into layers based on their responsibilities, with clear separation of concerns and dependency inversion.

### Onion Architecture Overview
- Core
  - Domain
  - Application
    - Dependencies:
      - Project References:
        - Domain
      - NuGet Packages:
        - AutoMapper
        - FluentValidation
        - FluentValidation.DependencyInjectionExtensions
        - MediatR
        - Microsoft.Data.SqlClient
        - Microsoft.Extensions.Configuration
- Infrastructure
  - Infrastructure
    - Dependencies:
      - Project References:
        - Domain
      - NuGet Packages:
        - Newtonsoft.Json
        - Microsoft.Extensions.Logging
        - Microsoft.Extensions.DependencyInjection.Abstractions
        - Microsoft.Extensions.Caching.Memory
        - Microsoft.Extensions.Caching.Abstractions
        - StackExchange.Redis
        - Microsoft.Extensions.Configuration
  - Persistence
    - Dependencies:
      - Project References:
        - Domain
        - Application
      - NuGet Packages:
        - Dapper
        - Microsoft.Data.SqlClient
        - Microsoft.Extensions.Configuration
- Presentation
  - Web Api
    - Dependencies:
      - Project References:
        - Domain
        - Application
        - Infrastructure
        - Persistence
      - NuGet Packages:
        - MediatR
        - Microsoft.Extensions.DependencyInjection
        - Swashbuckle.AspNetCore

---

### Application
The Application layer is responsible for the application's commands and queries, handlers using the CQRS Pattern and MediatR pattern, and validators using FluentValidation. It also includes interfaces for Dapper and repositories, and AutoMapper for mapping command requests with entities.

### Domain
The Domain layer contains the entities, constants, and result classes.

### Infrastructure
The Infrastructure layer contains the necessary infrastructure for the application, including services for logging with `ILogger`, serialization with `JsonConvert` and caching using either `IMemoryCache` or `IConnectionMultiplexer` (i.e., Redis). The Redis server is configured to run on a WSL2 Ubuntu distribution at port 6379.

### Persistence
The Persistence layer manages the database connection and database methods. It uses Dapper as an ORM and includes both synchronous and asynchronous methods (also with cancellation token support). It can execute commands directly from raw SQL or by calling stored procedures.

### Api
The API layer handles requests using `IMediator` and utilizes caching with `IRedisCacheService`.

---

## Technologies Used

* Onion Architecture
* CQRS Pattern, MediatR Pattern
* FluentValidation
* AutoMapper
* Dapper ORM, Microsoft SQL Server, Stored Procedures and Triggers
* MemoryCache, Redis, Logger, JsonConvert
* ASP.NET Core Web API

---

## CQRS Pattern (Command Query Responsibility Segregation)
Implements the CQRS pattern, separating the responsibilities of commands (write operations) and queries (read operations) to optimize performance and scalability.

---

## MediatR Pattern
Utilizes the MediatR library for implementing the Mediator pattern, simplifying the communication between components, promoting loose coupling, and reducing dependency injection.

---

## Dapper ORM
Integrates Dapper as the Object-Relational Mapping (ORM) tool for database access, providing high performance and flexibility while working with relational databases, and utilizes stored procedures for optimized database interactions.

---

## Stored Procedures and Triggers
Leverages stored procedures and triggers in the database layer for implementing complex business logic, improving security, and optimizing database performance.

---

## Caching
Utilizes `IMemoryCache` for in-memory caching and `IConnectionMultiplexer` (i.e., Redis) for distributed caching.
The Redis server is configured to run on a WSL2 Ubuntu distribution at port 6379.

---

## Logging
Implements logging using `ILogger` to provide a robust and flexible logging infrastructure, helping to log data effectively.

---

## Serialization
Uses `JsonConvert` for serialization and deserialization of objects to and from JSON, ensuring data is easily readable and transferrable.
