using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marqelle.Application.DTO;

namespace Marqelle.Application.Interfaces
{
    public interface IUserOrderService
    {
        Task<List<UserOrderHistoryDto>> GetUserOrdersAsync(long userId);
        Task<UserOrderResponseDto> PlaceOrderAsync(long userId, PlaceOrderDto dto);
        Task CancelOrderAsync(long orderId, long userId);
    }
}
