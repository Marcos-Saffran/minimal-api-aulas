using Microsoft.EntityFrameworkCore;
using minimal_api.Dominio.Entidades;

namespace MinimalApi.Infraestrutura.Db;

public class DbContexto : DbContext
{
    private readonly IConfiguration _configuracaoAppSettings;

    public DbContexto(IConfiguration configuracaoAppSettings)
    {
        _configuracaoAppSettings = configuracaoAppSettings;
    }


    public DbSet<Administrador> Administradores { get; set; } = default!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
            return;

        var stringConexao = _configuracaoAppSettings.GetConnectionString("mysql")?.ToString();

        if (!string.IsNullOrEmpty(stringConexao))
        {
            optionsBuilder.UseMySql(
                stringConexao,
                ServerVersion.AutoDetect(stringConexao));
        }
        else
        {
            throw new InvalidOperationException("A string de conexão 'mysql' não está configurada.");
        }
    }
}

