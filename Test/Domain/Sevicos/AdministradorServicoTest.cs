using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Sevicos
{
    [TestClass]
    public class AdministradorServicoTest
    {
        private static DbContexto CriarContextoDeTeste()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();
            // var stringConexao = configuration.GetConnectionString("MySql");

            // var options = new DbContextOptionsBuilder<DbContexto>()
            //     .UseMySql(stringConexao, ServerVersion.AutoDetect(stringConexao))
            //     .Options;

            return new DbContexto(configuration);
        }

        [TestMethod]
        public void TestandoSalvarAdministrador()
        {
            // Arrange
            var context = CriarContextoDeTeste();
            context.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

            var administradorServico = new AdministradorServico(context);

            var administrador = new Administrador();
            administrador.Email = "teste@teste.com";
            administrador.Senha = "senhaTeste123";
            administrador.Perfil = "Adm";

            // Act
            administradorServico.Incluir(administrador);

            // Assert
            Assert.AreEqual(1, administradorServico.Todos(1).Count);
            Assert.AreEqual("teste@teste.com", administrador.Email);
            Assert.AreEqual("senhaTeste123", administrador.Senha);
            Assert.AreEqual("Adm", administrador.Perfil);
            

        }
    }
}