using dn32.infra.dados;
using dn32.infra.extensoes;
using dn32.infra.nucleo.interfaces;
using dn32.infra.nucleo.especificacoes;
using dn32.infra.Nucleo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace dn32.infra.EntityFramework.Specifications
{
    public class DnSqlSpec<TE> : DnEspecificacao<TE> where TE : DnEntidade
    {
        private string Sql { get; set; }

        private object[] Parameters { get; set; }

        public DnSqlSpec<TE> SetParameter(string sql, params object[] parameters)
        {
            Sql = sql;
            Parameters = parameters;
            return this;
        }

        public override IQueryable<TE> Where(IQueryable<TE> query)
        {
            IgnorarAOrdenacao = true;

            var dbSet = query.DnCast<DbSet<TE>>();
            return dbSet.FromSqlRaw(Sql, Parameters);
        }

        public override IOrderedQueryable<TE> Order(IQueryable<TE> query) => throw new NotImplementedException();
    }
}
