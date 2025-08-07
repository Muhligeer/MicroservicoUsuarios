using MicroservicoUsuarios.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroservicoUsuarios.Core.Interfaces.Services;

public interface IAuthenticationService
{
    Task<string> Authenticate(string email, string password);
    Task ChangePassword(Guid userId, string newPassword);
    string GenerateJwtToken(Usuario usuario);
}
