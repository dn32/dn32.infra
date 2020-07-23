
namespace dn32.infra
{
    public abstract class DnServicoTransacional : DnServicoBase
    {
        internal IDnObjetosTransacionais ObjetosDaTransacao => SessaoDaRequisicao.ObjetosDaTransacao;

        protected new internal DnValidacaoTransacional Validacao
        {
            get => base.Validacao as DnValidacaoTransacional;
            set => base.Validacao = value;
        }

        protected new internal IDnRepositorioTransacional Repositorio
        {
            get => base.Repositorio as IDnRepositorioTransacional;
            set => base.Repositorio = value;
        }

        protected internal override void DefinirSessaoDoUsuario(SessaoDeRequisicaoDoUsuario sessaoDaRequisicao)
        {
            base.DefinirSessaoDoUsuario(sessaoDaRequisicao);
        }

        protected T CriarEspecificacao<T>() where T : DnEspecificacaoBase
        {
            return FabricaDeEspecificacao.Criar<T>(this);
        }
    }
}