using Marqelle.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Marqelle.Application.Interfaces
{
    public interface IPasswordService
    {
        string Hash(string password, Users user);
        bool Verify(string hashedPassword, string enteredPassword, Users user);
    }
}
