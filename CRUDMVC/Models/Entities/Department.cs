using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;

namespace CRUDMVC.Models.Entities
{
    public class Department
    {
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Отдел не указан")]
        [StringLength(55, MinimumLength = 3, ErrorMessage = "Количество символов от 3 до 55")]
        public string Name { get; set; }

    }
}
