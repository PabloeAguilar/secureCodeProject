# SecureCodeProject

This project is a web application consisting of a frontend (Blazor) and a backend (ASP.NET Core Web API) with a SQLite database. It includes authentication, user roles, and automated tests.

## Project Structure

- `frontend/`: Blazor client application.
- `webApi/`: ASP.NET Core RESTful API.
- `webApi.Tests/`: Unit and integration tests for the API.
- `database.sql`: Database script (if applicable).

## Requirements

- .NET 9.0 SDK or higher
- Visual Studio 2022 or higher (optional)

## Setup and Execution

1. **Clone the repository**
2. **Restore dependencies:**
   ```powershell
   dotnet restore
   ```
3. **Apply migrations and create the database:**
   ```powershell
   cd webApi
   dotnet ef database update
   ```
4. **Run the backend:**
   ```powershell
   dotnet run --project webApi/webApi.csproj
   ```
5. **Run the frontend:**
   ```powershell
   dotnet run --project frontend/frontend.csproj
   ```

## Tests

To run automated tests:
```powershell
cd webApi.Tests
dotnet test
```

## Folder Structure

- `frontend/Components/`: Blazor UI components
- `frontend/Services/`: Authentication and state services
- `webApi/Controlllers/`: API controllers
- `webApi/Models/`: Data models
- `webApi/Data/`: Database context
- `webApi/Migrations/`: Entity Framework migrations
- `webApi.Tests/UnitTests/`: Unit tests
- `webApi.Tests/IntegrationTests/`: Integration tests

## Security Notes

- The project implements best practices to prevent SQL injection and other common risks.
- It is recommended to review and update dependencies regularly.

## License

This project is for academic use and may be modified as needed by the user.
