using dn32.infra.dados;
using dn32.infra.nucleo.extensoes;
using dn32.infra.Nucleo.Extensoes;
using System.Linq;

namespace dn32.infra.nucleo.especificacoes
{
    public class DnTudoEspecificacao<T> : DnEspecificacao<T> where T : DnEntidade
    {
        public bool EhListagem { get; set; } = true;

        public DnTudoEspecificacao<T> AdicionarParametro(bool ehListagem)
        {
            EhListagem = ehListagem;
            return this;
        }

        public override IQueryable<T> Where(IQueryable<T> query) =>
              query.ObterInclusoes(EhListagem).ProjetarDeFormaDinamica(Servico);

        public override IOrderedQueryable<T> Order(IQueryable<T> query) =>
             query.ProjetarDeFormaDinamicaOrdenada(Servico);
    }
}
