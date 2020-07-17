using dn32.infra;
using Newtonsoft.Json;

namespace dn32.infra {
    public class DnInconsistencia {
        public string Mensagem { get; set; }

        public string ChaveDeGlobalizacao { get; set; }

        [JsonIgnore]
        public DnErroDeValidacao DnErroDeValidacao { get; set; }
    }
}