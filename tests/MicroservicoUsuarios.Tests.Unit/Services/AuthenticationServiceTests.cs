using MicroservicoUsuarios.Application.Services;
using MicroservicoUsuarios.Core.Entities;
using MicroservicoUsuarios.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using BCrypt.Net;

namespace MicroservicoUsuarios.Tests.Unit.Services;

public class AuthenticationServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepoMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<ILogger<AuthenticationService>> _loggerMock;
    private readonly AuthenticationService _service;

    public AuthenticationServiceTests()
    {
        _usuarioRepoMock = new Mock<IUsuarioRepository>();
        _configMock = new Mock<IConfiguration>();
        _loggerMock = new Mock<ILogger<AuthenticationService>>();

        _configMock.Setup(c => c["Jwt:Secret"]).Returns("uma-chave-secreta-forte-para-o-teste-tecnico-da-junto-seguros-4734891654");

        _service = new AuthenticationService(_usuarioRepoMock.Object, _configMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Authenticate_DeveRetornarToken_QuandoCredenciaisForemValidas()
    {
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = "teste@teste.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("SenhaSegura123")
        };

        _usuarioRepoMock.Setup(r => r.GetByEmail(usuario.Email))
            .ReturnsAsync(usuario);

        var token = await _service.Authenticate(usuario.Email, "SenhaSegura123");

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public async Task Authenticate_DeveRetornarNull_QuandoSenhaInvalida()
    {
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = "teste@teste.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("senhaCorreta")
        };

        _usuarioRepoMock.Setup(r => r.GetByEmail(usuario.Email))
            .ReturnsAsync(usuario);

        var token = await _service.Authenticate(usuario.Email, "senhaErrada");

        Assert.Null(token);
    }

    [Fact]
    public async Task ChangePassword_DeveAlterarSenha_QuandoUsuarioExiste()
    {
        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = "teste@teste.com",
            PasswordHash = "hashAntigo"
        };

        _usuarioRepoMock.Setup(r => r.GetById(usuario.Id))
            .ReturnsAsync(usuario);

        Usuario usuarioAtualizado = null;
        _usuarioRepoMock
            .Setup(r => r.Update(It.IsAny<Usuario>()))
            .Callback<Usuario>(u => usuarioAtualizado = u)
            .Returns(Task.CompletedTask);

        await _service.ChangePassword(usuario.Id, "novaSenha");

        Assert.NotNull(usuarioAtualizado);
        Assert.True(BCrypt.Net.BCrypt.Verify("novaSenha", usuarioAtualizado.PasswordHash));
        _usuarioRepoMock.Verify(r => r.Update(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task ChangePassword_DeveLancarExcecao_QuandoUsuarioNaoExiste()
    {
        _usuarioRepoMock.Setup(r => r.GetById(It.IsAny<Guid>()))
            .ReturnsAsync((Usuario)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.ChangePassword(Guid.NewGuid(), "novaSenha"));
    }

    [Fact]
    public void GenerateJwtToken_DeveLancarExcecao_SeChaveNaoConfigurada()
    {
        _configMock.Setup(c => c["Jwt:Secret"]).Returns((string)null);
        var usuario = new Usuario { Id = Guid.NewGuid(), Email = "teste@teste.com" };

        Assert.Throws<InvalidOperationException>(() => _service.GenerateJwtToken(usuario));
    }
}
