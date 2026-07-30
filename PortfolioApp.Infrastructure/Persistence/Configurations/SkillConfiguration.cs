using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioApp.Infrastructure.Persistence.Configurations
{
    public class SkillConfiguration : IEntityTypeConfiguration<Skill>
    {
        public void Configure(EntityTypeBuilder<Skill> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(s => s.Category)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(s => s.ProficiencyPercentage)
                .IsRequired();
        }
    }
}
