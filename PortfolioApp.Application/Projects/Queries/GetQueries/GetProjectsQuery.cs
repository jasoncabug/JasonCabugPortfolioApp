using MediatR;
using PortfolioApp.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace PortfolioApp.Application.Projects.Queries.GetQueries
{
    public record GetProjectsQuery : IRequest<List<ProjectDto>>
    {
        public bool? IsFeaturedOnly { get; init; }
    }

    public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, List<ProjectDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetProjectsQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Projects
                .Include(p => p.Skills)
                .AsNoTracking();

            if (request.IsFeaturedOnly == true)
            {
                query = query.Where(p => p.IsFeatured);
            }

            return await query
                .OrderBy(p => p.DisplayOrder)
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
                .ToListAsync(cancellationToken);
        }
    }
}
