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
        private readonly IGenericRepository<Address> _repository;
        public AddressService(IGenericRepository<Address> repository)
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
            await _repository.AddAsync(address);
            await _repository.SaveAsync();

        }

        public async Task DeleteAddress(long addressId)
        {
            var address = await _repository.GetByIdAsync(addressId);

            if (address == null)
                throw new Exception("No address found");
               
            _repository.Delete(address);
            await _repository.SaveAsync();
        }

        public async Task<List<AddressDto>> GetUserAddress(long userId)
        {
            var allAddresses = await _repository.GetAllAsync(); 

            var userAddresses = allAddresses
                .Where(a => a.UserId == userId)   
                .OrderByDescending(a => a.Id)     
                .ToList();

            var addressDtos = userAddresses.Select(a => new AddressDto
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

            return addressDtos;
        }

        public async Task UpdateAddressAsync(long addressId,long userId, AddressDto dto)
        {
            var address = await _repository.GetByIdAsync(addressId);

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

             _repository.Update(address);
            await _repository.SaveAsync();

        }

        public async Task SetDefaultAddressAsync(long addressId, long userId)
        {
            var userAddresses = await _repository.FindAllAsync(a => a.UserId == userId);

            var targetAddress = userAddresses.FirstOrDefault(a => a.Id == addressId);

            if (targetAddress == null)
                throw new Exception("Address not found or you are not authorized.");

            foreach (var addr in userAddresses)
            {
                addr.IsDefault = addr.Id == addressId;
            }

            await _repository.SaveAsync();
        }
    }
}


