using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FYP1.Models
{
    public class Comment
    {
        [Key]
        [Required]
        public int commentID {get; set;}

        [Required,StringLength(1000)]
        public int comment { get; set; }

        [DataType(DataType.Date), DisplayName("Version Date")]
        public DateTime commentDate { get; set; } = DateTime.Now;

        [Required]//maybe need add default status
        public bool commentStatus { get; set; }

        [Required,StringLength(50)]
        public string authorID { get; set; }

        [Required]
        public string bookPageID { get; set; }


        public BookPage? Page { get; set; }

        //public User? User { get; set; }

        [Timestamp]
        public byte[]? ConcurrencyToken { get; set; }
    }
}
