using dn32.infra.Factory;
using dn32.infra.nucleo.especificacoes;
using dn32.infra.nucleo.servicos;
using dn32.infra.nucleo.validacoes;
using dn32.infra.nucleo.interfaces;
using dn32.infra.Nucleo.Models;

namespace dn32.infra.servicos
{
    public abstract class DnServicoTransacionalBase : DnServicoBase
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
            return SpecFactory.Criar<T>(this);
        }
    }
}
