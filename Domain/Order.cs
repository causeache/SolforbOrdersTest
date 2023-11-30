using System.ComponentModel.DataAnnotations;

namespace SolforbOrdersTest.Domain
{
    public class Order
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Number { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int ProviderId { get; set; }
        [Required]
        public Provider? Provider { get; set; }
        [Required]
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
