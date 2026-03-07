using Marqelle.Application.DTO;
using Marqelle.Application.Interfaces;
using Marqelle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _repository;
        public AddressService(IAddressRepository repository)
        {
            _repository = repository;
        }
        public async Task AddAddress(long userId, AddressDto dto)
        {
            var address = new Address
            {
                UserId = userId,
                AddressType = dto.AddressType,
                FullName = dto.FullName,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Country = dto.Country,
                State = dto.State,
                City = dto.City,
                Pincode = dto.Pincode,
                FlatorHouseorBuildingName = dto.FlatorHouseorBuildingName,
                LandMark = dto.LandMark,
            };
            await _repository.AddAddressAsync(address);

        }

        public async Task DeleteAddress(long addressId)
        {
            var address = await _repository.GetAddressByIdAsync(addressId);

            if (address == null)
                throw new Exception("No address found");
            await _repository.DeleteAddressAsync(address);
        }

        public async Task<List<AddressDto>> GetUserAddress(long userId)
        {
            var addresses = await _repository.GetUserAddressesAsync(userId);

            return addresses.Select(a => new AddressDto
            {
                AddressId = a.Id,
                AddressType = a.AddressType,
                FullName = a.FullName,
                PhoneNumber = a.PhoneNumber,
                Email = a.Email,
                Country = a.Country,
                State = a.State,
                City = a.City,
                Pincode = a.Pincode,
                FlatorHouseorBuildingName = a.FlatorHouseorBuildingName,
                LandMark = a.LandMark
            }).ToList();
        }

        public async Task UpdateAddressAsync(long addressId,long userId, AddressDto dto)
        {
            var address = await _repository.GetAddressByIdAsync(addressId);

            if (address == null || address.UserId != userId)
                throw new Exception("Address not found or you are not authorized");

            address.AddressType = dto.AddressType;
            address.FullName = dto.FullName;
            address.PhoneNumber = dto.PhoneNumber;
            address.Email = dto.Email;
            address.Country = dto.Country;
            address.State = dto.State;
            address.City = dto.City;
            address.Pincode = dto.Pincode;
            address.FlatorHouseorBuildingName = dto.FlatorHouseorBuildingName;
            address.LandMark = dto.LandMark;

            await _repository.UpdateAddressAsync(address);

        }

        public async Task<AddressCheckoutDto?> GetCheckoutAddressAsync(long userId)
        {
            var addresses = await _repository.GetUserAddressesAsync(userId);

            var address = addresses.FirstOrDefault();

            if (address == null)
                return null;

            return new AddressCheckoutDto
            {
                AddressId = address.Id,
                FullName = address.FullName,
                AddressLine = $"{address.FlatorHouseorBuildingName}, {address.LandMark}",
                CityStatePincode = $"{address.City}, {address.State} {address.Pincode}",
                Country = address.Country,
                PhoneNumber = address.PhoneNumber
            };
        }

        public async Task<List<AddressCheckoutDto>> GetCheckoutAddressesAsync(long userId)
        {
            var addresses = await _repository.GetUserAddressesAsync(userId);

            return addresses.Select(a => new AddressCheckoutDto
            {
                AddressId = a.Id,
                FullName = a.FullName,
                AddressLine = $"{a.FlatorHouseorBuildingName}, {a.LandMark}",
                CityStatePincode = $"{a.City}, {a.State} {a.Pincode}",
                Country = a.Country,
                PhoneNumber = a.PhoneNumber
            }).ToList();
        }
    }
}

