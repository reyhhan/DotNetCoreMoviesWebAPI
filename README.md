# MoviesCore API

A comprehensive .NET 8 Web API for managing movies with user authentication, ratings, and advanced features like caching, API versioning, and health checks.

## 🏗️ Architecture

This project follows Clean Architecture principles with the following structure:

- **Movies.Api** - Web API layer with minimal API endpoints
- **Movies.Application** - Business logic, services, and data access
- **Movies.Contract** - DTOs and contracts for API communication
- **Identity.Api** - Separate identity service for JWT token generation
- **Movies.Api.Sdk** - SDK for consuming the Movies API
- **Movies.Api.Sdk.Consumer** - Example consumer application

## 🚀 Technologies Used

### Core Framework
- **.NET 8** - .NET version
- **ASP.NET Core Web API** - Web API framework
- **Minimal APIs** - Lightweight endpoint definitions

### Database & Data Access
- **PostgreSQL** - Primary database
- **Dapper** - Lightweight ORM for data access
- **Npgsql** - PostgreSQL .NET driver

### Authentication & Authorization
- **JWT Bearer Authentication** - Token-based authentication
- **Custom Authorization Policies** - Admin and trusted member policies
- **API Key Authentication** - Additional security layer

### API Features
- **API Versioning** - Multiple API versions support (v1.0, v2.0)
- **Swagger/OpenAPI** - API documentation and testing
- **Output Caching** - Performance optimization
- **Health Checks** - Application health monitoring
- **FluentValidation** - Request validation

### Development & Deployment
- **Docker** - Containerization support
- **User Secrets** - Secure configuration management
- **Environment-based Configuration** - Different settings per environment

## 📋 Features

### Movie Management
- ✅ Create, read, update, and delete movies
- ✅ Movie search and filtering by title, year, genre
- ✅ Pagination support
- ✅ Slug-based movie URLs (e.g., "the-matrix-1999")
- ✅ Movie metadata (title, year, genres)

### Rating System
- ✅ User movie ratings (1-5 stars)
- ✅ Average rating calculations
- ✅ User-specific rating retrieval
- ✅ Rating management (create/delete)

### Security & Authentication
- ✅ JWT-based authentication
- ✅ Role-based authorization (Admin, Trusted Member)
- ✅ API key authentication for admin operations
- ✅ User context in requests

### Performance & Reliability
- ✅ Output caching with configurable policies
- ✅ Database health checks
- ✅ Request/response validation
- ✅ Error handling middleware

## 🛠️ Setup Instructions

### Prerequisites
- .NET 8 SDK
- PostgreSQL database
- Docker (optional)

### Database Setup

1. **Using Docker (Recommended)**:
   ```bash
   cd Movies.Application
   docker-compose up -d
   ```

2. **Manual PostgreSQL Setup**:
   - Install PostgreSQL
   - Create a database for the application
   - Update connection string in configuration

### Configuration

1. **Set up User Secrets** (for development):
   ```bash
   cd Movies.Api
   dotnet user-secrets init
   dotnet user-secrets set "Database:ConnectionString" "Host=localhost;Port=5433;Database=movies;Username=your_user;Password=your_password"
   ```

2. **Environment Variables**:
   ```bash
   export POSTGRES_USER=your_user
   export POSTGRES_PASSWORD=your_password
   export POSTGRES_DB=movies
   ```

### Running the Application

1. **Start the Identity API**:
   ```bash
   cd Identity.Api
   dotnet run
   ```

2. **Start the Movies API**:
   ```bash
   cd Movies.Api
   dotnet run
   ```

3. **Access the API**:
   - Movies API: `https://localhost:7001`
   - Identity API: `https://localhost:7002`
   - Swagger UI: `https://localhost:7001/swagger`

## 📚 API Endpoints

### Authentication
```http
POST /token - Generate JWT token (Identity API)
```

### Movies
```http
GET    /api/movies              - Get all movies (paginated, filterable)
GET    /api/movies/{idOrSlug}   - Get movie by ID or slug
POST   /api/movies              - Create movie (Admin only)
PUT    /api/movies/{id}         - Update movie (Admin only)
DELETE /api/movies/{id}         - Delete movie (Admin only)
```

### Ratings
```http
POST   /api/movies/{id}/ratings - Rate a movie
DELETE /api/movies/{id}/ratings - Delete movie rating
GET    /api/ratings/me          - Get user's ratings
```

### Health & Monitoring
```http
GET /_health - Health check endpoint
```

## 🔧 API Usage Examples

### 1. Get Authentication Token
```bash
curl -X POST "https://localhost:7002/token" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@movies.com",
    "customClaims": {
      "admin": "true",
      "trusted_member": "true"
    }
  }'
```

### 2. Get All Movies
```bash
curl -X GET "https://localhost:7001/api/movies?page=1&pageSize=10&title=matrix" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

### 3. Create a Movie (Admin only)
```bash
curl -X POST "https://localhost:7001/api/movies" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "The Matrix",
    "yearOfRelease": 1999,
    "genres": ["Action", "Sci-Fi"]
  }'
```

### 4. Rate a Movie
```bash
curl -X POST "https://localhost:7001/api/movies/{movieId}/ratings" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "rating": 5
  }'
```

## 🏃‍♂️ Development

### Project Structure
```
MoviesCore/
├── Movies.Api/                 # Web API layer
│   ├── Endpoints/             # Minimal API endpoints
│   ├── Auth/                  # Authentication & authorization
│   ├── Mapping/               # Request/response mapping
│   └── Swagger/               # API documentation
├── Movies.Application/         # Business logic layer
│   ├── Services/              # Business services
│   ├── Repositories/          # Data access
│   ├── Models/                # Domain models
│   └── Validators/            # Input validation
├── Movies.Contract/           # API contracts
│   ├── Requests/              # Request DTOs
│   └── Responses/             # Response DTOs
├── Identity.Api/              # Identity service
├── Movies.Api.Sdk/            # Client SDK
└── Movies.Api.Sdk.Consumer/   # SDK usage example
```

### Key Design Patterns
- **Repository Pattern** - Data access abstraction
- **Service Layer** - Business logic encapsulation
- **Dependency Injection** - Loose coupling
- **Options Pattern** - Configuration management
- **Minimal APIs** - Lightweight endpoint definitions

### Running Tests
```bash
dotnet test
```

### Building for Production
```bash
dotnet publish -c Release
```

## 🔒 Security Considerations

- JWT tokens are used for authentication
- API keys provide additional security for admin operations
- User secrets are used for sensitive configuration in development
- Environment variables should be used for production secrets
- HTTPS is enforced in production

## 📈 Performance Features

- **Output Caching**: Configurable caching policies for movie endpoints
- **Database Connection Pooling**: Efficient database connections
- **Async/Await**: Non-blocking operations throughout
- **Pagination**: Efficient data retrieval for large datasets

## 🏥 Monitoring & Health

- Health checks for database connectivity
- Structured logging with different log levels
- API versioning for backward compatibility
- Swagger documentation for API exploration

## 📄 Acknowledgement

This project is part of a Dometrain course and is for educational purposes.

<img width="597" height="751" alt="image" src="https://github.com/user-attachments/assets/965aee7c-294d-44e0-a909-661062409ab7" />
