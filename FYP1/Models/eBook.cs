using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FYP1.Models
{
    public class eBook
    {
        [Key]
        [Required, DisplayName("Book ID")]
        public int bookID { get; set; }

        [Required, StringLength(200), DisplayName("Name")]
        public string title { get; set; }

        [Required, StringLength(50), DisplayName("Type")]
        public string type { get; set; }

        [Required, StringLength(1000), DisplayName("Description")]
        public string description { get; set; }

        [Required]
        public float height { get; set; }

        [Required]
        public float width { get; set; }

        public string? background { get; set; }

        [Required, StringLength(50)]
        public string edition { get; set; } = "1.0";

        [Required, StringLength(20), DisplayName("Status")]
        public string bookStatus { get; set; } = "DRAFT";

        [DataType(DataType.Date), DisplayName("Last Modified")]
        public DateTime LastUpdate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string authorID { get; set; }

        //public User? User { get; set; }

        public ICollection<Version>? Versions { get; set; }

        public ICollection<Chapter>? Chapters { get; set; }

        public ICollection<Collaboration>? Collaborations { get; set; }

        [Timestamp]
        public byte[] ConcurrencyToken { get; set; }

    }
}
