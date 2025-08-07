using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroservicoUsuarios.Application.DTOs;

public class ChangePasswordDto
{
    [Required(ErrorMessage = "A nova senha é obrigatória.")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "A senha deve ter entre 8 e 50 caracteres.")]
    public string NewPassword { get; set; }
    [Required(ErrorMessage = "A confirmação da nova senha é obrigatória.")]
    [Compare("NewPassword", ErrorMessage = "A nova senha e a confirmação não correspondem.")]
    public string ConfirmNewPassword { get; set; }
}
