using MediatR;
using MilesManager.Api.Features.Auth;
using MilesManager.Api.Features.Health;
using MilesManager.Api.Features.Trips;
using MilesManager.Api.Features.Users;
using MilesManager.Application;
using MilesManager.Application.Features.Users.Abstractions;
using MilesManager.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtSection = builder.Configuration.GetSection("Jwt");
var issuer = jwtSection["Issuer"] ?? throw new InvalidOperationException("Missing JWT issuer.");
var audience = jwtSection["Audience"] ?? throw new InvalidOperationException("Missing JWT audience.");
var key = jwtSection["Key"] ?? throw new InvalidOperationException("Missing JWT signing key.");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
                var principal = context.Principal;
                var subject = principal?.FindFirstValue("sub");
                var tokenVersionText = principal?.FindFirstValue("tv");

                if (!Guid.TryParse(subject, out var userId) || !int.TryParse(tokenVersionText, out var tokenVersion))
                {
                    context.Fail("Invalid token claims.");
                    return;
                }

                var user = await userRepository.GetByIdAsync(userId, context.HttpContext.RequestAborted);
                if (user is null || !user.IsActive || user.TokenVersion != tokenVersion)
                {
                    context.Fail("Token is no longer valid.");
                }
            }
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "MilesManager API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your JWT token."
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

    var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{typeof(Program).Assembly.GetName().Name}.xml");
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
        policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapHealthEndpoints();
app.MapTripEndpoints();
app.MapAuthEndpoints();
app.MapUserEndpoints();

app.Run();
