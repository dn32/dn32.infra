using System.Linq;
using dn32.infra;
using dn32.infra;
using Microsoft.EntityFrameworkCore;

namespace dn32.infra {
    public static class QueryExtension {
        public static IQueryable<T> WhereProximityText<T> (this IQueryable<T> query, string term, string table, string column, int tolerance) where T : DnEntidade {
            if (string.IsNullOrWhiteSpace (term)) { return query.OrderBy (x => x); }

            var sql = $@"
select * from {table}
where lower({column}) is not null and UTL_MATCH.jaro_winkler_similarity(lower({column}), lower({{0}})) > {tolerance}
order by UTL_MATCH.jaro_winkler_similarity(lower({column}), lower({{0}})) DESC
";

            var dbSet = query.DnCast<DbSet<T>> ();
            return dbSet.FromSqlRaw (sql, term);
        }

        public static IQueryable<T> WhereProximityText<T> (this IQueryable<T> query, string term, string table, string[] columns, int tolerance) where T : DnEntidade {
            if (string.IsNullOrWhiteSpace (term)) { return query.OrderBy (x => x); }

            var sqlArray = columns.Select (column => $@"
SELECT UTL_MATCH.JARO_WINKLER_SIMILARITY(LOWER({column}), LOWER('{term}')) PRECISAO__, {table}.* FROM {table} 
WHERE {column} IS NOT NULL AND UTL_MATCH.JARO_WINKLER_SIMILARITY(LOWER({column}), LOWER('{term}')) >= {tolerance}
");

            var union = string.Join ("\nUNION ALL\n", sqlArray);
            var sql = $@"SELECT * FROM ({union}) ORDER BY PRECISAO__ DESC";

            var dbSet = query.DnCast<DbSet<T>> ();
            return dbSet.FromSqlRaw (sql, term);
        }
    }
}