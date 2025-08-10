using System.ComponentModel.DataAnnotations;

namespace MicroservicoUsuarios.Application.DTOs;

public class LoginDto
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "O e-mail deve ser um endereço de e-mail válido.")]
    public string Email { get; set; }
    [Required(ErrorMessage = "A senha é obrigatória.")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 50 caracteres.")]
    public string Password { get; set; }
}
