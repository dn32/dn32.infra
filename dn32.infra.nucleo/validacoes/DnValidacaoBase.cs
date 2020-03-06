using dn32.infra.nucleo.servicos;

namespace dn32.infra.nucleo.validacoes
{
    public abstract class DnValidacaoBase
    {
        protected virtual DnServicoBase Servico { get; set; }
    }
}