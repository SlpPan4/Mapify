using System.Text.Json.Serialization;
using MapifyBackend.database_files;

var builder = WebApplication.CreateBuilder(args);

// Initialize db
DatabaseInitializer.EnsureDatabaseCreated();

// Register services
builder.Services.AddSingleton<DatabaseService>(); // One for all time
builder.Services.AddScoped<StratService>();     // Gets created for each request
builder.Services.AddScoped<CategoryService>(); // same
builder.Services.AddScoped<OperatorService>();
builder.Services.AddScoped<SubmissionService>();

// add Cors and controllers support
builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Middleware
app.UseCors("AllowAll");
app.MapControllers(); // Connects URL with controllers

// Run server
app.Run();
