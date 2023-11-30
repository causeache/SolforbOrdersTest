using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SolforbOrdersTest.Models;

public class OrderCreateViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Заполните номер заказа")]
    [DisplayName("Номер заказа")]
    public string Number { get; set; }

    [Required(ErrorMessage = "Заполните дату заказа")]
    [DisplayName("Дата заказа")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
    public DateTime Date { get; set; }

    [Required(ErrorMessage = "Выберите поставщика")]
    [DisplayName("Поставщик")]
    public int ProviderId { get; set; }
        
    [DisplayName("Товары")]
    [MinLength(1, ErrorMessage = "Добавьте товары в заказ")]
    public List<OrderItemViewModel> OrderItems { get; set; }

    public SelectList? Providers { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        for (var i = 0; i < OrderItems.Count; i++)
        {
            if (OrderItems[i].Name == Number)
            {
                yield return new ValidationResult("Имя товара не должно совпадать с номером заказа",
                    new[] { nameof(Number), $"{nameof(OrderItems)}[{i}].{nameof(OrderItemViewModel.Name)}" });
            }
        }
    }
}