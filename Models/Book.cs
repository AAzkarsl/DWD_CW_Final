namespace DWD_CW_Final.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Book
    {
        [Key]
        public int BookID { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; }

        [Required]
        [StringLength(255)]
        public string Author { get; set; }

        [Required]
        [StringLength(13)]
        public string ISBN { get; set; }

        public bool Available { get; set; }
    }
}
