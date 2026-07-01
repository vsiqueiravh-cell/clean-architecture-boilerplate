using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations;

public sealed class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("projects");

        builder.HasKey(project => project.Id);

        builder.Property(project => project.Key)
            .HasMaxLength(12)
            .IsRequired();

        builder.Property(project => project.Name)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(project => project.Description)
            .HasMaxLength(800);

        builder.Property(project => project.CreatedBy)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(project => project.LastModifiedBy)
            .HasMaxLength(160);

        builder.HasIndex(project => project.Key)
            .IsUnique();

        builder.HasMany(project => project.Tasks)
            .WithOne()
            .HasForeignKey(task => task.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
