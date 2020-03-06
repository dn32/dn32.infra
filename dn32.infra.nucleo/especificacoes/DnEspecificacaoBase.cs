using dn32.infra.nucleo.excecoes;
using dn32.infra.servicos;
using System.Linq;
using dn32.infra.dados;

namespace dn32.infra.nucleo.especificacoes
{
    public abstract class DnEspecificacaoBase
    {
        internal DnEspecificacaoBase() { }

        protected bool IgnorarAOrdenacao { get; set; } = false;

        public DnServicoTransacionalBase Servico { get; set; }

        protected IQueryable<TX> ObterEntidade<TX>() where TX : EntidadeBase
        {
            if (Servico == null)
                throw new DesenvolvimentoIncorretoException($"Falha ao inicializar a especificação [{GetType().Name}]. \nVocê deve usar [CriarEspecificacao] presente no serviço ou no controlador.");

            var transactionObjects = Servico.ObjetosDaTransacao;
            return transactionObjects.ObterObjetoQueryInterno<TX>();
        }

        internal void DefinirServico(DnServicoTransacionalBase servico)
        {
            Servico = servico;
        }
    }
}