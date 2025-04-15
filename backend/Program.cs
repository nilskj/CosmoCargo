using CosmoCargo.Data;
using CosmoCargo.Services;
using CosmoCargo.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
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
        logger.LogError(ex, "Ett fel uppstod vid migrering av databasen.");
    }
}

app.MapGet("/", () => "CosmoCargo API är igång!").AllowAnonymous();
app.MapPost("/api/auth/login", AuthEndpoints.Login);
app.MapPost("/api/auth/register", AuthEndpoints.Register);
app.MapGet("/api/shipments", ShipmentEndpoints.GetAllShipments).RequireAuthorization();
app.MapGet("/api/shipments/{id}", ShipmentEndpoints.GetShipmentById).RequireAuthorization();
app.MapPost("/api/shipments", ShipmentEndpoints.CreateShipment).RequireAuthorization(policy => policy.RequireRole("Customer"));
app.MapPut("/api/shipments/{id}/status", ShipmentEndpoints.UpdateShipmentStatus).RequireAuthorization(policy => policy.RequireRole("Pilot", "Admin"));
app.MapPut("/api/shipments/{id}/assign", ShipmentEndpoints.AssignPilot).RequireAuthorization(policy => policy.RequireRole("Admin"));

app.Run();
