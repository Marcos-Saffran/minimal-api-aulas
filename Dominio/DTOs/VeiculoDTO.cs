using MinimalApi.Dominio.ModelViews;

namespace MinimalApi.Dominio.DTOs
{
    public record VeiculoDTO
    {
        public string Nome { get; set; } = default!;

        public string Marca { get; set; } = default!;

        public int Ano { get; set; } = default!;

        public ErrosDeValidacao ValidarVeiculoDTO()
        {

            var errosDeValidacao = new ErrosDeValidacao();

            if (string.IsNullOrEmpty(Nome))
            {
                errosDeValidacao.Mensagens.Add("O nome do veículo é obrigatório.");
            }
            else
            {
                if (Nome.Length < 3)
                {
                    errosDeValidacao.Mensagens.Add("O nome do veículo deve ter pelo menos 3 caracteres.");
                }

                if (Nome.Length > 150)
                {
                    errosDeValidacao.Mensagens.Add("O nome do veículo deve ter no máximo 150 caracteres.");
                }
            }

            if (string.IsNullOrEmpty(Marca))
            {
                errosDeValidacao.Mensagens.Add("A marca do veículo é obrigatória.");
            }
            else
            {
                if (Marca.Length < 3)
                {
                    errosDeValidacao.Mensagens.Add("A marca do veículo deve ter pelo menos 3 caracteres.");
                }

                if (Marca.Length > 100)
                {
                    errosDeValidacao.Mensagens.Add("A marca do veículo deve ter no máximo 100 caracteres.");
                }
            }

            if (Ano < 1886 || Ano > DateTime.Now.Year)
            {
                errosDeValidacao.Mensagens.Add("O ano do veículo deve ser um valor válido entre 1886 e o ano atual.");
            }

            return errosDeValidacao;
        }
    }
}