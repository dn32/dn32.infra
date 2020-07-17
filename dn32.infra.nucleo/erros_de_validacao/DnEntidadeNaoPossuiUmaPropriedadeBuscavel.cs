﻿
using Newtonsoft.Json;

namespace dn32.infra
{
    public class DnEntidadeNaoPossuiUmaPropriedadeBuscavelErroDeValidacao : DnErroDeValidacao
    {
        [JsonProperty("chave_de_globalizacao")]
        public override string ChaveDeGlobalizacao => nameof(DnEntidadeNaoPossuiUmaPropriedadeBuscavelErroDeValidacao);

        public DnEntidadeNaoPossuiUmaPropriedadeBuscavelErroDeValidacao(string nomeDaEntidade) : base(
            $"A entidade {nomeDaEntidade} não possui uma propriedade " +
            $"decorada com o atributo {nameof(DnBuscavelAtributo)}")
        { }
    }
}