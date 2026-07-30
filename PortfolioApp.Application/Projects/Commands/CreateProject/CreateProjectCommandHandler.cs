using MediatR;
using PortfolioApp.Application.Common.Interfaces;
using PortfolioApp.Domain.Entities;

namespace PortfolioApp.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, Guid>
{
    private readonly IApplicationDbContext _context;

    public CreateProjectCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var entity = new Project
        {
            Title = request.Title,
            Description = request.Description,
            DemoUrl = request.DemoUrl,
            GithubUrl = request.GithubUrl
        };

        _context.Projects.Add(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}