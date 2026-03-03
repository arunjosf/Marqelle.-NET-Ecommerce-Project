using Marqelle.Application.DTO;
using Marqelle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Interfaces
{
    public interface IAdminProductService
    {
        Task<Products> AddProducts(AdminAddproductDto dto);
    }
}
