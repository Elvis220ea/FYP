using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FYP.Models
{
    public class PHBooking
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "NRIC cannot be empty!")]
        [RegularExpression(@"[STFG]\d{7}[A-Z]", ErrorMessage = "Invalid NRIC format!")]
        public string NRIC { get; set; }

        [Required(ErrorMessage = "Owner name cannot be empty!")]
        public string OwnerName { get; set; }

        [Required(ErrorMessage = "Pet name cannot be empty!")]
        public string PetName { get; set; }

        [Required(ErrorMessage = "Days cannot be empty!")]
        [Range(1, 5, ErrorMessage = "Days must be between 1 and 5!")]
        public int Days { get; set; }

        [Required(ErrorMessage = "Pet Type Id cannot be empty!")]
        public int PetTypeId { get; set; }

        [Required(ErrorMessage = "Feeding Frequency cannot be empty!")]
        [Range(0, 2, ErrorMessage = "Feeding Frequency must be between 0 and 2!")]
        public int FeedFreq { get; set; }

        [Required(ErrorMessage = "Check In date cannot be empty!")]
        [DataType(DataType.Date, ErrorMessage ="Invalid date format!")]
        public DateTime CheckInDate { get; set; }

        public int? BookedBy { get; set; }

        public bool FTCanned { get; set; }
        public bool FTDry { get; set; }
        public bool FTSoft { get; set; }

        public double Cost { get; set; }

        // fields added for reporting purpose
        public string BookedByName { get; set; }
        public string PetType { get; set; }
    }
}
