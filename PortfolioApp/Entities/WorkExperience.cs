using PortfolioApp.Domain.Common;

namespace PortfolioApp.Domain.Entities
{
    public class WorkExperience : BaseAuditableEntity
    {
        public string CompanyName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentRole { get; set; }
    }
}
