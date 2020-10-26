using System;
using System.Linq;




namespace dn32.infra
{
    public abstract class DnEspecificacaoBase
    {
        internal DnEspecificacaoBase() { }

        protected bool IgnorarAOrdenacao { get; set; } = false;

        public DnServicoTransacional Servico { get; set; }

        protected IQueryable<TX> ObterEntidade<TX>(Type tipoDaEntidadeOriginal) where TX : DnEntidadeBase
        {
            if (Servico == null)
                throw new DesenvolvimentoIncorretoException($"Falha ao inicializar a especificação [{GetType().Name}]. \nVocê deve usar [CriarEspecificacao] presente no serviço ou no controlador.");
            
            var tipo = UtilitarioDeFabrica.ObterTipoDebancoDeDados(tipoDaEntidadeOriginal);
            var transactionObjects = Servico.SessaoDaRequisicao.ObterObjetosDaTransacao(tipo);
            return transactionObjects.ObterObjetoQueryInterno<TX>();
        }

        internal void DefinirServico(DnServicoTransacional servico)
        {
            Servico = servico;
        }
    }
}