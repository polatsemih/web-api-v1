# SUPBank
An ASP.NET Web API project that showcases the implementation of advanced architectural patterns and technologies for building scalable and maintainable web applications.

---

## Onion Architecture
The project follows the Onion Architecture pattern, organizing code into layers based on their responsibilities, with clear separation of concerns and dependency inversion.

### Onion Architecture Overview
- Core
  - Application
    - Dependencies:
      - Projects:
        - Domain
      - Packages:
        - AutoMapper
        - FluentValidation
        - FluentValidation.DependencyInjectionExtensions
        - MediatR
        - Microsoft.Data.SqlClient
        - Microsoft.Extensions.DependencyInjection.Abstractions
  - Domain
- Infrastructure
  - Infrastructure
    - Dependencies:
      - Projects:
        - Application
        - Domain
      - Packages:
        - Microsoft.Extensions.DependencyInjection.Abstractions
        - Microsoft.Extensions.Caching.Memory
        - Microsoft.Extensions.Caching.Abstractions
        - Microsoft.Extensions.Configuration
        - Microsoft.Extensions.Logging
        - Serilog
        - Serilog.Sinks.File
        - StackExchange.Redis
        - Newtonsoft.Json
  - Persistence
    - Dependencies:
      - Projects:
        - Application
        - Domain
        - Infrastructure
      - Packages:
        - Dapper
        - Microsoft.Data.SqlClient
        - Microsoft.Extensions.Configuration
- Presentation
  - Api
    - Dependencies:
      - Projects:
        - Application
        - Domain
        - Infrastructure
        - Persistence
      - Packages:
        - Asp.Versioning.Mvc
        - Asp.Versioning.Mvc.ApiExplorer
        - Swashbuckle.AspNetCore
        - Microsoft.Extensions.DependencyInjection
        - MediatR
        - Serilog.AspNetCore
        - Serilog.Sinks.File
- Test
  - UnitTests.xUnit
    - Dependencies:
      - Projects:
        - Application
        - Domain
        - Infrastructure
        - Persistence
       - Packages:
         - AutoMapper
         - FluentAssertions
         - MediatR
         - Microsoft.NET.Test.Sdk
         - Moq
         - xunit
         - xunit.runner.visualstudio
---

### Application
The Application Layer serves as the intermediary between the Presentation Layer and the Core Domain in the Onion Architecture. It encapsulates the application-specific logic, including the handling of commands and queries following the `CQRS Pattern` and `MediatR pattern`, as well as the validation of inputs using `FluentValidation`. Additionally, it provides interfaces for data access using `Dapper`, repositories and infrastructure services, and facilitates object-to-object mapping through the use of `AutoMapper`.

### Domain
The domain layer, also known as the business layer, is a central part of the application architecture that encapsulates the core business logic and rules. It defines the fundamental concepts, behaviors, and entities that model the business domain. The domain layer typically includes entities, which represent the essential business objects and their relationships; constants, which define immutable values relevant to the domain; and response classes, which encapsulate the results or outcomes of domain operations.

### Infrastructure
The Infrastructure layer contains the necessary infrastructure for the application, including services for logging with `Serilog`, serialization with `Newtonsoft.Json` and caching using either `Microsoft.Extensions.Caching.Memory` or `StackExchange.Redis`. The `Redis server` is configured to run on a WSL2 Ubuntu distribution at port 6379.

### Persistence
The Persistence layer manages the database connection and database methods. It uses `Dapper` as an ORM and includes both synchronous and asynchronous methods (also with cancellation token support). It can execute commands directly from `raw SQL` or by calling `stored procedures`.

### Api
The API layer handles requests using `MediatR` and utilizes caching with `StackExchange.Redis`.

### UnitTests.xUnit
The unit test layer, utilizing `xUnit`, `FluentAssertions`, and `Moq`, comprehensively covers all scenarios within the controller methods.

---

## Technologies Used

* Onion Architecture
* CQRS Pattern, MediatR Pattern
* FluentValidation
* AutoMapper
* Dapper ORM, Microsoft SQL Server, Stored Procedures and Triggers
* MemoryCache, Redis
* Serilog
* Newtonsoft.Json
* ASP.NET Core Web API with versioning system
* Unit Tests
* Git

---

## CQRS Pattern (Command Query Responsibility Segregation)
Implements the CQRS pattern, separating the responsibilities of commands (write operations) and queries (read operations) to optimize performance and scalability.

---

## MediatR Pattern
Utilizes the MediatR library for implementing the Mediator pattern, simplifying the communication between components, promoting loose coupling, and reducing dependency injection.

---

## FluentValidation
Separate the validation logic from the business logic and integrate it into the application layer. This allows to define reusable validator classes.

---

## AutoMapper
Query requests or command requests are often validated for correctness and sanitized for security before being used to create or update entities. Mapping provides an opportunity to perform these operations as part of the mapping process, ensuring that only valid and sanitized data is used to modify the entity state.

---

## Dapper ORM
Integrates Dapper as the Object-Relational Mapping (ORM) tool for database access, providing high performance and flexibility while working with relational databases, and utilizes stored procedures for optimized database interactions.

---

## Stored Procedures and Triggers
Leverages stored procedures and triggers in the database layer for implementing complex business logic, improving security, and optimizing database performance.

---

## Caching
Utilizes `Microsoft.Extensions.Caching.Memory` for in-memory caching and `StackExchange.Redis` for distributed caching.
The `Redis server` is configured to run on a WSL2 Ubuntu distribution at port 6379.

---

## Logging
Implements logging using `Serilog` to provide a robust and flexible logging infrastructure, helping to log data effectively.

---

## Serialization
Uses `Newtonsoft.Json` for serialization and deserialization of objects to and from JSON, ensuring data is easily readable and transferrable.

---

## UnitTests
Unit tests, employing `xUnit`, `FluentAssertions`, `Moq`, provide a structured approach, including data initialization (Arrange), action execution (Act), and outcome verification (Assert), facilitating the verification of code correctness, reliability, and maintainability, thereby enhancing code quality, expediting development cycles, and bolstering confidence in software stability.
