using CleanArchitecture.Domain.Common;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Tests.Domain;

public sealed class ProjectTests
{
    [Fact]
    public void Create_NormalizesProjectKey()
    {
        var project = Project.Create("ERP Modernization", "erp26", null);

        Assert.Equal("ERP26", project.Key);
    }

    [Fact]
    public void Archive_RejectsProjectWithOpenTasks()
    {
        var project = Project.Create("ERP Modernization", "ERP26", null);
        project.AddTask("Define integration boundary", null, null, ProjectTaskPriority.High);

        var exception = Assert.Throws<DomainException>(project.Archive);

        Assert.Equal("Projects with open tasks cannot be archived.", exception.Message);
    }
}
