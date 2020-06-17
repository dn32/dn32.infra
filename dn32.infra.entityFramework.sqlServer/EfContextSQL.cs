// ReSharper disable CommentTypo

using dn32.infra.atributos;
using dn32.infra.enumeradores;

namespace dn32.infra.EntityFramework.SqlServer {
    /// <inheritdoc />
    /// <summary>
    /// Contexto do EF no net Core
    /// </summary>
    [DnTipoDeBancoDeDadosAtributo(EnumTipoDeBancoDeDados.SQL_SERVER)]
    public class EfContextSQLServer : EfContext {
        public EfContextSQLServer (string connectionString) : base (connectionString) { }

        protected override void OnConfiguring (Microsoft.EntityFrameworkCore.DbContextOptionsBuilder optionsBuilder) {
            Microsoft.EntityFrameworkCore.SqlServerDbContextOptionsExtensions.UseSqlServer (optionsBuilder, ConnectionString);
            base.OnConfiguring (optionsBuilder);
        }
    }
}