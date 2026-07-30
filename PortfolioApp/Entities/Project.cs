using PortfolioApp.Domain.Common;

namespace PortfolioApp.Domain.Entities
{
    public class Project : BaseAuditableEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public string? DemoUrl { get; set; }
        public string? ProjectUrl { get; set; }
        public string? GithubUrl { get; set; }
        public bool IsFeatured { get; set; }
        public int DisplayOrder { get; set; }

        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
    }
}
