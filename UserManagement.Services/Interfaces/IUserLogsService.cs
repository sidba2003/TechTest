using System.Collections.Generic;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces
{
    public interface IUserLogsService
    {
        IEnumerable<UserLogs> GetAll();
        IEnumerable<UserLogs> GetAuditsForUser(long userId);
        void AddAudit(UserLogs audit);
    }
}
