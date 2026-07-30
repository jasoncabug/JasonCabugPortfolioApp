using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioApp.Application.Projects.Commands.CreateProject;
using PortfolioApp.Application.Projects.Queries;
using PortfolioApp.Application.Projects.Queries.GetProjectById;
using PortfolioApp.Application.Projects.Queries.GetQueries;

namespace PortfolioApp.Api.Controllers;

public class ProjectsController : ApiControllerBase
{
    /// <summary>
    /// Gets all portfolio projects, with optional featured filtering.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(List<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProjectDto>>> GetAll([FromQuery] bool? isFeatured)
    {
        var result = await Mediator.Send(new GetProjectsQuery { IsFeaturedOnly = isFeatured });
        return Ok(result);
    }

    /// <summary>
    /// Gets a single project by ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProjectDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetProjectByIdQuery(id));
        if (result == null) return NotFound();

        return Ok(result);
    }

    /// <summary>
    /// Creates a new portfolio project entry.
    /// </summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Create([FromBody] CreateProjectCommand command)
    {
        var projectId = await Mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = projectId }, projectId);
    }
}