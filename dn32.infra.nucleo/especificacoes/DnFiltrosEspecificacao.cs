using System.Linq;




namespace dn32.infra
{
    public class DnFiltrosEspecificacao<T> : DnEspecificacao<T> where T : DnEntidade
    {
        protected Filtro[] Filtros { get; set; }

        public bool EhListagem { get; set; }

        public DnFiltrosEspecificacao<T> SetParameter(Filtro[] filtros, bool ehListagem)
        {
            Filtros = filtros;
            EhListagem = ehListagem;
            return this;
        }

        public override IQueryable<T> Where(IQueryable<T> query)
        {
            var expression = Filtros.ConverterFiltrosParaExpressao<T>();

            return query
                .Where(expression)
                .ObterInclusoes(EhListagem)
                .ProjetarDeFormaDinamica(Servico);
        }

        public override IOrderedQueryable<T> Order(IQueryable<T> query) =>
            query.ProjetarDeFormaDinamicaOrdenada(Servico);
    }
}