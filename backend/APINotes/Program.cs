using APINotes.Data;
using APINotes;
using Microsoft.EntityFrameworkCore;
using API.Services;
using System.Threading.RateLimiting;
using APINotes.Models;
using Microsoft.AspNetCore.Identity;
using BCrypt.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Handle reference cycles
builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;

            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            options.JsonSerializerOptions.WriteIndented = true;
        });

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

// Handle migrations on start
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(connectionString);
});

    // Define rate limiter middleware, limit 100 calls in 1 minute
    builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(1)
            }));

    options.OnRejected = (context, cancellationToken) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = retryAfter.TotalSeconds.ToString();
        }

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.WriteAsync("Too many requests. Please try again later");

        return new ValueTask();
    };
});

var app = builder.Build();

// Handle migrations
DatabaseManagementService.MigrationInitialisation(app);

// Create default user
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

    var defaultUser = new User
    {
        Username = "admin",
        Password = "Aa123456!",
        IsActive = true
    };

    defaultUser.Password = BCrypt.Net.BCrypt.HashPassword(defaultUser.Password);

    // Add the user to the database
    await dbContext.Users.AddAsync(defaultUser);
    await dbContext.SaveChangesAsync();
}

// Use rate limiter middleware
app.UseRateLimiter();

app.UseSwagger();
app.UseSwaggerUI();

// Redirect requests from http
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
