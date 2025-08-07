using MicroservicoUsuarios.Application.DTOs;
using MicroservicoUsuarios.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MicroservicoUsuarios.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            var token = await _authenticationService.Authenticate(loginDto.Email, loginDto.Password);
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized("Credenciais inválidas.");
            }
            return Ok(new { Token = token });
        }
        catch (Exception)
        {
            return StatusCode(500, $"Ocorreu um erro inesperado.");
        }
    }

    [HttpPost("change-password/{id}")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordDto changePasswordDto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try
        {
            await _authenticationService.ChangePassword(id, changePasswordDto.NewPassword);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "Ocorreu um erro inesperado ao alterar a senha.");
        }
    }
}
