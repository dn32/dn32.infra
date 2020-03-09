using dn32.infra.dados;
using dn32.infra.nucleo.extensoes;
using dn32.infra.Nucleo.Extensoes;
using System.Linq;

namespace dn32.infra.nucleo.especificacoes
{
    public class DnDinamicaEspeficicacao<T> : DnEspecificacao<T> where T : DnEntidade
    {
        public string[] Propriedades { get; set; }

        public bool EhListagem { get; set; }

        public DnDinamicaEspeficicacao<T> SetParameters(string[] propriedades, bool ehListagem)
        {
            Propriedades = propriedades;
            EhListagem = ehListagem;
            return this;
        }

        public override IQueryable<T> Where(IQueryable<T> query)
        {
            query = query.ObterInclusoes(EhListagem);
            return query.ProjetarDeFormaDinamica(Servico, Propriedades);
        }

        public override IOrderedQueryable<T> Order(IQueryable<T> query) =>
              query.ProjetarDeFormaDinamicaOrdenada(Servico);
    }
}
