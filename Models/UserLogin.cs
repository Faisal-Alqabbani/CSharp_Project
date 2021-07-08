using System.ComponentModel.DataAnnotations;

namespace C_Exam.Models
{
    public class UserLogin
    {
        [Required]
    
        [Display(Name = "Username")]
        public string UsernameLogin{ get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string PasswordLogin{ get; set; } 
    }
}