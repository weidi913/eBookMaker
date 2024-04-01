using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FYP1.Models
{
    public class Comment
    {
        [Key]
        public int commentID { get; set; }
        [Required,StringLength(1000)]
        public string comment { get; set; }

        [DataType(DataType.Date), DisplayName("Version Date")]
        public DateTime commentDate { get; set; } = DateTime.Now;

        [Required]//maybe need add default status
        public bool commentStatus { get; set; } = false;

        [Required,StringLength(256)]
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
