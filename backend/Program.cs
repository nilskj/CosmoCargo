using CosmoCargo.Data;
using CosmoCargo.Services;
using CosmoCargo.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CosmoCargo API",
        Version = "v1",
        Description = "API for managing space cargo shipments"
    });

    // Add JWT Authentication support in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "defaultDevKeyThatShouldBeReplaced"))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<IPilotService, PilotService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ett fel uppstod vid initialisering av databasen.");
    }
}

app.MapPost("/api/auth/login", AuthEndpoints.Login);
app.MapGet("/api/shipments", ShipmentEndpoints.GetAllShipments).RequireAuthorization();
app.MapGet("/api/shipments/{id}", ShipmentEndpoints.GetShipmentById).RequireAuthorization();
app.MapPost("/api/shipments", ShipmentEndpoints.CreateShipment).RequireAuthorization(policy => policy.RequireRole("Customer"));
app.MapPut("/api/shipments/{id}/status", ShipmentEndpoints.UpdateShipmentStatus).RequireAuthorization(policy => policy.RequireRole("Pilot", "Admin"));
app.MapPut("/api/shipments/{id}/assign", ShipmentEndpoints.AssignPilot).RequireAuthorization(policy => policy.RequireRole("Admin"));
app.MapGet("/api/pilots", PilotEndpoints.GetAllPilots).RequireAuthorization(policy => policy.RequireRole("Admin"));
app.MapGet("/api/pilots/{id}", PilotEndpoints.GetPilotById).RequireAuthorization(policy => policy.RequireRole("Admin"));
app.MapGet("/api/pilots/{id}/availability", PilotEndpoints.GetPilotAvailability).RequireAuthorization(policy => policy.RequireRole("Admin"));

app.Run();
