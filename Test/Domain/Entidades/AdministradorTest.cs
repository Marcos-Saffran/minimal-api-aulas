using MinimalApi.Dominio.Entidades;

namespace Test.Domain.Entidades
{
    [TestClass]
    public class AdministradorTest
    {
        [TestMethod]
        public void TestarGetSetPropriedades()
        {
            // Arrange
            var administrador = new Administrador();
            var emailEsperado = "admin@test.com";
            var senhaEsperada = "senha123";
            var perfilEsperado = "Adm";
            var idEsperado = 1;

            // Act
            administrador.Email = emailEsperado;
            administrador.Senha = senhaEsperada;
            administrador.Perfil = perfilEsperado;
            administrador.Id = idEsperado;

            // Assert
            Assert.AreEqual(emailEsperado, administrador.Email);
            Assert.AreEqual(senhaEsperada, administrador.Senha);
            Assert.AreEqual(perfilEsperado, administrador.Perfil);
            Assert.AreEqual(idEsperado, administrador.Id);
        }
    }
}