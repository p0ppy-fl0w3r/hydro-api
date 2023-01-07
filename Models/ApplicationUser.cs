using Microsoft.AspNetCore.Identity;

namespace Hydro.Models;

public class ApplicationUser : IdentityUser
{
    public int Age { get; set; }
}