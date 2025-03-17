using Microsoft.EntityFrameworkCore;

namespace TrabalhoESII.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Exemplo de tabela
        public DbSet<Exemplo> Exemplos { get; set; }
    }

    public class Exemplo
    {
        public int Id { get; set; }
        public string Nome { get; set; }
    }
}