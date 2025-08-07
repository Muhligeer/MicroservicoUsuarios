using MicroservicoUsuarios.Application.DTOs;
using MicroservicoUsuarios.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroservicoUsuarios.Core.Interfaces.Services;

public interface IUsuarioService
{
    Task<UsuarioDto> GetUsuarioById(Guid id);
    Task<IEnumerable<UsuarioDto>> GetAllUsuarios();
    Task<UsuarioDto> CreateUsuario(CreateUsuarioDto createUsuarioDto);
    Task UpdateUsuario(Guid id, UpdateUsuarioDto updateUsuarioDto);
    Task DeleteUsuario(Guid id);
}
