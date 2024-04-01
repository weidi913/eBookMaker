using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FYP1.Models
{
    public class Version
    {
        [Key]
        [Required, DisplayName("Version ID")]
        public int versionID { get; set; }

        [Required, StringLength(100)]
        public string verName { get; set; }

        [Required]
        public string verContent { get; set; }

        [DataType(DataType.Date), DisplayName("Version Date")]
        public DateTime versionDate { get; set; } = DateTime.Now;
        [Required]
        public int bookID { get; set; }
        [ForeignKey("bookID")]
        public eBook? eBook { get; set; }

        [Timestamp]
        public byte[]? ConcurrencyToken { get; set; }

    }
}
