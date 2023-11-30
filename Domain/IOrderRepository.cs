using SolforbOrdersTest.Dto;

namespace SolforbOrdersTest.Domain;

public interface IOrderRepository
{
    Task<IEnumerable<Order>> GetAllOrdersAsync();
    Task<Order> GetOrderByIdAsync(int id);
    Task CreateOrderAsync(Order order);
    Task UpdateOrderAsync(Order order);
    Task DeleteOrderAsync(int id);
    Task<IEnumerable<Order>> GetAllOrdersAsync(int? providerId, string[]? orderItemNames, DateTime? startDate,
        DateTime? endDate, string[]? itemUnits,
        int[]? orderNumbers);
    Task<IEnumerable<string>> GetDistinctOrderItems();
    Task<IEnumerable<Provider>> GetDistinctProviders();
    Task<IEnumerable<OrderNumber>> GetDistinctOrderNumbers();
    Task<IEnumerable<string>> GetDistinctUnitTypes();
    bool VerifyOrderNumber(string number, int providerId, int orderId);
}