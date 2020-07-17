using System;




namespace dn32.infra
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DnComposicaoAtributo : DnReferenciaAtributo
    {
        public DnJsonSchema Formulario { get; set; }
        public EnumTipoDeOperacaoParaComAsReferencias OperacaoAoSalvar { get; set; } = EnumTipoDeOperacaoParaComAsReferencias.AdicionarEAtualizar;
    }
}