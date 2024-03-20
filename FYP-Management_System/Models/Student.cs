﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexGen.Models
{
    public class Student
    {
        [Key]
        [ForeignKey("User")]
        public int Id { get; set; }

        [Required]
        public int Batch { get; set; }

        [Required]
        [StringLength(3)]
        public string Department { get; set; }

        public string? FYP_Name { get; set; }

        [Required]
        public float CGPA { get; set; }

        // Navigation property to FYP
        public virtual FYP FYP { get; set; }
    }
}