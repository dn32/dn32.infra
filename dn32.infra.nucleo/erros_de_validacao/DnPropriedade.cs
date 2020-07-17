using System.Reflection;
using dn32.infra;
using Newtonsoft.Json;

namespace dn32.infra {
    public class DnPropriedadeErroDeValidacao : DnErroDeValidacao {
        [JsonProperty ("nome_da_propriedade")]
        public string NomeDaPropriedade { get; set; }

        [JsonIgnore]
        public PropertyInfo Propriedade { get; set; }

        public DnPropriedadeErroDeValidacao (
                PropertyInfo propriedade,
                bool valoresGlobalizados,
                string mensagem,
                string propriedadeDeComposicao):
            base (mensagem, valoresGlobalizados) {
                this.Propriedade = propriedade;
                this.NomeDaPropriedade =
                    string.IsNullOrWhiteSpace (propriedadeDeComposicao) ?
                    propriedade.GetJsonPropertyName () :
                    $"{propriedadeDeComposicao}.{propriedade.GetJsonPropertyName()}";

                this.Valores = new [] { this.NomeDaPropriedade };
            }

        [JsonProperty ("chave_de_globalizacao")]
        public override string ChaveDeGlobalizacao => "DnPropriedadeErroDeValidacao";
    }
}