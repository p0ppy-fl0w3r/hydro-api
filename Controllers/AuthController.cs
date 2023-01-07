using Hydro.Data;
using Hydro.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Hydro.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _db;

    public AuthController(UserManager<ApplicationUser> userManager, ApplicationDbContext db)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public IEnumerable<ApplicationUser> GetAllUsers() {
        return _db.Users;
    }

}