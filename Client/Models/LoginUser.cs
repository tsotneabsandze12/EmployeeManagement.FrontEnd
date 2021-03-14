using System.ComponentModel.DataAnnotations;

namespace Client.Models
{
    public class LoginUser
    {
        [Required]
        [EmailAddress]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$")]
        public string Email { get; set; }


        [Required] 
        [MinLength(6)]
        public string Password { get; set; }
    }
}