# Fullstack Minimal APIs Demo

A small ASP.NET Core Minimal API built with .NET 10. The project demonstrates a simple product catalog API with CRUD-style operations, in-memory storage, dependency injection, Swagger/OpenAPI, health checks, and consistent error responses.

## Features

- ASP.NET Core Minimal API targeting `.NET 10`
- Product catalog endpoints
- In-memory product storage
- Dependency injection using `IProductService` and `ProductService`
- Swagger/OpenAPI documentation in the Development environment
- HTTPS redirection
- Health-check endpoint
- Problem Details support for unexpected exceptions
- Validation for product names, descriptions, prices, and stock quantities
- Support for full updates with `PUT` and partial updates with `PATCH`

## Technology Stack

- .NET 10
- ASP.NET Core Minimal APIs
- C# with nullable reference types enabled
- Swashbuckle.AspNetCore `10.3.1`
- Microsoft.AspNetCore.OpenApi `10.0.8`

## Project Structure

```text
.
├── Models/
│   ├── CreateProductRequest.cs
│   ├── PatchProductRequest.cs
│   ├── Product.cs
│   └── UpdateProductRequest.cs
├── Services/
│   ├── IProductService.cs
│   └── ProductService.cs
├── Properties/
│   └── launchSettings.json
├── Program.cs
├── Fullstack.MinimalApisDemo.csproj
├── appsettings.json
└── Fullstack.MinimalApisDemo.http
```

### Main Components

- `Program.cs` configures the application, registers services, defines routes, and contains request validation.
- `Models/Product.cs` defines the product returned by the API.
- `Models/CreateProductRequest.cs` defines the payload for creating a product.
- `Models/UpdateProductRequest.cs` defines the payload for replacing a product.
- `Models/PatchProductRequest.cs` defines the optional fields accepted for partial updates.
- `Services/IProductService.cs` defines the product service abstraction.
- `Services/ProductService.cs` stores and manages products in memory.

## Prerequisites

Install the following before running the project:

- .NET 10 SDK
- Visual Studio 2026 or another IDE that supports .NET 10
- PowerShell, Bash, or an equivalent terminal

Verify the installed SDK:

```powershell
dotnet --version
```

## Running the Application

From the project directory, restore dependencies and run the application:

```powershell
dotnet restore
dotnet run
```

The configured development URLs are:

- HTTP: `http://localhost:5205`
- HTTPS: `https://localhost:7177`

When running from Visual Studio, use the `http` or `https` launch profile. The application opens Swagger automatically in the Development environment.

## Swagger / OpenAPI

With the application running in Development, open:

- [Swagger UI](https://localhost:7177/swagger)
- [Swagger UI over HTTP](http://localhost:5205/swagger)

Swagger is enabled only when `ASPNETCORE_ENVIRONMENT` is `Development`.

## API Overview

All product endpoints use the `/api/products` route prefix.

| Method | Route | Description |
|---|---|---|
| `GET` | `/api/health` | Returns the health status of the API |
| `GET` | `/api/products` | Returns all products |
| `GET` | `/api/products/{id}` | Returns one product by ID |
| `POST` | `/api/products` | Creates a product |
| `PUT` | `/api/products/{id}` | Replaces all editable product fields |
| `PATCH` | `/api/products/{id}` | Updates selected product fields |

## Product Model

A product has the following fields:

```json
{
  "id": 1,
  "name": "Mechanical Keyboard",
  "description": "RGB mechanical keyboard",
  "price": 3499,
  "stock": 25,
  "isActive": true
}
```

`price` is represented as a decimal value and `stock` is an integer quantity.

## Endpoint Details

### Health Check

```http
GET /api/health
```

Example response:

```json
{
  "status": "Healthy",
  "message": "DevStore Minimal API is running."
}
```

### Get All Products

```http
GET /api/products
```

Example response:

```json
[
  {
	"id": 1,
	"name": "Mechanical Keyboard",
	"description": "RGB mechanical keyboard",
	"price": 3499,
	"stock": 25,
	"isActive": true
  },
  {
	"id": 2,
	"name": "Wireless Mouse",
	"description": "Ergonomic wireless mouse",
	"price": 1499,
	"stock": 40,
	"isActive": true
  }
]
```

### Get a Product

```http
GET /api/products/{id}
```

Example:

```http
GET /api/products/1
```

If the product does not exist, the API returns `404 Not Found`:

```json
{
  "message": "Product with ID 999 was not found."
}
```

### Create a Product

```http
POST /api/products
Content-Type: application/json
```

Request body:

```json
{
  "name": "4K Monitor",
  "description": "27-inch 4K monitor",
  "price": 29999,
  "stock": 10
}
```

A successful request returns `201 Created` and includes the new resource location in the `Location` header.

The `isActive` property is set to `true` automatically when a product is created.

### Replace a Product

`PUT` replaces all editable product fields and requires every field in the request body.

```http
PUT /api/products/1
Content-Type: application/json
```

Request body:

```json
{
  "name": "Mechanical Keyboard Pro",
  "description": "RGB mechanical keyboard with programmable keys",
  "price": 4999,
  "stock": 20,
  "isActive": true
}
```

The endpoint returns `200 OK` with the updated product or `404 Not Found` when the product does not exist.

### Partially Update a Product

`PATCH` updates only the fields supplied in the request body.

```http
PATCH /api/products/1
Content-Type: application/json
```

Request body:

```json
{
  "price": 4299,
  "stock": 18
}
```

Supported patch fields are:

- `name`
- `description`
- `price`
- `stock`
- `isActive`

At least one field must be provided. An empty object returns `400 Bad Request`.

## Validation Rules

The API validates create and replace requests using the following rules:

- `name` is required and cannot be empty or whitespace.
- `name` cannot exceed 100 characters.
- `description` is required for `POST` and `PUT` requests.
- `description` cannot exceed 500 characters.
- `price` must be greater than zero.
- `stock` cannot be negative.

For `PATCH` requests, only explicitly supplied fields are validated. A supplied `name` cannot be empty, and supplied `description`, `price`, and `stock` values must follow the same limits.

Validation errors return `400 Bad Request` with a response similar to:

```json
{
  "message": "Price must be greater than zero."
}
```

## Error Handling

- `200 OK` — successful read or update
- `201 Created` — successful product creation
- `400 Bad Request` — invalid input or an empty patch request
- `404 Not Found` — requested product does not exist
- `500 Internal Server Error` — unexpected errors are handled through ASP.NET Core Problem Details middleware

## Data Storage

Products are stored in a process-local `List<Product>` inside `ProductService`.

This means:

- Data is lost when the application stops or restarts.
- The service is registered as a singleton for the lifetime of the process.
- The API is intended for demonstration and learning, not production persistence.
- There is no database, authentication, authorization, pagination, filtering, or concurrency handling implemented.

The service contains a `Delete` method, but no HTTP `DELETE` endpoint is currently mapped in `Program.cs`.

## Testing the API with PowerShell

Start the application first, then run:

```powershell
Invoke-RestMethod -Uri "http://localhost:5205/api/health" -Method Get

Invoke-RestMethod -Uri "http://localhost:5205/api/products" -Method Get

$body = @{
	name = "4K Monitor"
	description = "27-inch 4K monitor"
	price = 29999
	stock = 10
} | ConvertTo-Json

Invoke-RestMethod `
	-Uri "http://localhost:5205/api/products" `
	-Method Post `
	-ContentType "application/json" `
	-Body $body
```

For HTTPS in a local development environment, the .NET development certificate may need to be trusted:

```powershell
dotnet dev-certs https --trust
```

## Building the Project

Build the project with:

```powershell
dotnet build
```

## Configuration

The application uses the standard ASP.NET Core configuration files:

- `appsettings.json` contains shared settings.
- `appsettings.Development.json` contains Development-specific settings.
- `Properties/launchSettings.json` defines local HTTP and HTTPS launch profiles.

The application does not currently require connection strings, external services, or environment-specific secrets.

## Future Improvements

Possible extensions for this demo include:

- Add a mapped `DELETE /api/products/{id}` endpoint.
- Replace in-memory storage with Entity Framework Core and a persistent database.
- Add automated unit and integration tests.
- Add pagination, filtering, and sorting.
- Add authentication and authorization.
- Use typed results and endpoint metadata for stronger OpenAPI descriptions.
- Add structured logging and request correlation.
- Add optimistic concurrency handling.
- Add production deployment configuration.
