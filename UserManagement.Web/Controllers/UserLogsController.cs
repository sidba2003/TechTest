using System.Linq;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Services.Domain.Implementations;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.WebMS.Controllers
{
    [ApiController]
    [Route("api/user-audits")]
    public class UserLogsController : ControllerBase
    {
        private readonly IUserLogsService _auditService;

        public UserLogsController(IUserLogsService auditService)
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

        [HttpGet]
        public IActionResult List()
        {
            var items = _auditService.GetAll();

            return Ok(items);
        }
    }
}
