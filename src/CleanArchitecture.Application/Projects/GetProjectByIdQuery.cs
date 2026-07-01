using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Projects;

public sealed record GetProjectByIdQuery(Guid Id) : IRequest<ProjectResponse>;

public sealed class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectResponse>
{
    private readonly IApplicationDbContext _dbContext;

    public GetProjectByIdQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProjectResponse> Handle(
        GetProjectByIdQuery request,
        CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects
            .Include(project => project.Tasks)
            .FirstOrDefaultAsync(project => project.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException($"Project '{request.Id}' was not found.");

        return ProjectResponse.FromEntity(project);
    }
}
