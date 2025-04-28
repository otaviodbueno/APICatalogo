using APICatalogo.DTO;
using APICatalogo.Models;
using APICatalogo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace APICatalogo.Controllers;


[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<ApplicationsUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ITokenService tokenService, 
        UserManager<ApplicationsUser> userManager, 
        RoleManager<IdentityRole> roleManager,
        IConfiguration config,
        ILogger<AuthController> logger)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _roleManager = roleManager;
        _config = config;
        _logger = logger;
    }


    [HttpPost]
    [Route("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {

        var user = await _userManager.FindByNameAsync(model.UserName!);

        if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password!))
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim("id", user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            foreach(var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var token = _tokenService.GenerateAcessToken(authClaims, _config);

            var refreshToken = _tokenService.GenerateRefreshToken();

            _ = int.TryParse(_config["JWT:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes);

            user.RefreshToken = refreshToken;

            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidityInMinutes);

            await _userManager.UpdateAsync(user);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo,
                RefreshToken = refreshToken
            });
        }

        return Unauthorized(new { message = "Usuário ou senha inválidos" });
    }

    [HttpPost]
    [Route("Register")]
    public async Task<IActionResult> Register([FromBody] LoginModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.UserName!);

        if (userExists is not null)
        {
            return BadRequest(new { message = "Usuário já existe" });
        }

        ApplicationsUser user = new()
        {
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.UserName,
            Email = model.Email,
        };

        var result = await _userManager.CreateAsync(user, model.Password!);

        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Erro ao criar usuário, verifique se a senha é válida!" });
        }

        return Ok(new Response
        {
            Status = "Success",
            Message = "Usuário criado com sucesso"
        });
    }

    [HttpPost]
    [Route("RefreshToken")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenModel tokenModel)
    {
       if(tokenModel is null)
        {
            return BadRequest("Invalid client request");    
        }

       string? acessToken = tokenModel.AcessToken;

        string? refreshToken = tokenModel.RefreshToken;

        var principal = _tokenService.GetPrincipalFromExpiredToken(acessToken!, _config);

        if(principal is null)
        {
            return BadRequest("Invalid acess token/refresh token");
        }

        var userName = principal.Identity?.Name;

        var user = await _userManager.FindByNameAsync(userName!);

        if(user == null || user.RefreshToken != refreshToken ||
            user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return BadRequest("Invalid acess token/refresh token");
        }

        var newAcessToken = _tokenService.GenerateAcessToken(principal.Claims, _config);

        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        await _userManager.UpdateAsync(user);

        return new ObjectResult(new
        {
            acessToken = new JwtSecurityTokenHandler().WriteToken(newAcessToken),
            refreshToken = newRefreshToken
        });
    }

    [Authorize(Policy = "SuperAdminOnly")]
    [HttpPost]
    [Route("Revoke/{username}")]
    public async Task<IActionResult> Revoke(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user is null)
        {
            return NotFound(new { message = "Usuário não encontrado" });
        }
        user.RefreshToken = null;

        await _userManager.UpdateAsync(user);

        return NoContent(); // Não tem conteúdo para retornar, mas a requisição foi bem sucedida
    }

    [HttpPost]
    [Route("CreateRole")]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        var roleExist = await _roleManager.RoleExistsAsync(roleName);
        if (roleExist)
        {
            _logger.LogWarning("Role já existe");
        }

        IdentityRole role = new()
        {
            Name = roleName
        };

        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Erro ao criar role");
            return BadRequest();
        }

        _logger.LogWarning("Role criada!");
        return Ok(new Response
        {
            Status = "Success",
            Message = "Role criada com sucesso"
        });
    }

    [HttpPost]
    [Authorize(Policy = "SuperAdminOnly")]
    [Route("AddRoleToUser")]
    public async Task<IActionResult> AddRoleToUser(string email, string roleName)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user is null)
        {
            _logger.LogWarning("Usuário não encontrado");
            return NotFound();
        }
        var roleExist = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            _logger.LogWarning("Role não existe");
            return NotFound();
        }
        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (!result.Succeeded)
        {
            return BadRequest();
        }

        _logger.LogWarning("Role adicionada ao usuário com sucesso");
        return Ok();
    }
}
