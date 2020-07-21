using Newtonsoft.Json;
using System;

namespace dn32.infra
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DnAgregacaoAttribute : DnReferenciaAttribute
    {
        public string CampoAApresentar { get; set; }

        public string[] PropriedadesParaBusca { get; set; }

        public bool PermitirAdicionar { get; set; }

        [JsonIgnore]
        public string PropriedadeParaBusca
        {
            get => this.PropriedadesParaBusca?.Length > 0 ? this.PropriedadesParaBusca[0] : null;
            set
            {
                this.PropriedadesParaBusca = new[] { value };
            }
        }

        public DnFiltroAttribute Filtro { get; set; }
    }
}