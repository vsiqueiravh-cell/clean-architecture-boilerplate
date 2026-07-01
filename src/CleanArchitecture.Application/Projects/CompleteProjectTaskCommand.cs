using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Projects;

public sealed record CompleteProjectTaskCommand(Guid TaskId) : IRequest<ProjectTaskResponse>;

public sealed class CompleteProjectTaskCommandHandler : IRequestHandler<CompleteProjectTaskCommand, ProjectTaskResponse>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CompleteProjectTaskCommandHandler(
        IApplicationDbContext dbContext,
        IDateTimeProvider dateTimeProvider)
    {
        _dbContext = dbContext;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<ProjectTaskResponse> Handle(
        CompleteProjectTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _dbContext.ProjectTasks
            .FirstOrDefaultAsync(task => task.Id == request.TaskId, cancellationToken)
            ?? throw new NotFoundException($"Project task '{request.TaskId}' was not found.");

        task.Complete(_dateTimeProvider.UtcNow);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ProjectTaskResponse.FromEntity(task);
    }
}
