namespace DWD_CW_Final.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Guest
    {
        [Key]
        public int GuestID { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }
    }
}
