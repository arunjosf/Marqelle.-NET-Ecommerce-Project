using Marqelle.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Interfaces
{
    public interface ICheckoutService
    {
        Task<CheckoutDto> GetCheckoutPageAsync(long userId);
    }
}
