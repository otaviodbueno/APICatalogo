using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTO;

public class RegisterModel
{
    [Required]
    public string? UserName { get; set; }
    [EmailAddress]
    [Required]
    public  string? Email { get; set; }
    [Required]
    public string? Password { get; set; }
}
