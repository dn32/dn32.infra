using System.Linq;




namespace dn32.infra
{
    public abstract class DnEspecificacaoBase
    {
        internal DnEspecificacaoBase() { }

        protected bool IgnorarAOrdenacao { get; set; } = false;

        public DnServicoTransacionalBase Servico { get; set; }

        protected IQueryable<TX> ObterEntidade<TX>() where TX : DnEntidadeBase
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