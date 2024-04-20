using System.Data.SqlClient;
using APBD6.Properties.DTOs;
using APBD6.Properties.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddConnections();
builder.Services.AddValidatorsFromAssemblyContaining<CreateAnimalValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/animals", (IConfiguration configuration, string? orderBy) =>
{
    var animals = new List<GetAllAnimals>();
    using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
    {
        List<string> possibleOrderBy = ["name", "area", "description", "category"];
        if (!possibleOrderBy.Contains(orderBy)) orderBy = "name";
        var sqlCommand = new SqlCommand("SELECT * FROM Animal ORDER BY " + orderBy, sqlConnection);
        sqlCommand.Connection.Open();
        var reader = sqlCommand.ExecuteReader();

        while (reader.Read())
        {
            animals.Add(new GetAllAnimals(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4)));
        }

        return Results.Ok(animals);
    }
});

app.MapPost("/api/animals", (IConfiguration configuration, CreateAnimalRequest animal, IValidator<CreateAnimalRequest> validator) =>
{
    var validation = validator.Validate(animal);
    if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());
    
    using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
    {
        var sqlCommand = new SqlCommand("INSERT INTO Animal (Name, Description, Category, Area) " +
                                        "values (@name, @description, @category, @area)", sqlConnection);
        
        sqlCommand.Parameters.AddWithValue("@name", animal.name);
        sqlCommand.Parameters.AddWithValue("@description", animal.description);
        sqlCommand.Parameters.AddWithValue("@category", animal.category);
        sqlCommand.Parameters.AddWithValue("@area", animal.area);
        
        sqlCommand.Connection.Open();
        sqlCommand.ExecuteNonQuery();

        return Results.Created("", null);
    }
});

app.MapPut("/api/animals/{id:int}", (IConfiguration configuration, CreateAnimalRequest animal, int id, IValidator<CreateAnimalRequest> validator) =>
{
    var validation = validator.Validate(animal);
    if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());
    
    using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
    {
        var sqlCommand = new SqlCommand("UPDATE Animal SET " +
                                        "Name = @name, " +
                                        "Description = @description, " +
                                        "Category = @category, " +
                                        "Area = @area " +
                                        "WHERE Id = @id", sqlConnection);
        
        sqlCommand.Parameters.AddWithValue("@id", id);
        sqlCommand.Parameters.AddWithValue("@name", animal.name);
        sqlCommand.Parameters.AddWithValue("@description", animal.description);
        sqlCommand.Parameters.AddWithValue("@category", animal.category);
        sqlCommand.Parameters.AddWithValue("@area", animal.area);
        
        sqlCommand.Connection.Open();
        sqlCommand.ExecuteNonQuery();

        var rowsAffected = sqlCommand.ExecuteNonQuery();
        return rowsAffected == 0 ? Results.NotFound() : Results.NoContent();
    }
});

app.MapDelete("/api/animals/{id:int}", (IConfiguration configuration, int id) => 
{
    using (var sqlConnection = new SqlConnection(configuration.GetConnectionString("Default")))
    {
        var sqlCommand = new SqlCommand("DELETE FROM Animal WHERE Id = @id", sqlConnection);
        
        sqlCommand.Parameters.AddWithValue("@id", id);
        
        sqlCommand.Connection.Open();
        sqlCommand.ExecuteNonQuery();
        
        var rowsAffected = sqlCommand.ExecuteNonQuery();
        return rowsAffected == 0 ? Results.NotFound() : Results.NoContent();
    }
});

app.Run();
