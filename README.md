# WorkShift

WorkShift is a backend-focused shift management system built with ASP.NET Core Web API
using Clean Architecture principles.

The project focuses on real-world business rules rather than simple CRUD operations.

## Features
- Department and employee management
- Shift scheduling with business rules:
  - Shift conflict prevention
  - Minimum rest time enforcement
  - Weekly working hour limits
- Role-based authentication (Admin / Staff) using JWT
- Staff can view only their own shifts
- Soft delete and deactivate logic
- Global exception handling
- FluentValidation for request validation

## Tech Stack
- ASP.NET Core (.NET 8)
- Entity Framework Core
- SQL Server (LocalDB)
- JWT Authentication
- FluentValidation
- Clean Architecture

## Business Rules
- No overlapping shifts for the same employee
- Minimum 12 hours rest between shifts
- Maximum weekly working hours (default: 52.5)
- Day / Night shift rotation supported

## Getting Started
1. Clone the repository
2. Update `appsettings.json` with your connection string and JWT settings
3. Run migrations
4. Start the API


