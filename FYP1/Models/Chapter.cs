using System.ComponentModel.DataAnnotations;

namespace FYP1.Models
{
    public class Chapter
    {
        [Key]
        public int chapterID { get; set; }

        [Required, StringLength(100)]
        public string chapterName { get; set; }

        [Required]
        public int chapterNo { get; set; }

        [Required]
        public int bookID { get; set; }

        public eBook? book { get; set; }

        public ICollection<BookPage>? BookPages { get; set; }

        [Timestamp]
        public byte[]? ConcurrencyToken { get; set; }

    }
}
