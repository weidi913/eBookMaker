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
        public string backgroundStyle { get; set; } = "";

        public Chapter? Chapter { get; set; }

        public IList<Element>? Elements { get; set; }

        public IList<Comment>? Comments { get; set; }

        [Timestamp]
        public byte[]? ConcurrencyToken { get; set; }
    }
}
