using System.ComponentModel.DataAnnotations;

namespace AwesomeNetwork.ViewModels.Account
{
    public class LoginViewModel
    {
        //[Required]
        //[Display(Name = "Email")]
        //public string Email { get; set; }

        //[Required]
        //[Display(Name = "Пароль")]
        //public string Password { get; set; }

        //public string ReturnUrl { get; set; }

        //public bool RememberMe { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email", Prompt = "Введите email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль", Prompt = "Введите пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}