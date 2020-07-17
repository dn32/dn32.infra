﻿using System.Reflection;

using Newtonsoft.Json;

namespace dn32.infra
{
    public class DnCampoDeTelaErroDeValidacao : DnPropriedadeErroDeValidacao
    {
        [JsonProperty("campo")]
        public string Campo { get; set; }

        [JsonProperty("chave_de_globalizacao")]
        public override string ChaveDeGlobalizacao => nameof(DnCampoDeTelaErroDeValidacao);

        public DnCampoDeTelaErroDeValidacao(
                PropertyInfo propriedade,
                bool valoresGlobalizados,
                string mensagem,
                string propriedadeDeComposicao,
                string campoDeComposicao) :
            base(propriedade, valoresGlobalizados, mensagem, propriedadeDeComposicao)
        {
            this.Campo = string.IsNullOrWhiteSpace(campoDeComposicao) ?
                propriedade.GetUiPropertyName() :
                $"{campoDeComposicao}.{propriedade.GetUiPropertyName()}";
        }
    }
}