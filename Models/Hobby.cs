using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace C_Exam.Models
{
    public class Hobby
    {
        [Required]
        public int HobbyId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public List<Association> Users { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
    }
}