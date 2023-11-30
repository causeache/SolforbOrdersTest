using System.ComponentModel;

namespace SolforbOrdersTest.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        
        [DisplayName("Номер заказа")]
        public string Number { get; set; }
        
        [DisplayName("Дата заказа")]
        public DateTime Date { get; set; }

        [DisplayName("Поставщик")]
        public string ProviderName { get; set; }

        [DisplayName("Товары")]
        public IEnumerable<OrderItemViewModel> OrderItems { get; set; }

    }
}
