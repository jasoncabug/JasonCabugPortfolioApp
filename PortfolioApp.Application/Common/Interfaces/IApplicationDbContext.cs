using Microsoft.EntityFrameworkCore;
using PortfolioApp.Domain.Entities;

namespace PortfolioApp.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Project> Projects { get; }
    DbSet<Skill> Skills { get; }
    DbSet<WorkExperience> WorkExperiences { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}