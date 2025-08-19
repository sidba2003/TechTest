using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserManagement.Models
{
    public class UserAudit
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long UserId { get; set; } 
        public string Operation { get; set; } = default!;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // json of before / after states
        public string? DataBefore { get; set; }
        public string? DataAfter { get; set; }
    }
}
