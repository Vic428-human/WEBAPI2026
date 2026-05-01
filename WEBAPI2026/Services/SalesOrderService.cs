using System.Collections.Generic;
using WEBAPI2026.Models.Dtos;
using WEBAPI2026.Models.Requests;
using WEBAPI2026.Repositories;

namespace WEBAPI2026.Services
{
    public class SalesOrderService
    {
        private readonly ISalesOrderRepository _salesOrderRepository;

        public SalesOrderService(ISalesOrderRepository salesOrderRepository)
        {
            _salesOrderRepository = salesOrderRepository;
        }

        public List<SalesOrderDto> GetSalesOrders(DateRangeRequest request)
        {
            return _salesOrderRepository.GetSalesOrders(request);
        }
    }
}