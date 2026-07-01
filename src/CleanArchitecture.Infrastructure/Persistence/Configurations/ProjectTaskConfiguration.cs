using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations;

public sealed class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
{
    public void Configure(EntityTypeBuilder<ProjectTask> builder)
    {
        builder.ToTable("project_tasks");

        builder.HasKey(task => task.Id);

        builder.Property(task => task.Title)
            .HasMaxLength(180)
            .IsRequired();

        builder.Property(task => task.Description)
            .HasMaxLength(800);

        builder.Property(task => task.Priority)
            .HasConversion<string>()
            .HasMaxLength(24)
            .IsRequired();

        builder.Property(task => task.Status)
            .HasConversion<string>()
            .HasMaxLength(24)
            .IsRequired();

        builder.Property(task => task.CreatedBy)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(task => task.LastModifiedBy)
            .HasMaxLength(160);

        builder.HasIndex(task => new { task.ProjectId, task.Status });
    }
}
