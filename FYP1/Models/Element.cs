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

        [StringLength(5000)]
        public string? elementStyle { get; set; }

        //Suppose not nullable
        public string? z_index { get; set; }

        
        public string? text { get; set; }

        [Required]
        public int bookPageID { get; set; }

        public BookPage? BookPage { get; set; }

        [Timestamp]
        public byte[]? ConcurrencyToken { get; set; }
    }
}
