using System.Reflection;
using dn32.infra;
using Newtonsoft.Json;

namespace dn32.infra {
    public class DnCampoDeTelaRequeridoErroDeValidacao : DnCampoDeTelaErroDeValidacao {
        [JsonProperty ("chave_de_globalizacao")]
        public override string ChaveDeGlobalizacao => nameof (DnCampoDeTelaRequeridoErroDeValidacao);

        public DnCampoDeTelaRequeridoErroDeValidacao (PropertyInfo propriedade, string propriedadeDeComposicao, string campoDeComposicao):
            base (propriedade, true, $"O campo '{(campoDeComposicao == null ? propriedade.GetUiPropertyName() : (!string.IsNullOrWhiteSpace(campoDeComposicao) ? (campoDeComposicao + ".") : "" + propriedade.GetUiPropertyName()))}' deve possuir um valor.", propriedadeDeComposicao, campoDeComposicao) { }
    }
}