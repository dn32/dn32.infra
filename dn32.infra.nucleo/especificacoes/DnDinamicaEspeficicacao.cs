using dn32.infra.dados;
using dn32.infra.nucleo.extensoes;
using dn32.infra.Nucleo.Extensoes;
using System.Linq;

namespace dn32.infra.nucleo.especificacoes
{
    public class DnDinamicaEspeficicacao<T> : DnEspecificacao<T> where T : DnEntidade
    {
        public string[] Campos { get; set; }

        public bool EhListagem { get; set; }

        public DnDinamicaEspeficicacao<T> SetParameters(string[] campos, bool ehListagem)
        {
            Campos = campos;
            EhListagem = ehListagem;
            return this;
        }

        public override IQueryable<T> Where(IQueryable<T> query)
        {
            query = query.ObterInclusoes(EhListagem);
            return query.ProjetarDeFormaDinamica(Servico, Campos);
        }

        public override IOrderedQueryable<T> Order(IQueryable<T> query) =>
              query.ProjetarDeFormaDinamicaOrdenada(Servico);
    }
}
