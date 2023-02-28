using System.ComponentModel.DataAnnotations;

namespace SchoolWebRegister.Web.ViewModels.Account
{
    public sealed class RegisterViewModel
    {
        [Required(ErrorMessage = "Введите логин")]
        [MaxLength(20, ErrorMessage = "Логин должен иметь длину меньше 15 символов")]
        [MinLength(5, ErrorMessage = "Логин должен иметь длину больше 5 символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [Display(Name = "Пароль")]
        [MaxLength(30, ErrorMessage = "Пароль должен иметь длину меньше 30 символов")]
        [MinLength(5, ErrorMessage = "Пароль должен иметь длину больше 5 символов")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [Display(Name = "Пароль")]
        [MaxLength(30, ErrorMessage = "Пароль должен иметь длину меньше 30 символов")]
        [MinLength(5, ErrorMessage = "Пароль должен иметь длину больше 5 символов")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
