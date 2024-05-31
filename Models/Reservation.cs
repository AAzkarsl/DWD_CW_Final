namespace DWD_CW_Final.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Reservation
    {
        [Key]
        public int ReservationID { get; set; }

        public int? GuestID { get; set; }
        public Guest? Guest { get; set; }

        public int? BookID { get; set; }
        public Book? Book { get; set; }

        public int? CDID { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReservationDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? IssueDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }

        public string? Status { get; set; } = "Reserved"; // Set default status to "Reserved"
    }
}
