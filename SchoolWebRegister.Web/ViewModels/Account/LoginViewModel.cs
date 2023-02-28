using System.ComponentModel.DataAnnotations;

namespace SchoolWebRegister.Web.ViewModels.Account
{
    public sealed class LoginViewModel
    {
        [Required(ErrorMessage = "Введите логин")]
        [Display(Name = "Логин")]
        [MaxLength(20, ErrorMessage = "Логин должен иметь длину меньше 15 символов")]
        [MinLength(5, ErrorMessage = "Логин должен иметь длину больше 5 символов")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [Display(Name = "Пароль")]
        [MaxLength(30, ErrorMessage = "Пароль должен иметь длину меньше 30 символов")]
        [MinLength(5, ErrorMessage = "Пароль должен иметь длину больше 5 символов")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

        public bool RememberMe { get; set; }
    }
}
