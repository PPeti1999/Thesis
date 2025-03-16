using HealthyAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HealthyAPI.Data
{
    public class Context : IdentityDbContext<User>

    {
        public Context(DbContextOptions<Context> options)
        : base(options)
        {
            //  InitializeData();
        }
    }
}
