using APINotes.Data;
using APINotes;
using Microsoft.EntityFrameworkCore;
using API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Get connection string from env vars
var front = builder.Configuration["front"] ?? "frontend";
var server = builder.Configuration["server"] ?? "sql-server";
var db = builder.Configuration["db"] ?? "UserDb";
var user = builder.Configuration["user"] ?? "SA";
var password = builder.Configuration["password"] ?? "Secret123456!";

var connectionString = $"Server={server};Initial Catalog={db};User ID={user};Password={password};TrustServerCertificate=true;";

// Add services
builder.Services.AddSwaggerGen();

// Handle migrations on start
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

// Handle migrations
DatabaseManagementService.MigrationInitialisation(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
