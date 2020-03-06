using dn32.infra.servicos;

namespace dn32.infra.nucleo.interfaces
{
    public interface IDnEspecificacaoBase
    {
        DnServicoTransacionalBase Servico { get; set; }
    }
}
