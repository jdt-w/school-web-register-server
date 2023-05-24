using System.ComponentModel.DataAnnotations;

namespace SchoolWebRegister.Web.ViewModels.Account
{
    public sealed class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Введите пароль")]
        [Display(Name = "Пароль")]
        [MaxLength(30, ErrorMessage = "Пароль должен иметь длину меньше 30 символов")]
        [MinLength(5, ErrorMessage = "Пароль должен иметь длину больше 5 символов")]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [Required(ErrorMessage = "Введите пароль")]
        [Display(Name = "Пароль")]
        [MaxLength(30, ErrorMessage = "Пароль должен иметь длину меньше 30 символов")]
        [MinLength(5, ErrorMessage = "Пароль должен иметь длину больше 5 символов")]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Подтвердите пароль")]
        [Display(Name = "Пароль")]
        [MaxLength(30, ErrorMessage = "Пароль должен иметь длину меньше 30 символов")]
        [MinLength(5, ErrorMessage = "Пароль должен иметь длину больше 5 символов")]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
