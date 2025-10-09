# Torah Backend API

This is the backend API for the Torah application. It is a .NET Core Web API that provides access to the Torah text, as well as other features like a glossary and dedications.

## Project Structure

The project follows a clean architecture pattern and is divided into four main projects:

- **TorahBackend.WebApi**: The presentation layer, containing the API controllers.
- **TorahBackend.Application**: The application layer, containing the business logic, services, DTOs, and interfaces.
- **TorahBackend.Domain**: The domain layer, containing the core entities and domain logic.
- **TorahBackend.Infrastructure**: The infrastructure layer, responsible for data access (repositories).

## Getting Started

### Prerequisites

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [MongoDB](https://www.mongodb.com/try/download/community)

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/your-username/TorahBackend.git
   ```
2. Navigate to the project directory:
   ```bash
   cd TorahBackend
   ```
3. Restore the dependencies:
   ```bash
   dotnet restore
   ```

### Running the application

1. Make sure your MongoDB server is running.
2. The application will automatically seed the database from the `torah.json` file on startup.
3. To run the application, navigate to the `TorahBackend` directory and use the `dotnet run` command:
   ```bash
   cd TorahBackend
   dotnet run
   ```
The application will start on `http://10.0.1.2:5000`.

## API Endpoints

The API provides the following endpoints:

- **Auth**
  - `POST /api/Auth/login`: Authenticates a user and returns a JWT token.
- **Dedicatoria**
  - `GET /api/Dedicatoria/list`: Gets the list of dedications.
  - `PATCH /api/Dedicatoria/actualizar/contenido`: Updates the content of a dedication (requires authentication).
- **Glosario**
  - `GET /api/Glosario/list`: Gets the list of glossary terms.
  - `PATCH /api/Glosario/actualizar/contenido`: Updates the content of a glossary term (requires authentication).
- **Libro**
  - `GET /api/Libro/list`: Gets the list of books.
  - `GET /api/Libro/get?id={id}`: Gets the details of a specific book.
  - `GET /api/Libro/get/torah`: Gets the entire Torah text.
  - `GET /api/Libro/list/testamento`: Gets the list of testaments.
  - `PATCH /api/Libro/actualizar/nombre`: Updates the name of a book (requires authentication).
  - `PATCH /api/Libro/actualizar/abreviatura`: Updates the abbreviation of a book (requires authentication).
  - `PATCH /api/Libro/actualizar/versiculo`: Updates the text of a verse (requires authentication).
- **VersionControlador**
  - `GET /api/VersionControlador/get`: Gets the version control information.

## Configuration

The application configuration is stored in `appsettings.json`. The main configuration settings are:

- **Jwt**: Contains the JWT secret, issuer, and expiry time.
- **ConnectionStrings**: Contains the connection string for the MongoDB database.

## Dependencies

The project uses the following main dependencies:

- **Microsoft.AspNetCore.Authentication.JwtBearer**: For JWT authentication.
- **MongoDB.Driver**: For MongoDB integration.
- **Swashbuckle.AspNetCore**: For API documentation.
