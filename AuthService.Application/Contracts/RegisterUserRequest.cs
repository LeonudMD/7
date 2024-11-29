using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Contracts
{
    public record RegisterUserRequest(string username, string email, string password);
}
