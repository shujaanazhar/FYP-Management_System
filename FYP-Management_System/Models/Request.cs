using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FYP_Management_System.Models
{
    public class Request
    {
        [Key]
        [ForeignKey("Supervisor")]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public bool Status { get; set; }



        public Request()
        {
            // Set default value for Status
            Status = false;
        }
    }
}