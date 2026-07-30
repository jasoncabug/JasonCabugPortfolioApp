using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Domain.Entities;

namespace PortfolioApp.Infrastructure.Persistence.Configurations
{
    public class WorkExperienceConfiguration : IEntityTypeConfiguration<WorkExperience>
    {
        public void Configure(EntityTypeBuilder<WorkExperience> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.CompanyName)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(w => w.Position)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(w => w.Description)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(w => w.StartDate)
                .IsRequired();
        }
    }
}
