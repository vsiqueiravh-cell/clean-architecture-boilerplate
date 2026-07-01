using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.Common;
using CleanArchitecture.Domain.Enums;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Projects;

public sealed record CreateProjectTaskCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    DateOnly? DueDate,
    ProjectTaskPriority Priority) : IRequest<ProjectTaskResponse>;

public sealed class CreateProjectTaskCommandValidator : AbstractValidator<CreateProjectTaskCommand>
{
    public CreateProjectTaskCommandValidator()
    {
        RuleFor(command => command.ProjectId)
            .NotEmpty();

        RuleFor(command => command.Title)
            .NotEmpty()
            .MaximumLength(180);

        RuleFor(command => command.Description)
            .MaximumLength(800);

        RuleFor(command => command.Priority)
            .IsInEnum();
    }
}

public sealed class CreateProjectTaskCommandHandler : IRequestHandler<CreateProjectTaskCommand, ProjectTaskResponse>
{
    private readonly IApplicationDbContext _dbContext;

    public CreateProjectTaskCommandHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProjectTaskResponse> Handle(
        CreateProjectTaskCommand request,
        CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects
            .Include(project => project.Tasks)
            .FirstOrDefaultAsync(project => project.Id == request.ProjectId, cancellationToken)
            ?? throw new NotFoundException($"Project '{request.ProjectId}' was not found.");

        var task = project.AddTask(
            request.Title,
            request.Description,
            request.DueDate,
            request.Priority);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ProjectTaskResponse.FromEntity(task);
    }
}
