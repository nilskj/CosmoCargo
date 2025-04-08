using CosmoCargo.Data;
using CosmoCargo.Models;
using CosmoCargo.Services;
using CosmoCargo.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Lägg till tjänster i containern
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Konfigurera CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Konfigurera databas
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Konfigurera autentisering
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

// Registrera tjänster
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IShipmentService, ShipmentService>();
builder.Services.AddScoped<ITollFormService, TollFormService>();
builder.Services.AddScoped<IRiskAssessmentService, RiskAssessmentService>();

var app = builder.Build();

// Konfigurera HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

// Seed-data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
    DbInitializer.Initialize(context);
}

// API-endpoints
app.MapGet("/", () => "CosmoCargo API är igång!").AllowAnonymous();

// Auth endpoints
app.MapPost("/api/auth/login", AuthEndpoints.Login);
app.MapPost("/api/auth/register", AuthEndpoints.Register);

// Shipment endpoints
app.MapGet("/api/shipments", ShipmentEndpoints.GetAllShipments)
   .RequireAuthorization();
app.MapGet("/api/shipments/{id}", ShipmentEndpoints.GetShipmentById)
   .RequireAuthorization();
app.MapPost("/api/shipments", ShipmentEndpoints.CreateShipment)
   .RequireAuthorization(policy => policy.RequireRole("Customer"));
app.MapPut("/api/shipments/{id}/status", ShipmentEndpoints.UpdateShipmentStatus)
   .RequireAuthorization(policy => policy.RequireRole("Pilot", "Admin"));
app.MapPut("/api/shipments/{id}/assign", ShipmentEndpoints.AssignPilot)
   .RequireAuthorization(policy => policy.RequireRole("Admin"));

// TollForm endpoints
app.MapGet("/api/tollforms", TollFormEndpoints.GetAllTollForms)
   .RequireAuthorization(policy => policy.RequireRole("Admin"));
app.MapGet("/api/tollforms/{id}", TollFormEndpoints.GetTollFormById)
   .RequireAuthorization();
app.MapPost("/api/tollforms", TollFormEndpoints.CreateTollForm)
   .RequireAuthorization(policy => policy.RequireRole("Customer"));
app.MapPut("/api/tollforms/{id}/review", TollFormEndpoints.ReviewTollForm)
   .RequireAuthorization(policy => policy.RequireRole("Admin"));

app.Run(); 