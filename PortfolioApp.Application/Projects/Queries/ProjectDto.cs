namespace PortfolioApp.Application.Projects.Queries
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? DemoUrl { get; set; }
        public string? ProjectUrl { get; set; }
        public string? GithubUrl { get; set; }
        public bool IsFeatured { get; set; }
        public int DisplayOrder { get; set; }
        public List<string> Skills { get; set; } = new();
    }
}
