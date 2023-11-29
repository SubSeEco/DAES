using DAES.API.BackOffice.Modelos;
using Microsoft.EntityFrameworkCore;

namespace DAES.API.BackOffice.Modelos
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<Organizacion> TuTabla { get; set; }

        // Otro código de configuración de DbContext
    }
}


