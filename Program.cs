using Fullstack.MinimalApisDemo.Models;
using Fullstack.MinimalApisDemo.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IProductService, ProductService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();

var app = builder.Build();
// Handle unexpected exceptions using Problem Details.
app.UseExceptionHandler();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Simple health endpoint.
app.MapGet("/api/health", () =>
{
    return Results.Ok(new
    {
        Status = "Healthy",
        Message = "DevStore Minimal API is running."
    });
})
.WithName("GetHealth")
.WithTags("Health")
.WithSummary("Check API health");



// Create a route group for product endpoints.
var products = app.MapGroup("/api/products")
    .WithTags("Products");






// GET: Fetch all products.
products.MapGet("/", (IProductService service) =>
{
    return Results.Ok(service.GetAll());
})
.WithName("GetAllProducts")
.WithSummary("Get all products");


// GET: Fetch one product by ID.
products.MapGet("/{id:int}", (
    int id,
    IProductService service) =>
{
    var product = service.GetById(id);

    if (product is null)
    {
        return Results.NotFound(new
        {
            Message = $"Product with ID {id} was not found."
        });
    }

    return Results.Ok(product);
})
.WithName("GetProductById")
.WithSummary("Get a product by ID");



// POST: Create a product.
products.MapPost("/", (
    CreateProductRequest request,
    IProductService service) =>
{
    var validationError = ValidateProduct(
        request.Name,
        request.Description,
        request.Price,
        request.Stock);

    if (validationError is not null)
    {
        return Results.BadRequest(new
        {
            Message = validationError
        });
    }

    var product = service.Create(request);

    return Results.Created(
        $"/api/products/{product.Id}",
        product);
})
.WithName("CreateProduct")
.WithSummary("Create a product");



// PUT: Replace all editable product fields.

products.MapPut("/{id:int}", (
    int id,
    UpdateProductRequest request,
    IProductService service) =>
{
    var validationError = ValidateProduct(
        request.Name,
        request.Description,
        request.Price,
        request.Stock);

    if (validationError is not null)
    {
        return Results.BadRequest(new
        {
            Message = validationError
        });
    }

    var product = service.Update(id, request);

    return product is null
        ? Results.NotFound(new
        {
            Message = $"Product with ID {id} was not found."
        })
        : Results.Ok(product);
})
.WithName("UpdateProduct")
.WithSummary("Replace an existing product");




// PATCH: Update selected product fields.
products.MapPatch("/{id:int}", (
    int id,
    PatchProductRequest request,
    IProductService service) =>
{
    var productExists = service.GetById(id);

    if (productExists is null)
    {
        return Results.NotFound(new
        {
            Message = $"Product with ID {id} was not found."
        });
    }

    if (!HasPatchValues(request))
    {
        return Results.BadRequest(new
        {
            Message = "Provide at least one field to update."
        });
    }

    var validationError = ValidatePatch(request);

    if (validationError is not null)
    {
        return Results.BadRequest(new
        {
            Message = validationError
        });
    }

    var product = service.Patch(id, request);

    return Results.Ok(product);
})
.WithName("PatchProduct")
.WithSummary("Update selected product fields");




app.Run();

// Validate fields shared by POST and PUT.
static string? ValidateProduct(
    string name,
    string description,
    decimal price,
    int stock)
{
    if (string.IsNullOrWhiteSpace(name))
    {
        return "Product name is required.";
    }

    if (name.Trim().Length > 100)
    {
        return "Product name cannot exceed 100 characters.";
    }

    if (description is null)
    {
        return "Product description is required.";
    }

    if (description.Trim().Length > 500)
    {
        return "Description cannot exceed 500 characters.";
    }

    if (price <= 0)
    {
        return "Price must be greater than zero.";
    }

    if (stock < 0)
    {
        return "Stock cannot be negative.";
    }

    return null;
}

// Check whether the PATCH request includes any fields.
static bool HasPatchValues(PatchProductRequest request)
{
    return request.Name is not null
        || request.Description is not null
        || request.Price.HasValue
        || request.Stock.HasValue
        || request.IsActive.HasValue;
}
// Validate only fields explicitly supplied in PATCH.
static string? ValidatePatch(PatchProductRequest request)
{
    if (request.Name is not null)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return "Product name cannot be empty.";
        }

        if (request.Name.Trim().Length > 100)
        {
            return "Product name cannot exceed 100 characters.";
        }
    }

    if (request.Description is not null
        && request.Description.Trim().Length > 500)
    {
        return "Description cannot exceed 500 characters.";
    }

    if (request.Price.HasValue && request.Price.Value <= 0)
    {
        return "Price must be greater than zero.";
    }

    if (request.Stock.HasValue && request.Stock.Value < 0)
    {
        return "Stock cannot be negative.";
    }

    return null;
}



