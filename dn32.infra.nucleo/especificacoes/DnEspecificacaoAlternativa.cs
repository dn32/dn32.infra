using System;
using System.Linq;



namespace dn32.infra
{
    public abstract class DnEspecificacaoAlternativa<TE, TO> : DnEspecificacaoBase, IDnEspecificacaoAlternativaGenerica<TO> where TE : EntidadeBase
    {
        public abstract IQueryable<TO> Where(IQueryable<TE> query);

        public abstract IOrderedQueryable<TO> Order(IQueryable<TO> query);

        internal IOrderedQueryable<TO> ConverterParaIQueryable(IQueryable<TE> query) =>
            Order(Where(query));

        public Type TipoDeEntidade => typeof(TE);

        public Type TipoDeRetorno => typeof(TO);
    }
}