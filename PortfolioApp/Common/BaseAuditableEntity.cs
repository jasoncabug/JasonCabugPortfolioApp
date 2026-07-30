using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioApp.Domain.Common
{
    public abstract class BaseAuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTimeOffset? LastModifiedAt { get; set; }
        public string? LastModifiedBy { get; set; }
    }
}
