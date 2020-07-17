using Newtonsoft.Json;

namespace dn32.infra {
    public class DnParametroNuloErroDeValidacao : DnValorNuloErroDeValidacao {
        [JsonProperty ("chave_de_globalizacao")]
        public override string ChaveDeGlobalizacao => nameof (DnParametroNuloErroDeValidacao);

        public DnParametroNuloErroDeValidacao (string parametro) : base ($"O par�metro {parametro} n�o deveria ser nulo.", parametro) { }
    }
}