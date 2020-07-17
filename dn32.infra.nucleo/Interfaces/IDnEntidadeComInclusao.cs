using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace dn32.infra {
    public interface IDnEntidadeComInclusao {
        [NotMapped, JsonIgnore]
        string[] InclusoesParaLista { get; }

        [NotMapped, JsonIgnore]
        string[] InclusoesParaUm { get; }
    }
}