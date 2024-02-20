using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FYP1.Models
{
    public class Member : IdentityUser
    {
        public string? firstName { get; set; }
        public string? lastName { get; set; }
/*        public Role role { get; set; } = Role.Memeber;
*/        [NotMapped, Display(Name = "Image")]
        public IFormFile? imageFile { get; set; }
        public string? imageData { get; set; }
        public string gender { get; set; }
        public DateTime birthday { get; set; }
/*        [Timestamp]
        public byte[] ConcurrencyToken { get; set; }*/
    }

    public enum MemeberStatus
    {
        Submitted,
        Approved,
        Rejected
    }

    public enum Role
    {
        Admin,
        Moderator,
        Reviewer,
        Memeber,
        Guest
    }
}
