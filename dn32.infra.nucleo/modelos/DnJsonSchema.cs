using dn32.infra.nucleo.atributos;
using System.Collections.Generic;

namespace dn32.infra.nucleo.modelos
{
    public class DnJsonSchema
    {
        public DnFormularioJsonAtributo Formulario { get; set; }
        public List<DnPropriedadeJsonAtributo> Propriedades { get; set; }
    }
}
