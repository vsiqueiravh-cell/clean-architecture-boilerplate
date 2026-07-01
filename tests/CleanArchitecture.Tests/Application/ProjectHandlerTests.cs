using CleanArchitecture.Application.Projects;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;
using CleanArchitecture.Infrastructure.Persistence;
using CleanArchitecture.Tests.Support;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Tests.Application;

public sealed class ProjectHandlerTests
{
    [Fact]
    public async Task CreateProjectCommandHandler_CreatesProjectWithAuditFields()
    {
        await using var dbContext = CreateDbContext();
        var handler = new CreateProjectCommandHandler(dbContext);

        var response = await handler.Handle(
            new CreateProjectCommand("ERP Modernization", "erp26", "Reference architecture"),
            CancellationToken.None);

        var project = await dbContext.Projects.SingleAsync();
        Assert.Equal("ERP26", response.Key);
        Assert.Equal("victor", project.CreatedBy);
        Assert.Equal(new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero), project.CreatedAt);
    }

    [Fact]
    public async Task CreateProjectCommandHandler_RejectsDuplicateProjectKey()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Projects.Add(Project.Create("First Project", "ERP26", null));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CreateProjectCommandHandler(dbContext);

        await Assert.ThrowsAsync<ValidationException>(() =>
            handler.Handle(new CreateProjectCommand("Second Project", "erp26", null), CancellationToken.None));
    }

    [Fact]
    public async Task CompleteProjectTaskCommandHandler_CompletesOpenTask()
    {
        await using var dbContext = CreateDbContext();
        var project = Project.Create("ERP Modernization", "ERP26", null);
        var task = project.AddTask("Design API boundary", null, null, ProjectTaskPriority.Critical);
        dbContext.Projects.Add(project);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new CompleteProjectTaskCommandHandler(
            dbContext,
            new FixedDateTimeProvider(new DateTimeOffset(2026, 7, 1, 13, 0, 0, TimeSpan.Zero)));

        var response = await handler.Handle(new CompleteProjectTaskCommand(task.Id), CancellationToken.None);

        Assert.Equal(ProjectTaskStatus.Completed, response.Status);
        Assert.Equal(new DateTimeOffset(2026, 7, 1, 13, 0, 0, TimeSpan.Zero), response.CompletedAt);
    }

    [Fact]
    public async Task SearchProjectsQueryHandler_FiltersAndPaginatesProjects()
    {
        await using var dbContext = CreateDbContext();
        dbContext.Projects.Add(Project.Create("ERP Modernization", "ERP26", null));
        dbContext.Projects.Add(Project.Create("CRM Portal", "CRM26", null));
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var handler = new SearchProjectsQueryHandler(dbContext);

        var response = await handler.Handle(
            new SearchProjectsQuery("erp", new CleanArchitecture.Shared.Pagination.PagedRequest(1, 10)),
            CancellationToken.None);

        var project = Assert.Single(response.Items);
        Assert.Equal("ERP26", project.Key);
        Assert.Equal(1, response.TotalCount);
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString("N"))
            .Options;

        return new ApplicationDbContext(
            options,
            new FixedCurrentUser("victor", ["Administrator"]),
            new FixedDateTimeProvider(new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero)));
    }
}
