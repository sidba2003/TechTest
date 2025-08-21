using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations
{
    public class UserLogsService : IUserLogsService
    {
        private readonly IDataContext _dataContext;
        public UserLogsService(IDataContext dataContext) => _dataContext = dataContext;

        public async Task<IEnumerable<UserLogs>> GetAllAsync()
            => await _dataContext.GetAll<UserLogs>().OrderByDescending(a => a.Timestamp).ToListAsync();

        public async Task<IEnumerable<UserLogs>> GetAuditsForUserAsync(long userId)
            => await _dataContext.GetAll<UserLogs>()
                                 .Where(a => a.UserId == userId)
                                 .OrderByDescending(a => a.Timestamp)
                                 .ToListAsync();

        public async Task AddAuditAsync(UserLogs audit)
        {
            if (audit == null) throw new System.ArgumentNullException(nameof(audit));
            await _dataContext.CreateAsync(audit);
        }
    }
}
