# UserProfile API

This project is an ASP.NET Core API for managing users, posts, and related data. It uses **Entity Framework Core** with support for **SQL Server** and **PostgreSQL**, implements **JWT authentication**, **role-based access**, and includes **middleware for security, logging, and error handling**. The API is fully containerizable with Docker and supports health checks, rate-limiting, and credential stuffing protection.

---

## Features

* **Database Setup**

  * Uses Entity Framework Core for SQL Server or PostgreSQL.
  * Supports migrations and automatic database updates.
  * One-to-one, one-to-many, and many-to-many relationships supported.

* **Authentication & Authorization**

  * JWT-based authentication with access tokens and refresh tokens.
  * Role-based access control for secure endpoints.
  * Credential stuffing protection via login attempt tracking.

* **Controllers & DTOs**

  * AuthController: Register and login users.
  * UserController: CRUD operations for users and role management.
  * HealthController: /health, /ready, /live endpoints.
  * DTOs separate input/output data from EF entities.

* **Middleware**

  * Error handling middleware for global exception management.
  * Logging middleware with Serilog for console and file logging.
  * Security headers middleware to protect against common attacks.
  * IP rate-limiting middleware to prevent abuse.
  * Block suspicious IPs middleware.

* **Health Checks**

  * Custom health checks to monitor application and database status.
  * Configurable via `HealthCheckConfiguration.cs`.

* **Docker Support**

  * Can run API in Docker with SQL Server or PostgreSQL containers.
  * Supports `.env` files for environment configuration.
  * Docker Compose setup for multi-container deployment.

* **Unit Testing**

  * Uses xUnit and Moq for testing controllers, services, and middleware.
  * Supports in-memory databases for isolated testing.

* **File Uploads**

  * Supports user profile image uploads.
  * Ensures safe storage and updates user entity with file path.

* **Additional Features**

  * CORS configuration.
  * Config folder for separate app configurations.
  * Scalable startup architecture with dependency injection and service registration.

---

## Quick Setup

### SQL Server

1. Create `Entities` folder and add entities (`User.cs`, `Post.cs`, `UserDetail.cs`).
2. Create `Data` folder and add `AppDbContext.cs`.
3. Create database in SQL Server Management Studio.
4. Add connection string in `appsettings.json`.
5. Register DbContext in `Program.cs`.
6. Run migrations in Package Manager Console:

   * Add-Migration InitialCreate -Context UserProfile.Data.AppDbContext
   * Update-Database -Context UserProfile.Data.AppDbContext

### PostgreSQL

1. Install package: `Npgsql.EntityFrameworkCore.PostgreSQL`.
2. Add connection string in `appsettings.json`.
3. Register DbContext and migration methods in `Program.cs`.
4. Run migrations:

   * Add-Migration InitialCreate -Context UserProfile.Data.AppDbContext
   * Update-Database -Context UserProfile.Data.AppDbContext

### JWT Authentication

1. Configure JWT settings in `appsettings.json`.
2. Add JWT middleware in `Program.cs`.
3. Secure endpoints with `[Authorize]` and roles where needed.

### Refresh Tokens

1. Add refresh token properties to user entity.
2. Implement generate, save, and validate refresh token methods.
3. Create refresh token endpoint to issue new access tokens.

### Middleware & Logging

1. Error handling middleware for global exception management.
2. Logging with Serilog to console and file.
3. Request logging middleware for auditing requests.
4. Security headers middleware.
5. Rate limiting and IP blocking middleware.

### Unit Testing

1. Use xUnit and Moq.
2. Create in-memory database for testing.
3. Test controllers, services, and endpoints.

### Docker

1. Create Dockerfile and docker-compose.yml.
2. Deploy API and database containers.
3. Use `.env.docker` for configuration.
