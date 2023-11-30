using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using SolforbOrdersTest.Domain;
using SolforbOrdersTest.Dto;
using SolforbOrdersTest.Mappers;
using SolforbOrdersTest.Models;

namespace SolforbOrdersTest.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly TimeSpan _cacheTimeSpan = TimeSpan.FromSeconds(10);

        private const string ProvidersCacheKey = "providers";
        private const string OrderItemsCacheKey = "orderItems";
        private const string OrderNumbersKey = "orderNumbers";
        private const string UnitsKey = "units";

        public OrderController(IOrderRepository orderRepository, IMemoryCache memoryCache)
        {
            _orderRepository = orderRepository;
            _memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index(int? providerIds,
            string[]? orderItemNames,
            DateTime? startDate,
            DateTime? endDate,
            string[]? itemUnitsNames,
            int[]? orderNumbersIds)
        {

            var providersSelectList = await PopulateProvidersMultiSelectList();
            var orderItemsSelectList = await PopulateOrderItemsMultiSelectList();
            var orderNumbersSelectList = await PopulateOrderNumbersMultiSelectList();
            var itemUnitsSelectList = await PopulateItemUnitsMultiSelectList();

            var orders = await _orderRepository.GetAllOrdersAsync(providerIds,
                orderItemNames?.Where(x => x != null).ToArray(),
                startDate, endDate,
                itemUnitsNames?.Where(x => x != null).ToArray(),
                orderNumbersIds);

            var vm = new OrdersListViewModel()
            {
                StartDate = DateTime.Now.AddMonths(-1),
                EndDate = DateTime.Now,
                Providers = providersSelectList,
                OrderItems = orderItemsSelectList,
                OrderNumbers = orderNumbersSelectList,
                ItemUnits = itemUnitsSelectList,
                Orders = orders.Select(o => o.ToOrderViewModel()).OrderBy(o => o.Date)
            };

            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            var providers = await PopulateProvidersSelectList();
            var vm = new OrderCreateViewModel
            {
                Date = DateTime.Now,
                OrderItems = new List<OrderItemViewModel>()
                {
                    new OrderItemViewModel()
                },
                Providers = providers
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OrderCreateViewModel vm)
        {

            ValidateOrderNumberProviderIsUnique(vm.Number, vm.ProviderId, 0, nameof(vm.Number));

            if (!ModelState.IsValid)
            {
                vm.Providers = await PopulateProvidersSelectList();
                return View(vm);
            }

            var order = vm.ToOrder();
            await _orderRepository.CreateOrderAsync(order);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            var vm = order.ToOrderDetailsViewModel();
            return View(vm);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            var vm = order.ToOrderEditViewModel();
            vm.Providers = await PopulateProvidersSelectList();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(OrderEditViewModel vm)
        {
            ValidateOrderNumberProviderIsUnique(vm.Number, vm.ProviderId, vm.Id, nameof(vm.Number));

            if (!ModelState.IsValid)
            {
                vm.Providers = await PopulateProvidersSelectList();
                return View(vm);
            }

            var order = vm.ToOrder();
            await _orderRepository.UpdateOrderAsync(order);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            var order = await _orderRepository.GetOrderByIdAsync((int)id);
            return View(order.ToOrderViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _orderRepository.DeleteOrderAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<SelectList> PopulateProvidersSelectList()
        {
            var providers = await _orderRepository.GetDistinctProviders();

            var providersSelectList =
                new List<SelectListItem>()
                {
                    new SelectListItem("-- Выберите поставщика --", "")
                };
            providersSelectList.AddRange(providers.Select(p => new SelectListItem(p.Name, p.Id.ToString())));
            return new SelectList(providersSelectList, nameof(SelectListItem.Value), nameof(SelectListItem.Text));
        }

        private async Task<MultiSelectList> PopulateItemUnitsMultiSelectList()
        {
            if (!_memoryCache.TryGetValue<IEnumerable<string>>(UnitsKey, out var units))
            {
                units = await _orderRepository.GetDistinctUnitTypes();
                _memoryCache.Set(UnitsKey, units, _cacheTimeSpan);
            }
            var itemUnitsSelectList = new MultiSelectList(units?.OrderBy(u => u));
            return itemUnitsSelectList;
        }

        private async Task<MultiSelectList> PopulateOrderNumbersMultiSelectList()
        {
            if (!_memoryCache.TryGetValue<IEnumerable<OrderNumber>>(OrderNumbersKey, out var orderNumbers))
            {
                orderNumbers = await _orderRepository.GetDistinctOrderNumbers();
                _memoryCache.Set(OrderNumbersKey, orderNumbers, _cacheTimeSpan);
            }
            var orderNumbersSelectList = new MultiSelectList(orderNumbers?.OrderBy(o => o.Number),
                nameof(OrderNumber.Id), nameof(OrderNumber.Number));
            return orderNumbersSelectList;
        }

        private async Task<MultiSelectList> PopulateOrderItemsMultiSelectList()
        {
            if (!_memoryCache.TryGetValue<IEnumerable<string>>(OrderItemsCacheKey, out var items))
            {
                items = await _orderRepository.GetDistinctOrderItems();
                _memoryCache.Set(OrderItemsCacheKey, items, _cacheTimeSpan);
            }

            var orderItemsSelectList = new MultiSelectList(items?.OrderBy(i => i));
            return orderItemsSelectList;
        }

        private async Task<MultiSelectList> PopulateProvidersMultiSelectList()
        {
            if (!_memoryCache.TryGetValue<IEnumerable<Provider>>(ProvidersCacheKey, out var providers))
            {
                providers = await _orderRepository.GetDistinctProviders();
                _memoryCache.Set(ProvidersCacheKey, providers, _cacheTimeSpan);
            }

            var providersSelectList =
                new MultiSelectList(providers?.OrderBy(i => i.Name), nameof(Provider.Id), nameof(Provider.Name));
            return providersSelectList;
        }

        private void ValidateOrderNumberProviderIsUnique(string orderNumber, int providerId, int orderId, string errorKey)
        {
            if (!_orderRepository.VerifyOrderNumber(orderNumber, providerId, orderId))
            {
                ModelState.AddModelError(errorKey, $"Заказ этого поставщика с таким номером уже существует");
            }
        }
    }
}
