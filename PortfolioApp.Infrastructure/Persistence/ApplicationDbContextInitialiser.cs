using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PortfolioApp.Domain.Entities;

namespace PortfolioApp.Infrastructure.Persistence
{
    public static class InitialiserExtensions
    {
        public static async Task InitialiseDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

            await initialiser.InitialiseAsync();
            await initialiser.SeedAsync();
        }
    }

    public class ApplicationDbContextInitialiser
    {
        private readonly ILogger<ApplicationDbContextInitialiser> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ApplicationDbContextInitialiser(
            ILogger<ApplicationDbContextInitialiser> logger,
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                if (_context.Database.IsSqlServer())
                {
                    await _context.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while initialising the database.");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }

        private async Task TrySeedAsync()
        {
            // 1. Seed Default Admin User
            var administrator = new ApplicationUser
            {
                UserName = "admin@portfolio.com",
                Email = "admin@portfolio.com",
                FirstName = "System",
                LastName = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            if (_userManager.Users.All(u => u.UserName != administrator.UserName))
            {
                await _userManager.CreateAsync(administrator, "Admin123!");
            }

            // 2. Seed Default Sample Projects (if empty)
            if (!_context.Projects.Any())
            {
                _context.Projects.Add(new Project
                {
                    Title = "Enterprise Portfolio API",
                    Description = "Clean Architecture backend with .NET 9/10, CQRS, MediatR, EF Core, and JWT Authentication.",
                    ProjectUrl = "https://github.com/example/portfolio-api",
                    IsFeatured = true,
                    DisplayOrder = 1
                });

                await _context.SaveChangesAsync();
            }
        }
    }
}