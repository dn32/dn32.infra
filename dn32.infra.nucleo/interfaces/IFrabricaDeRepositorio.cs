using dn32.infra.dados;
using dn32.infra.servicos;
namespace dn32.infra.nucleo.interfaces
{
    internal interface IFrabricaDeRepositorio
    {
        IDnRepositorio<T> Create<T>(IDnObjetosTransacionais objetosTransacionais, DnServico<T> servico) where T : EntidadeBase;
    }
}
