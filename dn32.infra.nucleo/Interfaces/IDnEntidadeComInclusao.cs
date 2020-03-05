using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace dn32.infra.Interfaces
{
    public interface IDnEntidadeComInclusao
    {
        [NotMapped, JsonIgnore]
        string[] InclusoesParaLista { get; }

        [NotMapped, JsonIgnore]
        string[] InclusoesParaUm { get; }
    }
}
