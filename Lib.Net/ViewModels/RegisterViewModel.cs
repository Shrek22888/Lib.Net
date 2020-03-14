using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lib.Net.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Email не указан")]
        [EmailAddress(ErrorMessage ="Email введен неверно")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Группа не указана")]
        public string Group { get; set; }
        [Required(ErrorMessage ="Имя не указано")]
        public string Name { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Пароль не указан")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Повторите пароль")]
        [Compare("Password", ErrorMessage ="Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

    }
}
