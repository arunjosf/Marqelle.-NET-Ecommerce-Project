using Marqelle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Interfaces
{
    public interface IAddressRepository
    {
        Task AddAddressAsync(Address address);
        Task UpdateAddressAsync(Address address);
        Task<List<Address>> GetUserAddressesAsync(long userId);
        Task<Address?> GetAddressByIdAsync(long addressId);
        Task DeleteAddressAsync(Address address);
    }
}
