using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FYP1.Models
{
    public class eBook
    {
        [Key]
        [Required, DisplayName("Version ID")]
        public int bookID { get; set; }

        [Required, StringLength(200)]
        public string title { get; set; }

        [Required, StringLength(50)]
        public string type { get; set; }

        [Required, StringLength(1000)]
        public string description { get; set; }

        [Required]
        public float height { get; set; }

        [Required]
        public float width { get; set; }

        [Required]
        public string background { get; set; }

        [Required, StringLength(50)]
        public string edition { get; set; }

        [Required, StringLength(20)]
        public string bookStatus { get; set; }

        [DataType(DataType.Date), DisplayName("Last Edited")]
        public DateTime commentDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public int authorID { get; set; }

        //public User? User { get; set; }

        public ICollection<Version>? Versions { get; set; }

        public ICollection<Chapter> Chapters { get; set; }

        public ICollection<Collaboration>? Collaborations { get; set; }

    }
}
