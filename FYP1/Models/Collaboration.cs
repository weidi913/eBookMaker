using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FYP1.Models
{
    public class Collaboration
    {
        [Required, StringLength(450)]
        public string authorID { get; set; }
        [Required]
        public int bookID { get; set; }
        [ForeignKey("bookID")]
        public eBook? eBook { get; set; }
        public Member? User { get; set; }

        [Timestamp]
        public byte[]? ConcurrencyToken { get; set; }
    }
}
