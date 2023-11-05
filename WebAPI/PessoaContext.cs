using Microsoft.EntityFrameworkCore;

namespace WebAPI;

public class PessoaContext : DbContext
{
    public PessoaContext(DbContextOptions<PessoaContext> options)
        : base(options)
    {}

    public DbSet<Pessoa> Pessoas { get; set; } = null!;
}
