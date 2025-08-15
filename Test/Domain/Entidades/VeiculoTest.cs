using MinimalApi.Dominio.Entidades;

namespace Test.Domain.Entidades
{
    [TestClass]
    public class VeiculoTest
    {
        [TestMethod]
        public void TestarGetSetPropriedades()
        {
            // Arrange
            var veiculo = new Veiculo();
            var marcaEsperada = "Toyota";
            var NomeEsperado = "Corolla";
            var anoEsperado = 2020;
            var idEsperado = 1;

            // Act
            veiculo.Marca = marcaEsperada;
            veiculo.Nome = NomeEsperado;
            veiculo.Ano = anoEsperado;
            veiculo.Id = idEsperado;

            // Assert
            Assert.AreEqual(marcaEsperada, veiculo.Marca);
            Assert.AreEqual(NomeEsperado, veiculo.Nome);
            Assert.AreEqual(anoEsperado, veiculo.Ano);
            Assert.AreEqual(idEsperado, veiculo.Id);
        }
    }
}