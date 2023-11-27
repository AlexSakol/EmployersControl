using System.ComponentModel.DataAnnotations;

namespace CRUDMVC.Models.Entities
{
    public class Employer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Имя не заполнено")]
        [StringLength(50, MinimumLength = 2, ErrorMessage ="Длина стоки должна быть от 2 до 50 символов")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Должность не заполнена")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Длина стоки должна быть от 2 до 50 символов")]
        public string Post { get; set; }

        [Required(ErrorMessage = "Зарплата не указана")]        
        [Range (0.01, 50000.00, ConvertValueInInvariantCulture = true, ErrorMessage="Диапазон зарплаты от 0.01 по 50 000")]
        [DataType(DataType.Currency)]        
        public decimal Salary { get; set; }     
        public int DepartmentId { get; set; }        
        
    }
}
