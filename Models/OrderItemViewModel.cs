using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SolforbOrdersTest.Models;

public class OrderItemViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage ="Заполните наименование товара")]    
    [DisplayName("Наименование товара")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Заполните количество")]
    [Range(0, double.MaxValue, ErrorMessage ="Количество должно быть больше 0")]
    [DisplayName("Количество")]
    public decimal Quantity { get; set; }

    [Required(ErrorMessage = "Заполните единицы измерения")]
    [DisplayName("Единица измерения")] 
    public string Unit { get; set; }
}

