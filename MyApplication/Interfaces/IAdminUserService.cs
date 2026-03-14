using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marqelle.Application.DTO;

namespace Marqelle.Application.Interfaces
{
    public interface IAdminUserService
    {
        Task<List<AdminUserManagementDto>> GetAllUsersAsync();
        Task<List<AdminUserManagementDto>> SearchUsersAsync(long? id, string? name);
        Task BlockUserAsync(long userId);
        Task UnblockUserAsync(long userId);
    }
}
