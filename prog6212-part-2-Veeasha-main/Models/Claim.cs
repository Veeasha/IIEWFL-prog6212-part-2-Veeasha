using System;

namespace PROG6212.Models
{
    public class Claim
    {
        public int ClaimID { get; set; } // Unique identifier for the claim
        public string? Lecturer { get; set; } // Name of the lecturer
        public DateTime Date { get; set; } // Date of the claim
        public int HoursWorked { get; set; } // Hours worked
        public double HourlyRate { get; set; } // Hourly rate
        public double TotalAmount { get; set; } // Total amount calculated from hours worked and hourly rate
        public string Status { get; set; } // Status of the claim (e.g., Pending, Approved, Rejected)
        public string AdditionalNotes { get; set; } // Any additional notes provided by the lecturer

        public string SupportingDocument { get; set; }

    }
}
