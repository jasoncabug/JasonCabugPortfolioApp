using PortfolioApp.Domain.Common;

namespace PortfolioApp.Domain.Entities
{
    public class Skill : BaseAuditableEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // e.g., "Backend", "Frontend", "Cloud"
        public int ProficiencyPercentage { get; set; }
    }
}
