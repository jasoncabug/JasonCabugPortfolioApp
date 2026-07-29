using System;
using System.Collections.Generic;
using System.Text;

namespace PortfolioApp.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
    }
}
