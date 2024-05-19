# VkBank
An ASP.NET Web API project that showcases the implementation of advanced architectural patterns and technologies for building scalable and maintainable web applications.

---

## Onion Architecture
The project follows the Onion Architecture pattern, organizing code into layers based on their responsibilities, with clear separation of concerns and dependency inversion.

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
* Microsoft SQL Server
* Stored Procedures and Triggers

---

## Onion Architecture Overview

- Core
  - Domain
  - Application
    - Dependencies:
      - Domain
      - 
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
      - 
      - Dapper
      - Microsoft.Data.SqlClient
      - Microsoft.Extensions.Configuration
- Presentation
  - Web Api
    - Dependencies:
      - Application
      - Persistence
      - 
      - MediatR
      - Microsoft.Extensions.DependencyInjection
      - Swashbuckle.AspNetCore
