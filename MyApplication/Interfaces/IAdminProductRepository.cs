using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marqelle.Application.DTO;
using Marqelle.Domain.Entities;

namespace Marqelle.Application.Interfaces
{
    public interface IAdminProductRepository
    {
        Task<Products> AddProductsAsync(Products product, string categoryName);
    }
}
