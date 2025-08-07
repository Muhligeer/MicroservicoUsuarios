using MicroservicoUsuarios.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroservicoUsuarios.Core.Interfaces;

public interface IUsuarioRepository
{
    Task<Usuario> GetById(Guid id);
    Task<Usuario> GetByEmail(string email);
    Task<IEnumerable<Usuario>> GetAll();
    Task Add(Usuario usuario);
    Task Update(Usuario usuario);
    Task Delete(Guid id);
}
