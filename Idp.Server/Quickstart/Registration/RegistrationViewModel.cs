using System.ComponentModel.DataAnnotations;

namespace Idp.Server.Quickstart.Registration
{
    public class RegistrationViewModel
    {
        [MaxLength(200)]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [MaxLength(200)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [MaxLength(200)]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(250)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        public string ReturnUrl { get; set; }
    }
}
