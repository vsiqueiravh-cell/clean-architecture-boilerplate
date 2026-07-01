using CleanArchitecture.Application.Abstractions;
using CleanArchitecture.Shared.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Projects;

public sealed record SearchProjectsQuery(
    string? SearchTerm,
    PagedRequest Page) : IRequest<PagedList<ProjectResponse>>;

public sealed class SearchProjectsQueryHandler : IRequestHandler<SearchProjectsQuery, PagedList<ProjectResponse>>
{
    private readonly IApplicationDbContext _dbContext;

    public SearchProjectsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedList<ProjectResponse>> Handle(
        SearchProjectsQuery request,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Projects
            .AsNoTracking()
            .Include(project => project.Tasks)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var searchTerm = request.SearchTerm.Trim().ToLowerInvariant();
            query = query.Where(project =>
                project.Name.ToLower().Contains(searchTerm)
                || project.Key.ToLower().Contains(searchTerm));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var projects = await query
            .OrderBy(project => project.Key)
            .Skip(request.Page.Skip)
            .Take(request.Page.NormalizedPageSize)
            .ToListAsync(cancellationToken);

        return new PagedList<ProjectResponse>(
            projects.Select(ProjectResponse.FromEntity).ToArray(),
            request.Page.NormalizedPage,
            request.Page.NormalizedPageSize,
            totalCount);
    }
}
