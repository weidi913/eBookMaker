using System.ComponentModel.DataAnnotations;

namespace FYP1.Models
{
    public class Element
    {
        [Key]
        public int elementID { get; set; }

        [Required, StringLength(50)]
        public string elementType { get; set; }

        [Required]
        public bool elementLock { get; set; } = false;

        [Required,StringLength(5000)]
        public string elementStyle { get; set; }

        [Required]
        public string z_index { get; set; }

        [Required]
        public string text { get; set; }

        [Required]
        public int bookPageID { get; set; }

        public BookPage? BookPage { get; set; }
    }
}
