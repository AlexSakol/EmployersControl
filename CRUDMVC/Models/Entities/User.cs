using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CRUDMVC.Models.Entities
{
    public class User
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Логин не указан")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина логина должна быть от 3 до 50 символов")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Логин должен содержать только латиницу")]
        
        public string Login { get; set; }

        [Required(ErrorMessage = "Пароль не указан")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,50}$", ErrorMessage = "Пароль должен содержать только латиницу, хотя бы одну цифру, хотя бы одну заглавную букву, хотя бы один спецсимвол, длина от 8 до 50 символов")]
        [DataType(DataType.Password)]
        public string Password { get; set; }   
        public int RoleId { get; set; }

    }
}
