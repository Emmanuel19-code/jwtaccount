using jwtaccount.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace jwtaccount.Infrastructure.Context
{
    public class ApplicationDBContext(DbContextOptions<ApplicationDBContext>options) : DbContext(options)
    {
       public DbSet<User> users {get;set;}
    }
}