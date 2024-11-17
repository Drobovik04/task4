using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace task4.Models
{
    public class CustomUser : IdentityUser
    {
        [Required]
        [MaxLength(10)]
        public string Status { get; set; } = "Active";

        public DateTime? LastLoginTime { get; set; }
    }
}
