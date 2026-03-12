using Marqelle.Application.DTO;
using Marqelle.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Interfaces
{
    public interface IAdminOrderService
    {
        Task UpdateOrderStatusAsync(long orderId, OrderStatus newStatus);
        Task<List<AdminOrderHistoryDto>> GetAllOrdersAsync();
        Task<List<AdminOrderHistoryDto>> SearchOrdersAsync(long? orderId, long? productId, long? userId);
    }
}
