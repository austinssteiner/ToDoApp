# ToDo App - Full Stack Application 

A full-stack task management application built with .NET Core (backend) and React (frontend). This application demonstrates clean architecture and proper separation of concerns.

## Quick Start

### Prerequisites

- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Node.js 18+** - [Download](https://nodejs.org/)
- **npm** (comes with Node.js)

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/austinssteiner/ToDoApp
   cd ToDoApp
   ```

2. **Install frontend dependencies**
   ```bash
   npm install
   cd todoapp.client
   npm install
   cd ..
   ```

3. **Run the application**

   **Option 1: Using NPM scripts (Recommended)**
   ```bash
   npm run dev
   ```
   This starts both backend and frontend concurrently.

   **Option 2: Manual**
   ```bash
   # Terminal 1 - Backend
   cd ToDoApp.Server
   dotnet run

   # Terminal 2 - Frontend
   cd todoapp.client
   npm run dev
   ```

   **Option 3: Using PowerShell/Batch scripts (Windows)**
   ```powershell
   .\start-all.ps1
   # or
   start-all.bat
   ```

### Default Credentials

On first run, an admin account is automatically created:
- **Username:** `admin`
- **Password:** `admin123`

### Access the Application

Once running:
- **Frontend:** http://localhost:5173
- **Backend API:** http://localhost:5180
- **Swagger UI:** http://localhost:5180/swagger

### Default Credentials

On first run, an admin account is automatically created:
- **Username:** `admin`
- **Password:** `admin123`

## Architecture

### Backend (.NET Core)

- **Framework:** .NET 8.0
- **Database:** SQLite with EF Core Migrations
- **Architecture Pattern:** CQRS using MediatR
- **API Style:** RESTful API with Swagger documentation
- **Error Handling:** Global exception middleware with ProblemDetails
- **Validation:** DataAnnotations on DTOs with ModelState validation

**Key Components:**
- `Controllers/` - Thin controllers that delegate to MediatR handlers
- `Features/` - Organized by feature (Tasks, Subtasks, Users) with CQRS pattern
- `DTOs/` - Data Transfer Objects for API requests/responses
- `Middleware/` - Global exception handling + request/response logging with correlation IDs
- `Data/` - EF Core DbContext and database seeding
- `Migrations/` - Database schema migrations

### Frontend (React)

- **Framework:** React 19 with Vite
- **State Management:** React Query (@tanstack/react-query) for server state
- **Styling:** CSS modules
- **Error Handling:** Error Boundary component

**Key Components:**
- `components/` - Reusable UI components
- `services/` - API service layer
- React Query for data fetching, caching, and synchronization

## Features

- User authentication (login)
- Task management (create, read, update, delete)
- Subtask management
- Task completion tracking
- Real-time data synchronization
- Responsive UI

## Testing

### Running Tests

```bash
cd ToDoApp.Server.Tests
dotnet test
```

The test suite includes integration tests for the Tasks API endpoints, covering:
- Successful task creation
- Validation error handling
- Task retrieval
- Error scenarios (404, etc.)

## API Documentation

### Swagger UI

Once the backend is running, visit http://localhost:5180/swagger for interactive API documentation.

### .http File

See `ToDoApp.Server/ToDoApp.Server.http` for example API requests that can be used with REST Client extensions in VS Code or Rider.

### Rate Limiting & Logging

- Fixed-window rate limiting (60 requests/minute per remote IP with a small queue) returns HTTP 429 when exceeded.
- Request/response logging middleware emits structured logs with correlation IDs for easier tracing in production.

## Database

### Migrations

The application uses EF Core migrations for database schema management:

```bash
# Create a new migration
cd ToDoApp.Server
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update
```

Migrations are automatically applied on application startup.

### Database Location

SQLite database file: `ToDoApp.Server/todoapp.db`

## Assumptions & Design Decisions

### Assumptions

1. **Single-user focus:** The application is designed for individual task management. Multi-user collaboration features are not included.

2. **No persistent authentication:** Login is session-based (stored in React state). No JWT tokens or persistent sessions.

3. **SQLite for simplicity:** SQLite is used for easy setup and portability. For production, consider PostgreSQL or SQL Server.

4. **Development environment:** CORS is configured for localhost development. Production would require proper CORS configuration.

5. **Password security:** Passwords are hashed using BCrypt, but password complexity requirements are minimal (6 characters minimum).

### Trade-offs

1. **In-memory state vs. API calls:**
   - **Chosen:** React Query with API calls
   - **Reason:** Ensures data consistency, enables caching, and provides better error handling

2. **Migrations vs. EnsureCreated:**
   - **Chosen:** EF Core Migrations
   - **Reason:** Reproducible across environments, version-controlled schema changes

3. **Global error handling vs. per-controller:**
   - **Chosen:** Global exception middleware
   - **Reason:** Consistent error responses, cleaner controllers, centralized error handling

4. **React Query vs. useState/useEffect:**
   - **Chosen:** React Query
   - **Reason:** Automatic caching, background refetching, optimistic updates, better UX

5. **DTOs vs. returning entities directly:**
   - **Chosen:** DTOs for all requests/responses
   - **Reason:** Better API contract, prevents over-fetching, allows for transformation

6. **CQRS with MediatR vs. direct service calls:**
   - **Chosen:** CQRS pattern with MediatR
   - **Reason:** Better separation of concerns, easier to test, scalable architecture

## What Would Be Implemented Next

If given more time, the following features would be prioritized:

### High Priority

1. **Authentication & Authorization:**
   - JWT token-based authentication
   - Refresh tokens
   - Role-based access control (RBAC)
   - Password reset functionality

2. **Pagination & Filtering:**
   - Paginated task lists for better performance
   - Filter by status (completed/pending)
   - Sort by date, name, priority
   - Search functionality

3. **Enhanced Task Features:**
   - Task priorities (high, medium, low)
   - Due dates and reminders
   - Task categories/tags
   - Task attachments

4. **Better Error Handling:**
   - Structured logging
   - Request/response logging middleware
   - Error tracking (Sentry, Application Insights)

### Medium Priority

5. **Performance:**
   - Response caching
   - Database query optimization
   - Frontend code splitting
   - Image optimization

6. **Testing:**
   - More comprehensive integration tests
   - Unit tests for handlers
   - Frontend component tests
   - E2E tests

7. **Documentation:**
   - More detailed XML comments
   - API versioning
   - OpenAPI specification enhancements

8. **DevOps:**
   - CI/CD pipeline
   - Docker containerization
   - Environment-specific configurations
   - Health check endpoints

### Low Priority

9. **UI/UX Enhancements:**
   - Dark mode
   - Keyboard shortcuts
   - Drag-and-drop task reordering
   - Bulk operations

10. **Advanced Features:**
    - Task templates
    - Recurring tasks
    - Task sharing/collaboration
    - Export/import functionality

## Scalability Considerations

### Current Limitations

1. **SQLite:** Not ideal for high-concurrency scenarios. Consider PostgreSQL or SQL Server for production since SQLite does not have concurrent writes.
2. **No caching layer:** Consider Redis for distributed caching.
3. **No load balancing:** Single instance deployment.
4. **No rate limiting:** API could be abused without rate limiting.

### Scaling Strategies

1. **Database:** Migrate to PostgreSQL or SQL Server to allow for concurrent reads and writes
2. **Caching:** Implement Redis for frequently accessed data
3. **API:** Add API Gateway with rate limiting
4. **Frontend:** Implement CDN for static assets
5. **Monitoring:** Add Application Performance Monitoring (APM)

## Security Considerations

### Current Implementation

- Password hashing (BCrypt)
- Input validation on all DTOs
- SQL injection protection (EF Core parameterized queries)
- CORS configuration
- Global error handling (prevents information leakage)

### Future Enhancements

- JWT authentication with refresh tokens
- HTTPS enforcement
- Rate limiting
- Input sanitization
- Content Security Policy (CSP) headers
- Security headers (HSTS, X-Frame-Options, etc.)

## Project Structure

```
ToDoApp/
├── ToDoApp.Server/          # Backend API
│   ├── Controllers/         # API controllers
│   ├── Features/            # CQRS handlers organized by feature
│   ├── DTOs/                # Data Transfer Objects
│   ├── Models/              # Domain models
│   ├── Data/                # EF Core DbContext
│   ├── Middleware/          # Custom middleware
│   └── Migrations/          # Database migrations
├── todoapp.client/          # Frontend React app
│   ├── src/
│   │   ├── components/      # React components
│   │   ├── services/        # API service layer
│   │   └── main.jsx         # Application entry point
├── ToDoApp.Server.Tests/    # Integration tests
└── README.md                # This file
```
