using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text;
using Uam.LabHelpDesk.Api.Interfaces;
using Uam.LabHelpDesk.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<Uam.LabHelpDesk.Api.Interfaces.ISmtpService, Uam.LabHelpDesk.Api.Services.SmtpService>();
builder.Services.AddScoped<Uam.LabHelpDesk.Api.Interfaces.IEmailNotificationService, Uam.LabHelpDesk.Api.Services.EmailNotificationService>();
builder.Services.AddScoped<Uam.LabHelpDesk.Api.Interfaces.IOtpCodeRepository, Uam.LabHelpDesk.Api.Repositories.OtpCodeRepository>();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "UAM Lab Help Desk API",
        Version = "v1",
        Description = "API para gestión de laboratorios y equipos - UAM Lab Help Desk"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese: Bearer {su_token_jwt}"
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

var resourcesPath = builder.Configuration["Localization:ResourcesPath"] ?? "Resources";
builder.Services.AddLocalization(options => options.ResourcesPath = resourcesPath);

builder.Services.AddDbContext<Uam.LabHelpDesk.Api.Data.AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.MigrationsAssembly("Uam.LabHelpDesk.Api")));

builder.Services.AddScoped<Uam.LabHelpDesk.Api.Interfaces.IUnitOfWork, Uam.LabHelpDesk.Api.Repositories.UnitOfWork>();

var jwtSection = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSection["SecretKey"] ?? throw new InvalidOperationException("Falta Jwt:SecretKey en appsettings.json");
var key = Encoding.UTF8.GetBytes(secretKey);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Semillar base de datos con Roles y Usuario Administrador por defecto
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<Uam.LabHelpDesk.Api.Data.AppDbContext>();
    context.Database.EnsureCreated(); // Asegurar que la base de datos esté creada

    if (!context.Roles.Any())
    {
        var adminRole = new Uam.LabHelpDesk.Api.Models.Role
        {
            Name = "Administrator",
            Description = "Administrador del Sistema",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        var techRole = new Uam.LabHelpDesk.Api.Models.Role
        {
            Name = "Technician",
            Description = "Técnico de Laboratorio",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        var instructorRole = new Uam.LabHelpDesk.Api.Models.Role
        {
            Name = "Instructor",
            Description = "Instructor del Laboratorio",
            IsActive = true,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };

        context.Roles.AddRange(adminRole, techRole, instructorRole);
        context.SaveChanges();
    }

    var adminRoleDb = context.Roles.FirstOrDefault(r => r.Name == "Administrator");
    if (adminRoleDb != null)
    {
        bool changed = false;
        if (!context.Users.Any(u => u.Email == "admin@uam.edu"))
        {
            var adminUser = new Uam.LabHelpDesk.Api.Models.User
            {
                RoleId = adminRoleDb.Id,
                FirstName = "Admin",
                LastName = "UAM",
                Email = "admin@uam.edu",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin1234!"),
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };
            context.Users.Add(adminUser);
            changed = true;
        }

        if (!context.Users.Any(u => u.Email == "karinabril19@gmail.com"))
        {
            var newAdminUser = new Uam.LabHelpDesk.Api.Models.User
            {
                RoleId = adminRoleDb.Id,
                FirstName = "Karina",
                LastName = "Abril",
                Email = "karinabril19@gmail.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin1234!"),
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };
            context.Users.Add(newAdminUser);
            changed = true;
        }

        if (changed)
        {
            context.SaveChanges();
        }
    }
}

var defaultCulture = app.Configuration["Localization:DefaultCulture"] ?? "es";
var supportedCultureCodes = app.Configuration.GetSection("Localization:SupportedCultures").Get<string[]>() ?? ["es", "en"];
var supportedCultures = supportedCultureCodes.Select(c => new CultureInfo(c)).ToArray();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseMiddleware<Uam.LabHelpDesk.Api.Middlewares.ExceptionMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
