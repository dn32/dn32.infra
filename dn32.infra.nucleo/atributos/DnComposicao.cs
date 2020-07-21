using System;




namespace dn32.infra
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DnComposicaoAttribute : DnReferenciaAttribute
    {
        public DnJsonSchema Formulario { get; set; }
        public EnumTipoDeOperacaoParaComAsReferencias OperacaoAoSalvar { get; set; } = EnumTipoDeOperacaoParaComAsReferencias.AdicionarEAtualizar;
    }
}