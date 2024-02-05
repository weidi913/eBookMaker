using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FYP1.Models
{
    public class Collaboration
    {
        [Key]
        [Required, DisplayName("Collaboration ID")]
        public int collabID { get; set; }

        [Required, StringLength(50)]
        public string authorID { get; set; }


        public int bookID { get; set; }

        public eBook? eBook { get; set; }
        //public User? User { get; set; }

        [Timestamp]
        public byte[] ConcurrencyToken { get; set; }
    }
}
