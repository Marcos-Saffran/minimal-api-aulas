using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalApi.Dominio.ModelViews;

namespace MinimalApi.Dominio.DTOs
{
    public class AdministradorDTO
    {
        public string Email { get; set; } = default!;
        public string Senha { get; set; } = default!;
        public string Perfil { get; set; } = default!;

        public ErrosDeValidacao ValidarDTO()
        {
            var errosDeValidacao = new ErrosDeValidacao();

            if (string.IsNullOrEmpty(Email))
            {
                errosDeValidacao.Mensagens.Add("O email é obrigatório.");
            }

            if (string.IsNullOrEmpty(Senha))
            {
                errosDeValidacao.Mensagens.Add("A senha é obrigatória.");
            }

            if (string.IsNullOrEmpty(Perfil))
            {
                errosDeValidacao.Mensagens.Add("O perfil é obrigatório.");
            }

            return errosDeValidacao;
        }
    }
}