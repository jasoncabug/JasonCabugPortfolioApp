using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Domain.Entities;

namespace PortfolioApp.Infrastructure.Persistence.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(p => p.DemoUrl)
                .HasMaxLength(500);

            builder.Property(p => p.GithubUrl)
                .HasMaxLength(500);

            builder.Property(p => p.ImageUrl)
                .HasMaxLength(500);
        }
    }
}
