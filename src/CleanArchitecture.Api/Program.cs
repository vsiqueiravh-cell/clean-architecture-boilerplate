using System.Text;
using System.Text.Json.Serialization;
using CleanArchitecture.Api.Support;
using CleanArchitecture.Application;
using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.Authentication;
using CleanArchitecture.Application.Projects;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Authentication;
using CleanArchitecture.Shared.Pagination;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Clean Architecture Boilerplate",
        Version = "v1",
        Description = "Enterprise API boilerplate with Clean Architecture, CQRS, validation, EF Core and JWT."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document, null),
            []
        }
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var jwtOptions = builder.Configuration
    .GetSection(JwtOptions.SectionName)
    .Get<JwtOptions>() ?? new JwtOptions();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("projects:write", policy => policy.RequireRole("Administrator", "ProjectManager"));

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.DocumentTitle = "Clean Architecture Boilerplate";
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Clean Architecture Boilerplate v1");
});

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/health").AllowAnonymous();

var api = app.MapGroup("/api/v1");

api.MapPost("/auth/token", (LoginRequest request, IJwtTokenService tokenService) =>
    Results.Ok(tokenService.CreateToken(request)))
    .AllowAnonymous()
    .WithTags("Authentication")
    .WithName("CreateAccessToken");

var projects = api.MapGroup("/projects")
    .RequireAuthorization()
    .WithTags("Projects");

projects.MapPost("", async (
    CreateProjectCommand command,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var response = await sender.Send(command, cancellationToken);
    return Results.Created($"/api/v1/projects/{response.Id}", response);
})
.RequireAuthorization("projects:write")
.WithName("CreateProject");

projects.MapGet("/{id:guid}", async (
    Guid id,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var response = await sender.Send(new GetProjectByIdQuery(id), cancellationToken);
    return Results.Ok(response);
})
.WithName("GetProjectById");

projects.MapGet("", async (
    string? searchTerm,
    int? page,
    int? pageSize,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var response = await sender.Send(
        new SearchProjectsQuery(searchTerm, new PagedRequest(page ?? 1, pageSize ?? 25)),
        cancellationToken);

    return Results.Ok(response);
})
.WithName("SearchProjects");

projects.MapPost("/{id:guid}/tasks", async (
    Guid id,
    CreateProjectTaskRequest request,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var command = new CreateProjectTaskCommand(
        id,
        request.Title,
        request.Description,
        request.DueDate,
        request.Priority);

    var response = await sender.Send(command, cancellationToken);
    return Results.Created($"/api/v1/projects/{id}/tasks/{response.Id}", response);
})
.RequireAuthorization("projects:write")
.WithName("CreateProjectTask");

api.MapPost("/tasks/{id:guid}/complete", async (
    Guid id,
    ISender sender,
    CancellationToken cancellationToken) =>
{
    var response = await sender.Send(new CompleteProjectTaskCommand(id), cancellationToken);
    return Results.Ok(response);
})
.RequireAuthorization("projects:write")
.WithTags("Project Tasks")
.WithName("CompleteProjectTask");

app.Run();

public sealed record CreateProjectTaskRequest(
    string Title,
    string? Description,
    DateOnly? DueDate,
    ProjectTaskPriority Priority);

public partial class Program;
