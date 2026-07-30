using MediatR;
using Microsoft.EntityFrameworkCore;
using PortfolioApp.Application.Common.Interfaces;

namespace PortfolioApp.Application.Projects.Queries.GetProjectById
{
    public record GetProjectByIdQuery(Guid Id) : IRequest<ProjectDto?>;

    public class GetProjectByIdQueryHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetProjectByIdQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProjectDto?> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Projects
                .Include(p => p.Skills)
                .AsNoTracking()
                .Where(p => p.Id == request.Id)
                .Select(p => new ProjectDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Description = p.Description,
                    ImageUrl = p.ImageUrl,
                    DemoUrl = p.DemoUrl,
                    ProjectUrl = p.ProjectUrl,
                    GithubUrl = p.GithubUrl,
                    IsFeatured = p.IsFeatured,
                    DisplayOrder = p.DisplayOrder,
                    Skills = p.Skills.Select(s => s.Name).ToList()
                })
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}