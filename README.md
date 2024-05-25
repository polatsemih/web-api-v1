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

### Core
The Core includes the Application and Domain layers, which contain the business logic and domain entities respectively.

#### Application
The Application layer is responsible for the application's commands and queries, handlers, and validators.

#### Domain
The Domain layer contains the core business entities, constants, and result classes.

### Infrastructure
The Infrastructure includes the Infrastructure and Persistence layers, which handle the application's external services integration and data access.

#### Infrastructure
The Infrastructure layer contains the necessary infrastructure for the application, including service registrations and configuration.

#### Persistence
The Persistence layer manages the database connection and database methods calling stored procedures using Dapper.


### Presentation
The Presentation includes the Api layer, which serves as the entry point for the application, handling HTTP requests and responses using MediatR.

#### Api

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

## Technologies Used

* ASP.NET Core Web API
* Onion Architecture
* CQRS Pattern
* MediatR Pattern
* Dapper ORM
* FluentValidation
* AutoMapper
* Microsoft SQL Server
* Stored Procedures and Triggers

---

## Onion Architecture Overview
- Core
  - Domain
  - Application
    - Dependencies:
      - Domain
      - AutoMapper
      - FluentValidation
      - FluentValidation.DependencyInjectionExtensions
      - MediatR
      - Microsoft.Data.SqlClient
      - Microsoft.Extensions.Configuration
- Infrastructure
  - Infrastructure
  - Persistence
    - Dependencies:
      - Domain
      - Application
      - Dapper
      - Microsoft.Data.SqlClient
      - Microsoft.Extensions.Configuration
- Presentation
  - Web Api
    - Dependencies:
      - Application
      - Persistence
      - MediatR
      - Microsoft.Extensions.DependencyInjection
      - Swashbuckle.AspNetCore
