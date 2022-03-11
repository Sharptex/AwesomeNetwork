using System;
using System.ComponentModel.DataAnnotations;

namespace AwesomeNetwork.ViewModels.Account
{
    public class UserEditViewModel
    {
        [Required]
        [Display(Name = "Аватар", Prompt = "Введите адрес аватара")]
        public string Image { get; set; }

        [Required]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; }

        [Required]
        [Display(Name = "Имя", Prompt = "Введите имя")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Дата рождения")]
        public DateTime BirthDate { get; set; }

        [Required]
        [Display(Name = "Никнейм")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Статус")]
        public string Status { get; set; }

        [Required]
        [Display(Name = "О себе")]
        public string About { get; set; }

        public string UserId { get; set; }
    }
}
