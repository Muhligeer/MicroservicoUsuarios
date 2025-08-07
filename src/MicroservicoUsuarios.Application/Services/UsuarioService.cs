using MicroservicoUsuarios.Application.DTOs;
using MicroservicoUsuarios.Core.Entities;
using MicroservicoUsuarios.Core.Interfaces;
using MicroservicoUsuarios.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroservicoUsuarios.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ILogger<UsuarioService> _logger;

    public UsuarioService(IUsuarioRepository usuarioRepository, ILogger<UsuarioService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _logger = logger;
    }

    public async Task<UsuarioDto> CreateUsuario(CreateUsuarioDto createUsuarioDto)
    {
        _logger.LogInformation("Iniciando a criação de um novo usuário.");
        try
        {
            var existingUser = await _usuarioRepository.GetByEmail(createUsuarioDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("O e-mail já está em uso.");
            }

            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Nome = createUsuarioDto.Nome,
                Email = createUsuarioDto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUsuarioDto.Password)
            };

            await _usuarioRepository.Add(usuario);
            _logger.LogInformation("Usuário criado com sucesso. ID: {Id}", usuario.Id);
            return new UsuarioDto { Id = usuario.Id, Nome = usuario.Nome, Email = usuario.Email };
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Falha na validação de negócio ao criar usuário. E-mail: {Email}", createUsuarioDto.Email);
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao criar usuário. E-mail: {Email}", createUsuarioDto.Email);
            throw;
        }
    }

    public async Task<IEnumerable<UsuarioDto>> GetAllUsuarios()
    {
        _logger.LogInformation("Buscando todos os usuários.");
        try
        {
            var usuarios = await _usuarioRepository.GetAll();
            _logger.LogInformation("Encontrados {Count} usuários.", usuarios.Count());
            return usuarios.Select(u => new UsuarioDto { Id = u.Id, Nome = u.Nome, Email = u.Email }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao buscar todos os usuários.");
            throw;
        }
    }

    public async Task<UsuarioDto> GetUsuarioById(Guid id)
    {
        _logger.LogInformation("Buscando usuário por ID: {Id}", id);
        try
        {
            var usuario = await _usuarioRepository.GetById(id);
            if (usuario == null)
            {
                _logger.LogWarning("Usuário não encontrado. ID: {Id}", id);
                throw new KeyNotFoundException("Usuário não encontrado.");
            }

            _logger.LogInformation("Usuário encontrado. ID: {Id}", id);
            return new UsuarioDto { Id = usuario.Id, Nome = usuario.Nome, Email = usuario.Email };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar usuário por ID: {Id}", id);
            throw;
        }
    }

    public async Task UpdateUsuario(Guid id, UpdateUsuarioDto updateUsuarioDto)
    {
        _logger.LogInformation("Atualizando usuário. ID: {Id}", id);
        try
        {
            var usuario = await _usuarioRepository.GetById(id);
            if (usuario == null)
            {
                throw new KeyNotFoundException("Usuário não encontrado.");
            }

            usuario.Nome = updateUsuarioDto.Nome;

            await _usuarioRepository.Update(usuario);
            _logger.LogInformation("Usuário atualizado com sucesso. ID: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao atualizar usuário. ID: {Id}", id);
            throw;
        }
    }

    public async Task DeleteUsuario(Guid id)
    {
        _logger.LogInformation("Excluindo usuário. ID: {Id}", id);
        try
        {
            await _usuarioRepository.Delete(id);
            _logger.LogInformation("Usuário excluído com sucesso. ID: {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao excluir usuário. ID: {Id}", id);
            throw;
        }
    }
}
