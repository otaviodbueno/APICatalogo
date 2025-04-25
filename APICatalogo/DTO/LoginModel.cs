using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTO;

public class LoginModel
{
    [Required]
    public string? UserName { get; set; }
    public string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
}
