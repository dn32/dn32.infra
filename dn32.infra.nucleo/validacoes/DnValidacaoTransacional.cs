using System.Collections.Generic;
using System.Linq;




namespace dn32.infra
{
    public abstract class DnValidacaoTransacional : DnValidacaoBase
    {
        public bool PausarODisparoDeErrosDeValidacao { get; set; }

        protected internal new DnServicoTransacional Servico
        {
            get => base.Servico as DnServicoTransacional;
            set => base.Servico = value;
        }

        protected internal virtual void Inicializar(DnServicoTransacional servico)
        {
            Servico = servico;
        }

        public void AdicionarInconsistencia(DnErroDeValidacao inconsistencia)
        {
            this.Servico.SessaoDaRequisicao.ContextoDeValidacao.AddInconsistency(inconsistencia);
        }

        public void LimparInconsistencias()
        {
            this.Servico.SessaoDaRequisicao.ContextoDeValidacao.Inconsistencies.Clear();
        }

        public void ExecutarAsValidacoes()
        {
            if (PausarODisparoDeErrosDeValidacao) return;
            this.Servico.SessaoDaRequisicao.ContextoDeValidacao.Validate();
        }

        public void ExecutarAsValidacoes(List<DnServicoTransacional> anotherServices)
        {
            if (PausarODisparoDeErrosDeValidacao) return;

            anotherServices.SelectMany(x => x.SessaoDaRequisicao.ContextoDeValidacao.Inconsistencies).ToList().ForEach(ex =>
            {
                Servico.SessaoDaRequisicao.ContextoDeValidacao.AddInconsistency(ex);
            });

            this.Servico.SessaoDaRequisicao.ContextoDeValidacao.Validate();
        }

        public void ValorDeveSerInformado(object valor, string mensagem = "")
        {
            if (valor != null) { return; }

            mensagem = string.IsNullOrWhiteSpace(mensagem) ? "O valor não pode ser nulo" : mensagem;
            AdicionarInconsistencia(new DnValorNuloErroDeValidacao(mensagem));
            ExecutarAsValidacoes();
        }
    }
}