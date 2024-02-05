using System.ComponentModel.DataAnnotations;

namespace FYP1.Models
{
    public class BookPage
    {
        [Key]
        public int bookPageID { get; set; }

        [Required]
        public int pageNo { get; set; }

        [Required]
        public bool pageLock { get; set; } = false;

        [Required]
        public int chapterID { get; set; }

        public Chapter? Chapter { get; set; }

        public ICollection<Element>? Elements { get; set; }

        public ICollection<Comment>? Comments { get; set; }

        [Timestamp]
        public byte[] ConcurrencyToken { get; set; }
    }
}
