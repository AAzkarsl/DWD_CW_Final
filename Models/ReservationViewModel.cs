namespace DWD_CW_Final.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ReservationViewModel
    {
        [Required]
        [Display(Name = "Guest Name")]
        public string GuestName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Guest Email")]
        public string GuestEmail { get; set; }

        [Display(Name = "Book")]
        public int? BookID { get; set; }

        [Display(Name = "CD")]
        public int? CDID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Reservation Date")]
        public DateTime ReservationDate { get; set; } = DateTime.Today;
    }
}
