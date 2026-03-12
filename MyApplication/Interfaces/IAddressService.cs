using Marqelle.Application.DTO;
using Marqelle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Interfaces
{
    public interface IAddressService
    {
        Task AddAddress(long userId, AddressDto dto);
        Task<List<AddressDto>> GetUserAddress(long userId);
        Task UpdateAddressAsync(long addressId, long userId, AddressDto dto);
        Task DeleteAddress(long addressId);
        Task SetDefaultAddressAsync(long addressId, long userId);
    }
}
