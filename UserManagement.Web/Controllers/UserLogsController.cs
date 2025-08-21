using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.WebMS.Controllers
{
    [ApiController]
    [Route("api/user-audits")]
    public class UserLogsController : ControllerBase
    {
        private readonly IUserLogsService _auditService;
        public UserLogsController(IUserLogsService auditService) => _auditService = auditService;

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetAuditsByUserId(long userId)
        {
            var audits = await _auditService.GetAuditsForUserAsync(userId);
            if (audits == null || !audits.Any()) return Ok(System.Array.Empty<object>());
            return Ok(audits);
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var items = await _auditService.GetAllAsync();
            return Ok(items);
        }
    }
}
