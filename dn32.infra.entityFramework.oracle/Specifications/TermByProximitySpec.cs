using System;
using System.Linq;
using dn32.infra;
using dn32.infra;
using dn32.infra;

namespace dn32.infra {
    public class TermByProximitySpec<TE> : DnEspecificacao<TE> where TE : DnEntidade {
        private string Term { get; set; }

        private string TableName { get; set; }

        private string[] Columns { get; set; }

        private int Tolerance { get; set; }

        public TermByProximitySpec<TE> AddParameter (string[] properties, string term, int tolerance) {
            TableName = typeof (TE).GetTableName ();
            Columns = properties.Select (property => typeof (TE).GetProperties ().FirstOrDefault (x => x.Name.Equals (property, StringComparison.InvariantCultureIgnoreCase))?.GetColumnName () ??
                throw new Exception ($"Propriedade not found {typeof(TE).Name}.{property}")).ToArray ();
            Term = term;
            Tolerance = tolerance == 0 ? 80 : tolerance;
            return this;
        }

        public override IQueryable<TE> Where (IQueryable<TE> query) {
            IgnorarAOrdenacao = true;
            return query.WhereProximityText (Term, TableName, Columns, Tolerance);
        }

        public override IOrderedQueryable<TE> Order (IQueryable<TE> query) =>
            throw new NotImplementedException ();
    }
}