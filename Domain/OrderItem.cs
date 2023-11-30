using System.ComponentModel.DataAnnotations;

namespace SolforbOrdersTest.Domain;

public class OrderItem
{
    [Required]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public decimal Quantity { get; set; }
    [Required]
    public string Unit { get; set; }
    [Required]
    public int OrderId { get; set; }
}