using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SolforbOrdersTest.Domain;
using SolforbOrdersTest.Dto;
using SolforbOrdersTest.Infrastructure.DB;

namespace SolforbOrdersTest.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _dbContext;

        public OrderRepository(OrderDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            var orders = await _dbContext.Orders
                .Include(o => o.Provider)
                .Include(o => o.OrderItems)
                .ToListAsync();
            return orders;
        }

        public async Task<Order> GetOrderByIdAsync(int id)
        {
            var order = await _dbContext.Orders
                .Include(o => o.Provider)
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == id);
            return order;
        }

        public async Task CreateOrderAsync(Order order)
        {
            if (!ValidateOrder(order))
            {
                throw new Exception("Invalid order");
            }
            
            await _dbContext.AddAsync(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            if (!ValidateOrder(order))
            {
                throw new Exception("Invalid order");
            }

            var transaction = await _dbContext.Database.BeginTransactionAsync();
            try
            {
                var currentItems = _dbContext.OrderItems.Where(i => i.OrderId == order.Id);
                var updatedItemsIds = order.OrderItems.Select(o => o.Id);
                var itemsToDelete = currentItems.Where(i => !updatedItemsIds.Contains(i.Id));
                if (!itemsToDelete.IsNullOrEmpty())
                {
                    _dbContext.OrderItems.RemoveRange(itemsToDelete);
                    await _dbContext.SaveChangesAsync();
                }
                _dbContext.Orders.Update(order);
                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await _dbContext.Orders.FindAsync(id);
            _dbContext.Orders.Remove(order);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(int? provider, string[]? orderItemNames, DateTime? startDate,
            DateTime? endDate, string[]? itemUnits,
            int[]? orderNumbers)
        {
            var query = _dbContext.Orders.AsQueryable();

            if (provider != null)
                query = query.Where(o => o.ProviderId == provider);

            if (!orderItemNames.IsNullOrEmpty())
                query = query.Where(o => o.OrderItems.Any(item => orderItemNames.Contains(item.Name)));

            if (startDate != null && endDate != null && endDate >= startDate)
            {
                query = query.Where(o => o.Date >= startDate && o.Date < endDate.Value.AddDays(1));
            }

            if (!itemUnits.IsNullOrEmpty())
            {
                query = query.Where(o => o.OrderItems.Any(item => itemUnits.Contains(item.Unit)));
            }

            if (!orderNumbers.IsNullOrEmpty())
            {
                query = query.Where(o => orderNumbers.Contains(o.Id));
            }

            var orders = await query
                .Include(o => o.Provider)
                .Include(o => o.OrderItems)
                .ToListAsync();
            return orders;
        }

        public async Task<IEnumerable<string>> GetDistinctOrderItems()
        {
            return await _dbContext.OrderItems.Select(i => i.Name).Distinct().ToListAsync();
        }

        public async Task<IEnumerable<Provider>> GetDistinctProviders()
        {
            return await _dbContext.Providers.Distinct().ToListAsync();
        }

        public async Task<IEnumerable<OrderNumber>> GetDistinctOrderNumbers()
        {
            return await _dbContext.Orders.Distinct().Select(o => new OrderNumber()
            {
                Id = o.Id,
                Number = o.Number,
            }).ToListAsync();
        }

        public async Task<IEnumerable<string>> GetDistinctUnitTypes()
        {
            return await _dbContext.OrderItems.Select(i => i.Unit).Distinct().ToListAsync();
        }

        public bool VerifyOrderNumber(string number, int providerId, int orderId)
        {
            return !_dbContext.Orders.Any(o => o.Number == number && o.ProviderId == providerId && o.Id!=orderId);
        }

        private bool ValidateOrder(Order order)
        {
            if (order.OrderItems.IsNullOrEmpty())
                return false;
            if (order.OrderItems.Any(i => i.Name == order.Number))
                return false;
            if(!VerifyOrderNumber(order.Number, order.ProviderId,order.Id)) return false;

            return true;
        }
    }

}
