using System.Collections.Generic;
using System.Linq;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations
{
    public class UserLogsService : IUserLogsService
    {
        private readonly IDataContext _dataContext;

        public UserLogsService(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IEnumerable<UserLogs> GetAll()
        {
            return _dataContext.GetAll<UserLogs>().OrderByDescending(a => a.Timestamp).ToList();
        }

        public IEnumerable<UserLogs> GetAuditsForUser(long userId)
        {
            return _dataContext.GetAll<UserLogs>()
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .ToList();
        }

        public void AddAudit(UserLogs audit)
        {
            if (audit == null)
                throw new System.ArgumentNullException(nameof(audit));

            _dataContext.Create(audit);
        }
    }
}
