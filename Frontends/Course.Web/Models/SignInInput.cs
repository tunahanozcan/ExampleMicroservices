using System.ComponentModel.DataAnnotations;

namespace Course.Web.Models
{
    public class SigninInput
    {
        [Required]
        [Display(Name = "Email Adresiniz")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Şifreniz")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Beni Hatırla")]
        public bool IsRemember { get; set; }
    }
}
