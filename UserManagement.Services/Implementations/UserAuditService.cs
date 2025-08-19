using System.Collections.Generic;
using System.Linq;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations
{
    public class UserAuditService : IUserAuditService
    {
        private readonly IDataContext _dataContext;

        public UserAuditService(IDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IEnumerable<UserAudit> GetAuditsForUser(long userId)
        {
            return _dataContext.GetAll<UserAudit>()
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .ToList();
        }

        public void AddAudit(UserAudit audit)
        {
            if (audit == null)
                throw new System.ArgumentNullException(nameof(audit));

            _dataContext.Create(audit);
        }
    }
}
