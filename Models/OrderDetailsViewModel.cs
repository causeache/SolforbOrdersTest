using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SolforbOrdersTest.Models;

public class OrderDetailsViewModel
{
    public int Id { get; set; }
    
    [DisplayName("Номер заказа")]
    public string Number { get; set; }
    
    [DisplayName("Дата заказа")]
    [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}")]
    public DateTime Date { get; set; }
    
    [DisplayName("Поставщик")]
    public string ProviderName { get; set; }
    
    [DisplayName("Товары")]
    public List<OrderItemViewModel> OrderItems { get; set; }
}