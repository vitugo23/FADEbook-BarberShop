# FADEbook - The Night Owls Barber Shop Management System

A full-stack barber shop management and appointment booking system built collaboratively.
The application streamlines the booking process for customers and barbers with Google Calendar integration for seamless scheduling.

## Features

- **Customer Management** - Registration, profile management, appointment history
- **Barber Management** - Barber profiles, availability, service offerings
- **Appointment Booking** - Real-time appointment scheduling and management
- **Google Calendar Integration** - Automatic calendar sync for barbers and customers
- **Service Management** - Define and manage barber services and pricing
- **Availability Tracking** - Real-time barber availability updates
- **Responsive Design** - Works seamlessly on desktop and mobile devices

## Tech Stack

**Backend:**
- ASP.NET Core Web API (.NET 9)
- Entity Framework Core with Code-First Migrations
- SQL Server (Dockerized)
- Repository Pattern for data access
- Service Layer Architecture for business logic
- RESTful API design principles
- Google Calendar API integration
- xUnit for testing

**Frontend:**
- React 19
- Modern JavaScript/ES6+
- Responsive UI components

**DevOps & Infrastructure:**
- Docker & Docker Compose
- SQL Server in Docker container
- CI/CD automation scripts
- Environment-based configuration

## Team - The Night Owls

This project was developed collaboratively by a team during Revature training. Each member contributed to different aspects of the application.

## Team Members
* Christian Brewer
* Victor Torres
* Charles Trangay
* Dean Gelbaum
* Jeremiah Ogembo
* Muhiddin Kurbonov

**Victor Torres - Backend Developer & System Architect**

## Contributions (Victor Torres)

As a backend developer, I was responsible for the entire backend architecture and implementation.

### **System Architecture & Design**
- Designed and implemented clean architecture with 11 distinct layers
- Established Repository Pattern for data abstraction
- Created Service Layer to encapsulate business logic
- Developed custom Middleware for cross-cutting concerns
- Implemented comprehensive Exception handling strategy
- Organized modular project structure for maintainability

### **Core Backend Development**

#### **API Layer** (`/Controllers`)
Built RESTful API controllers for:
- **CustomerController** - Customer registration, profile management, appointment history
- **BarberController** - Barber profiles, availability management
- **AppointmentController** - Complete CRUD for appointment booking
- **ServiceController** - Barber service management (haircuts, styling, etc.)
- **GoogleCalendarController** - OAuth integration and calendar synchronization

#### **Business Logic Layer** (`/Services`)
Implemented service classes including:
- **CustomerAppointmentService** - Appointment business rules and validation
- **BarberService** - Barber availability logic
- **AuthenticationService** - User authentication and JWT management
- **CalendarIntegrationService** - Google Calendar API interactions

#### **Data Access Layer** (`/Repositories`)
Created repository implementations:
- Generic Repository pattern for code reusability
- CustomerRepository, BarberRepository, AppointmentRepository
- Async/await operations for optimal performance
- Complex LINQ queries for efficient data retrieval

#### **Database Design** (`/Models` & `/DB`)
- Designed normalized database schema:
  - **Customer** - User information and contact details
  - **Barber** - Barber profiles and credentials
  - **Appointment** - Booking records with status tracking
  - **Service** - Service offerings and pricing
  - **Availability** - Barber scheduling information
- Implemented Entity Framework Core DbContext
- Created Code-First migrations for version control
- Established proper relationships and foreign key constraints

#### **DTOs** (`/DTO`)
Created Data Transfer Objects for:
- Customer registration and login requests
- Appointment booking requests/responses
- Barber profile updates
- Service management
- Validation attributes for data integrity

#### **Middleware & Exception Handling**
- Custom middleware for request/response logging
- Global exception handler for consistent error responses
- Validation middleware for input sanitization
- Custom exception types for domain-specific errors

#### **Google Calendar Integration**
- Implemented OAuth 2.0 flow for Google Calendar
- Automated appointment sync to Google Calendar
- Token management and refresh logic
- Calendar event creation/update/deletion

### **Database & Migrations**
- Designed database schema with proper normalization
- Created 20+ EF Core migrations for schema evolution
- Implemented database seeding for development/testing
- Configured connection string management via environment variables

### **Security Implementation**
- Password hashing with secure algorithms
- Password complexity requirements (8+ chars, mixed case, numbers, special chars)
- JWT token-based authentication
- Secure credential storage using environment variables
- OAuth 2.0 implementation for Google integration

## Project Structure
```
FADEbook/
├── api/                          # Backend API (.NET 9)
│   ├── Controllers/             # API endpoints
│   │   ├── CustomerController.cs
│   │   ├── BarberController.cs
│   │   ├── AppointmentController.cs
│   │   ├── ServiceController.cs
│   │   └── GoogleCalendarController.cs
│   ├── Models/                  # Domain models
│   │   ├── CustomerModel.cs
│   │   ├── BarberModel.cs
│   │   ├── AppointmentModel.cs
│   │   └── ServiceModel.cs
│   ├── DTO/                     # Data Transfer Objects
│   ├── Services/                # Business logic layer
│   ├── Repositories/            # Data access layer
│   ├── DB/                      # DbContext & configurations
│   ├── Migrations/              # EF Core migrations
│   ├── Middleware/              # Custom middleware
│   ├── Exceptions/              # Exception handling
│   ├── Mapping/                 # Object mapping
│   ├── Program.cs              # Application startup
│   └── .env.example            # Environment template
├── Api.Tests/                   # Unit & integration tests
├── fadebook-frontend/           # React frontend
├── Diagrams/                    # Architecture diagrams
├── docker-compose.yml          # Docker orchestration
├── start-db.sh                 # Database startup script
├── cicd.sh                     # CI/CD automation
└── README.md
```

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- SQL Server or Docker
- Google Cloud Console account (for Calendar API)

## Running Locally

### Option 1: Using Docker (Recommended)
```bash
# 1. Create .env file from example
cp api/.env.example api/.env

# 2. Update .env with your configuration:
# - Set MSSQL_SA_PASSWORD
# - Add Google OAuth credentials (optional for basic testing)

# 3. Start all services
docker-compose up -d

# Services will be available at:
# API: http://localhost:5288
# Frontend: http://localhost:3000
# SQL Server: localhost:1433
```

### Option 2: Manual Setup

**1. Start SQL Server Database:**
```bash
# Using the provided script
./start-db.sh

# Or manually with Docker
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword!1" \
   -p 1433:1433 --name sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

**2. Configure Environment:**
```bash
cd api
cp .env.example .env
```

**3. Run Backend:**
```bash
cd api

# Restore dependencies
dotnet restore

# Apply database migrations
dotnet ef database update

# Run API
dotnet run
```

API available at `http://localhost:5288`

**4. Run Frontend:**
```bash
cd fadebook-frontend

# Install dependencies
npm install

# Start development server
npm start
```

Frontend available at `http://localhost:3000`

## API Documentation

Access interactive API documentation at:
- **Swagger UI:** `http://localhost:5288/swagger`

### Main API Endpoints

#### **Customer Management**
- `POST /api/customers/register` - Register new customer
- `POST /api/customers/login` - Customer authentication
- `GET /api/customers/{id}` - Get customer profile
- `PUT /api/customers/{id}` - Update customer profile
- `GET /api/customers/{id}/appointments` - Get customer's appointments

#### **Barber Management**
- `GET /api/barbers` - Get all barbers
- `GET /api/barbers/{id}` - Get barber details
- `GET /api/barbers/{id}/availability` - Check barber availability
- `POST /api/barbers` - Create barber profile (admin)
- `PUT /api/barbers/{id}` - Update barber profile

#### **Appointments**
- `GET /api/appointments` - Get all appointments
- `GET /api/appointments/{id}` - Get appointment details
- `POST /api/appointments` - Book new appointment
- `PUT /api/appointments/{id}` - Update appointment
- `DELETE /api/appointments/{id}` - Cancel appointment
- `GET /api/appointments/available-slots` - Get available time slots

#### **Services**
- `GET /api/services` - Get all services
- `GET /api/services/{id}` - Get service details
- `POST /api/services` - Create service (admin)
- `PUT /api/services/{id}` - Update service
- `DELETE /api/services/{id}` - Remove service

#### **Google Calendar Integration**
- `GET /api/calendar/auth` - Initiate OAuth flow
- `POST /api/calendar/sync/{appointmentId}` - Sync appointment to calendar
- `DELETE /api/calendar/remove/{appointmentId}` - Remove from calendar

## Running Tests
```bash
cd Api.Tests
dotnet test --verbosity normal

# With coverage report
dotnet test /p:CollectCoverage=true /p:CoverageReportFormat=opencover
```

## Architecture Highlights

### **Clean Architecture**
- Clear separation of concerns across 11 layers
- Domain models isolated from infrastructure concerns
- Dependency injection throughout

### **Repository Pattern**
- Abstracted data access layer
- Easy to swap data sources
- Simplified unit testing

### **Service Layer**
- Encapsulated business logic
- Reusable across multiple controllers
- Centralized validation rules

### **DTO Pattern**
- Clean API contracts
- Prevents over-posting vulnerabilities
- Simplified API versioning

## Key Learning Outcomes

### **Backend Development Mastery**
✅ ASP.NET Core Web API proficiency  
✅ Entity Framework Core and LINQ  
✅ Clean Architecture implementation  
✅ RESTful API design patterns  
✅ Async/await and performance optimization  

### **Database Management**
✅ SQL Server and T-SQL  
✅ Code-First migrations  
✅ Complex relationship modeling  
✅ Query optimization  
✅ Transaction management  

### **Integration & APIs**
✅ OAuth 2.0 implementation  
✅ Third-party API integration (Google Calendar)  
✅ JWT authentication  
✅ Token management and refresh  

### **Software Engineering**
✅ Design patterns (Repository, Service, Factory)  
✅ SOLID principles  
✅ Dependency injection  
✅ Unit and integration testing  
✅ Git collaboration in teams  

### **DevOps**
✅ Docker containerization  
✅ Docker Compose orchestration  
✅ Environment configuration  
✅ CI/CD scripting  

## 📸 Screenshots

### API Documentation (Swagger)
![Swagger UI](screenshots/swagger-ui.png)
*Comprehensive API documentation with interactive endpoints*

![API Endpoints](screenshots/api-endpoints.png)
*RESTful endpoints for appointments, customers, and barbers*

### Frontend Application

![Homepage](screenshots/frontend-homepage.png)
*Modern Next.js frontend with responsive design*

![Barber Selection](screenshots/barber-list.png)
*Browse and select from available barbers*

### Database Architecture

![Database Schema](screenshots/database-schema.png)
*Entity Relationship Diagram showing normalized database design*

---

## Technical Highlights

- **Comprehensive appointment booking system** with conflict detection
- **Real-time availability** tracking across multiple barbers
- **Google Calendar integration** for seamless scheduling
- **OAuth 2.0 implementation** for secure third-party authorization
- **11 architectural layers** demonstrating advanced design patterns
- **Async operations throughout** for optimal performance
- **Comprehensive validation** at multiple levels
- **Production-ready error handling** with meaningful user feedback
- **Dockerized deployment** for easy setup and scaling

---

**Developed by The Night Owls Team**

