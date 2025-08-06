namespace MinimalApi.Infraestrutura.Db;

{
    public class DbContexto : DbContext
    {
        public DbContexto(DbContextOptions<DbContexto> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; } = default!;
    }
}