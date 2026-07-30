using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioApp.Application.Projects.Commands.CreateProject
{
    public record CreateProjectCommand(
        string Title,
        string Description,
        string? DemoUrl,
        string? GithubUrl
    ) : IRequest<Guid>;
}
