using MicroservicoUsuarios.Core.Entities;
using MicroservicoUsuarios.Core.Interfaces;
using MicroservicoUsuarios.Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace MicroservicoUsuarios.Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(IUsuarioRepository usuarioRepository, IConfiguration configuration, ILogger<AuthenticationService> logger)
    {
        _usuarioRepository = usuarioRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string> Authenticate(string email, string password)
    {
        _logger.LogInformation("Tentativa de autenticação para o usuário: {Email}", email);
        var usuario = await _usuarioRepository.GetByEmail(email);
        if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
        {
            _logger.LogWarning("Falha de autenticação para o usuário: {Email}", email);
            return null;
        }

        _logger.LogInformation("Autenticação bem-sucedida para o usuário: {Email}", email);
        return GenerateJwtToken(usuario);
    }

    public async Task ChangePassword(Guid userId, string newPassword)
    {
        _logger.LogInformation("Tentativa de alteração de senha para o usuário: {Id}", userId);
        var usuario = await _usuarioRepository.GetById(userId);
        if (usuario == null)
        {
            _logger.LogWarning("Falha na alteração de senha: usuário não encontrado. ID: {Id}", userId);
            throw new KeyNotFoundException("Usuário não encontrado.");
        }
        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        await _usuarioRepository.Update(usuario);
        _logger.LogInformation("Senha alterada com sucesso para o usuário: {Id}", userId);
    }

    public string GenerateJwtToken(Usuario usuario)
    {
        var jwtSecret = _configuration["Jwt:Secret"];
        if (string.IsNullOrEmpty(jwtSecret))
        {
            _logger.LogError("Chave JWT não configurada em appsettings.json.");
            throw new InvalidOperationException("Chave JWT não configurada.");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
