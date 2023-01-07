using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Hydro.Data;
using Hydro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Hydro.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;
    private readonly IConfiguration _configuration;

    public AuthController(UserManager<ApplicationUser> userManager, ApplicationDbContext db, IConfiguration configuration)
    {
        _db = db;
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpGet]
    public IEnumerable<ApplicationUser> GetAllUsers()
    {
        return _db.Users;
    }


    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register(RegisterModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username ?? "");
        if (userExists != null)
        {
            return BadRequest("Username already exists.");
        }

        ApplicationUser user = new()
        {
            Email = model.Email,
            SecurityStamp = Guid.NewGuid().ToString(),
            UserName = model.Username,
            Age = model.Age
        };

        var result = await _userManager.CreateAsync(user, model.Password ?? "");
        if (!result.Succeeded)
        {
            return StatusCode(500);
        }

        return Ok("Created user.");
    }


    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var authClaims = new List<Claim>{
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = GetToken(authClaims);

            return Ok(
                new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    username = user.UserName,
                    email = user.Email,
                    age = user.Age
                }
            );
        }

        return Unauthorized();
    }


    private JwtSecurityToken GetToken(List<Claim> authClaim)
    {
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);


        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddDays(99),
            claims: authClaim,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return token;
    }



}