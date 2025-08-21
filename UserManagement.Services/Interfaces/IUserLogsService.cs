using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces
{
    public interface IUserLogsService
    {
        Task<IEnumerable<UserLogs>> GetAllAsync();
        Task<IEnumerable<UserLogs>> GetAuditsForUserAsync(long userId);
        Task AddAuditAsync(UserLogs audit);
    }
}
