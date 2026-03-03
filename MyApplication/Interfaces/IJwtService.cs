using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marqelle.Domain.Entities;

namespace Marqelle.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(Users user);
    }

}
