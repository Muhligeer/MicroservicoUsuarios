using MicroservicoUsuarios.Application.DTOs;
using MicroservicoUsuarios.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicoUsuarios.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        try
        {
            var usuarioDto = await _usuarioService.GetUsuarioById(id);
            return Ok(usuarioDto);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocorreu um erro inesperado ao buscar o usuário.");
        }
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var usuarios = await _usuarioService.GetAllUsuarios();
            return Ok(usuarios);
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocorreu um erro inesperado ao buscar todos os usuários.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUsuarioDto createUsuarioDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var usuarioDto = await _usuarioService.CreateUsuario(createUsuarioDto);
            return CreatedAtAction(nameof(GetById), new { id = usuarioDto.Id }, usuarioDto);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocorreu um erro inesperado ao criar o usuário.");
        }
    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUsuarioDto updateUsuarioDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            await _usuarioService.UpdateUsuario(id, updateUsuarioDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocorreu um erro inesperado ao atualizar o usuário.");
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _usuarioService.DeleteUsuario(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocorreu um erro inesperado ao excluir o usuário.");
        }
    }
}
