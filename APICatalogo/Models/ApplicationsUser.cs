using Microsoft.AspNetCore.Identity;

namespace APICatalogo.Models;

public class ApplicationsUser : IdentityUser
{

    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }

}
