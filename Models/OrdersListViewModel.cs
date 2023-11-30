using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;

namespace SolforbOrdersTest.Models;

public class OrdersListViewModel
{
    [DisplayName("Поставщики")]
    public MultiSelectList Providers { get; set; }
    
    [DisplayName("Заказы")]
    public MultiSelectList OrderNumbers { get; set; }
    
    [DisplayName("Единицы измерения")]
    public MultiSelectList ItemUnits { get; set; }
    
    [DisplayName("Товары")]
    public MultiSelectList OrderItems { get; set; }
    
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public IEnumerable<int> OrderNumbersIds { get; set; }
    public IEnumerable<string> OrderItemNames { get; set; }
    public IEnumerable<string> ItemUnitsNames { get; set; }
    public IEnumerable<int> ProviderIds { get; set; }
    public IEnumerable<OrderViewModel> Orders { get;set;}
}