using System.Linq;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.WebMS.Controllers
{
    [ApiController]
    [Route("api/user-audits")]
    public class UserAuditController : ControllerBase
    {
        private readonly IUserAuditService _auditService;

        public UserAuditController(IUserAuditService auditService)
        {
            _auditService = auditService;
        }

        [HttpGet("{userId}")]
        public IActionResult GetAuditsByUserId(long userId)
        {
            var audits = _auditService.GetAuditsForUser(userId);

            if (audits == null || !audits.Any())
                return Ok(new object[0]);

            return Ok(audits);
        }
    }
}
