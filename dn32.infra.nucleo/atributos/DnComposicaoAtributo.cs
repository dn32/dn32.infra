using System;
using dn32.infra;
using dn32.infra;
using dn32.infra;

namespace dn32.infra {
    [AttributeUsage (AttributeTargets.Property)]
    public class DnComposicaoAtributo : DnReferenciaAtributo {
        public DnJsonSchema Formulario { get; set; }
        public EnumTipoDeOperacaoParaComAsReferencias OperacaoAoSalvar { get; set; } = EnumTipoDeOperacaoParaComAsReferencias.AdicionarEAtualizar;
    }
}