using System;
using System.Linq;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using Microsoft.EntityFrameworkCore;

namespace dn32.infra {
    public class DnSqlSpec<TE> : DnEspecificacao<TE> where TE : DnEntidade {
        private string Sql { get; set; }

        private object[] Parameters { get; set; }

        public DnSqlSpec<TE> SetParameter (string sql, params object[] parameters) {
            Sql = sql;
            Parameters = parameters;
            return this;
        }

        public override IQueryable<TE> Where (IQueryable<TE> query) {
            IgnorarAOrdenacao = true;

            var dbSet = query.DnCast<DbSet<TE>> ();
            return dbSet.FromSqlRaw (Sql, Parameters);
        }

        public override IOrderedQueryable<TE> Order (IQueryable<TE> query) =>
            throw new NotImplementedException ();
    }
}