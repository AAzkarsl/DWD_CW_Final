namespace DWD_CW_Final.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Message
    {
        [Key]
        public int MessageID { get; set; }

        [Required]
        public int GuestID { get; set; }
        public Guest Guest { get; set; }

        [Required]
        [StringLength(1000)]
        public string MessageText { get; set; }

        [DataType(DataType.Date)]
        public DateTime MessageDate { get; set; }
    }
}
