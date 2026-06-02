using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
