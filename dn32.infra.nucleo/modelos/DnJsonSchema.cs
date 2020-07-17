using System.Collections.Generic;
using dn32.infra;

namespace dn32.infra {
    public class DnJsonSchema {
        public DnFormularioJsonAtributo Formulario { get; set; }
        public List<DnPropriedadeJsonAtributo> Propriedades { get; set; }
        public string Desabilitado { get; internal set; }
    }
}