using Microsoft.EntityFrameworkCore;

public class MeuContextoBancoDados : DbContext
{
    public MeuContextoBancoDados(DbContextOptions<MeuContextoBancoDados> options) : base(options)
    {
    }

    public DbSet<ResultadoPesquisa> ResultadosPesquisa { get; set; }
}
