using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Projects;

public sealed record CreateProjectCommand(
    string Name,
    string Key,
    string? Description) : IRequest<ProjectResponse>;

public sealed class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty()
            .MaximumLength(160);

        RuleFor(command => command.Key)
            .NotEmpty()
            .Matches("^[a-zA-Z0-9]{2,12}$")
            .WithMessage("Project key must be 2 to 12 alphanumeric characters.");

        RuleFor(command => command.Description)
            .MaximumLength(800);
    }
}

public sealed class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectResponse>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateProjectCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProjectResponse> Handle(
        CreateProjectCommand request,
        CancellationToken cancellationToken)
    {
        var normalizedKey = request.Key.Trim().ToUpperInvariant();
        var exists = await _dbContext.Projects
            .AnyAsync(project => project.Key == normalizedKey, cancellationToken);

        if (exists)
        {
            throw new ValidationException($"Project key '{normalizedKey}' is already in use.");
        }

        var project = Project.Create(request.Name, request.Key, request.Description);

        _dbContext.Projects.Add(project);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ProjectResponse.FromEntity(project);
    }
}
