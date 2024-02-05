using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FYP1.Models
{
    public class Member : IdentityUser
    {
        public int? MemberID { get; set; }

        // user ID from AspNetUser table.
        public string? OwnerID { get; set; }

        [DataType(DataType.EmailAddress)]
        public string? email123 { get; set; }
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? phoneNo { get; set; }
        public string? street { get; set; }
        public string? city { get; set; }
        public string? state { get; set; }
        public string? zipcode { get; set; }
        public string? country { get; set; }
/*        public Role role { get; set; } = Role.Memeber;
*/        [NotMapped, Display(Name = "Image")]
        public IFormFile? imageFile { get; set; }
        public string? imageData { get; set; }

        public MemeberStatus? Status { get; set; }

        [Timestamp]
        public byte[] ConcurrencyToken { get; set; }
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
