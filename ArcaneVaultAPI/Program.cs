// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using ArcaneVaultAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Scalar.AspNetCore;

namespace ArcaneVaultAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();

            // EF Core with SQLite
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
                    ?? "Data Source=arcanevault.db")
                .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning)));

            // OpenAPI documentation
            builder.Services.AddOpenApi();

            // CORS for Web app
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowWebApp", policy =>
                {
                    policy.WithOrigins("http://localhost:5000", "https://localhost:7000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var app = builder.Build();

            // Global exception middleware
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "application/json";
                    var error = new { Message = "An unexpected error occurred.", Detail = ex.Message };
                    await context.Response.WriteAsJsonAsync(error);
                }
            });

            // Apply migrations and create/seed the database
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.Migrate();
            }

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseCors("AllowWebApp");

            app.Use(async (context, next) =>
            {
                var expectedApiKey = builder.Configuration["ApiSettings:ApiKey"];
                if (!string.IsNullOrEmpty(expectedApiKey))
                {
                    // Bypass API key requirement for image endpoints so the browser can load them directly via <img>
                    if (context.Request.Path.HasValue && context.Request.Path.Value.EndsWith("/image", StringComparison.OrdinalIgnoreCase))
                    {
                        await next();
                        return;
                    }

                    if (!context.Request.Headers.TryGetValue("X-Api-Key", out var extractedApiKey) ||
                        extractedApiKey != expectedApiKey)
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Unauthorized: Invalid or missing API Key.");
                        return;
                    }
                }
                await next();
            });

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
