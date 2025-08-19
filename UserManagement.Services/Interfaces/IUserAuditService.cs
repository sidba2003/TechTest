using System.Collections.Generic;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces
{
    public interface IUserAuditService
    {
        IEnumerable<UserAudit> GetAuditsForUser(long userId);
        void AddAudit(UserAudit audit);
    }
}
