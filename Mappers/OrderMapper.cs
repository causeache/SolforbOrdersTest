using SolforbOrdersTest.Domain;
using SolforbOrdersTest.Models;

namespace SolforbOrdersTest.Mappers
{
    public static class OrderMapperExtensions
    {
        public static Order ToOrder(this OrderCreateViewModel vm)
        {
            return new Order()
            {
                Number = vm.Number,
                Date = vm.Date,
                ProviderId = vm.ProviderId,
                OrderItems = vm.OrderItems.Select(o => o.ToOrderItem()).ToList()
            };
        }

        public static Order ToOrder(this OrderEditViewModel vm)
        {
            return new Order()
            {
                Id = vm.Id,
                Number = vm.Number,
                Date = vm.Date,
                ProviderId = vm.ProviderId,
                OrderItems = vm.OrderItems.Select(o => o.ToOrderItem()).ToList()
            };
        }

        public static OrderItem ToOrderItem(this OrderItemViewModel vm)
        {
            return new OrderItem()
            {
                Id = vm.Id,
                Name = vm.Name,
                Quantity = vm.Quantity,
                Unit = vm.Unit
            };
        }

        public static OrderItemViewModel ToOrderItemViewModel(this OrderItem item)
        {
            return new OrderItemViewModel()
            {
                Id = item.Id,
                Name = item.Name,
                Quantity = item.Quantity,
                Unit = item.Unit
            };
        }

        public static OrderViewModel ToOrderViewModel(this Order order)
        {
            return new OrderViewModel()
            {
                Id = order.Id,
                Date = order.Date,
                ProviderName = order.Provider?.Name,
                Number = order.Number,
                OrderItems = order.OrderItems.Select(i => i.ToOrderItemViewModel())
            };
        }

        public static OrderEditViewModel ToOrderEditViewModel(this Order order)
        {
            return new OrderEditViewModel()
            {
                Id = order.Id,
                Date = order.Date,
                ProviderId = order.ProviderId,
                Number = order.Number,
                OrderItems = order.OrderItems.Select(i => i.ToOrderItemViewModel()).ToList(),
            };
        }

        public static OrderDetailsViewModel ToOrderDetailsViewModel(this Order order)
        {
            return new OrderDetailsViewModel()
            {
                Id = order.Id,
                Date = order.Date,
                ProviderName = order.Provider.Name,
                Number = order.Number,
                OrderItems = order.OrderItems.Select(i => i.ToOrderItemViewModel()).ToList()
            };
        }


    }
}
