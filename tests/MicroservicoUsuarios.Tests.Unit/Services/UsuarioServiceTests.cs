using MicroservicoUsuarios.Application.DTOs;
using MicroservicoUsuarios.Application.Services;
using MicroservicoUsuarios.Core.Entities;
using MicroservicoUsuarios.Core.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace MicroservicoUsuarios.Tests.Unit.Services;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly Mock<ILogger<UsuarioService>> _loggerMock;
    private readonly UsuarioService _usuarioService;

    public UsuarioServiceTests()
    {
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _loggerMock = new Mock<ILogger<UsuarioService>>();
        _usuarioService = new UsuarioService(_usuarioRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task CreateUsuario_DeveCriarUsuario_QuandoEmailNaoExiste()
    {
        var dto = new CreateUsuarioDto
        {
            Nome = "Teste",
            Email = "teste@teste.com",
            Password = "123456"
        };

        _usuarioRepositoryMock.Setup(r => r.GetByEmail(dto.Email)).ReturnsAsync((Usuario)null);

        var result = await _usuarioService.CreateUsuario(dto);

        Assert.Equal(dto.Email, result.Email);
        _usuarioRepositoryMock.Verify(r => r.Add(It.IsAny<Usuario>()), Times.Once);
    }

    [Fact]
    public async Task CreateUsuario_DeveLancarExcecao_QuandoEmailJaExiste()
    {
        var dto = new CreateUsuarioDto { Nome = "Teste", Email = "teste@teste.com", Password = "123456" };

        _usuarioRepositoryMock.Setup(r => r.GetByEmail(dto.Email))
            .ReturnsAsync(new Usuario());

        await Assert.ThrowsAsync<InvalidOperationException>(() => _usuarioService.CreateUsuario(dto));
    }

    [Fact]
    public async Task GetAllUsuarios_DeveRetornarLista()
    {
        var usuarios = new List<Usuario>
            {
                new Usuario { Id = Guid.NewGuid(), Nome = "User1", Email = "u1@teste.com" }
            };

        _usuarioRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(usuarios);

        var result = await _usuarioService.GetAllUsuarios();

        Assert.Single(result);
    }

    [Fact]
    public async Task GetUsuarioById_DeveRetornarUsuario_QuandoExiste()
    {
        var usuario = new Usuario { Id = Guid.NewGuid(), Nome = "User", Email = "user@teste.com" };

        _usuarioRepositoryMock.Setup(r => r.GetById(usuario.Id))
            .ReturnsAsync(usuario);

        var result = await _usuarioService.GetUsuarioById(usuario.Id);

        Assert.Equal(usuario.Email, result.Email);
    }

    [Fact]
    public async Task GetUsuarioById_DeveLancarExcecao_QuandoNaoExiste()
    {
        _usuarioRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((Usuario)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _usuarioService.GetUsuarioById(Guid.NewGuid()));
    }

    [Fact]
    public async Task UpdateUsuario_DeveAtualizar_QuandoUsuarioExiste()
    {
        var usuario = new Usuario { Id = Guid.NewGuid(), Nome = "Antigo" };
        var dto = new UpdateUsuarioDto { Nome = "Novo" };

        _usuarioRepositoryMock.Setup(r => r.GetById(usuario.Id)).ReturnsAsync(usuario);

        await _usuarioService.UpdateUsuario(usuario.Id, dto);

        _usuarioRepositoryMock.Verify(r => r.Update(It.Is<Usuario>(u => u.Nome == "Novo")), Times.Once);
    }

    [Fact]
    public async Task DeleteUsuario_DeveChamarRepositorio()
    {
        var id = Guid.NewGuid();

        await _usuarioService.DeleteUsuario(id);

        _usuarioRepositoryMock.Verify(r => r.Delete(id), Times.Once);
    }
}
