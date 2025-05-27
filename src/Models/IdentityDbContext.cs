using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace videogame_api.src.Models
{
    public class IdentityDb : IdentityDbContext<IdentityUser>
    {
        public IdentityDb(DbContextOptions<IdentityDb> options) : base(options)
        {}
    }
}
