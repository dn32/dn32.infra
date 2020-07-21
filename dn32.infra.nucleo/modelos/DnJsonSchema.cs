using System.Collections.Generic;


namespace dn32.infra
{
    public class DnJsonSchema
    {
        public DnFormularioJsonAttribute Formulario { get; set; }
        public List<DnPropriedadeJsonAttribute> Propriedades { get; set; }
        public string Desabilitado { get; internal set; }
    }
}