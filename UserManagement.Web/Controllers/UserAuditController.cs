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

        /// <summary>
        /// Get all audit logs for a specific user by userId.
        /// </summary>
        /// <param name="userId">The ID of the user to fetch audits for</param>
        [HttpGet("{userId}")]
        public IActionResult GetAuditsByUserId(long userId)
        {
            var audits = _auditService.GetAuditsForUser(userId);

            if (audits == null || !audits.Any())
                return NotFound($"No audit logs found for user with ID {userId}.");

            return Ok(audits);
        }
    }
}
