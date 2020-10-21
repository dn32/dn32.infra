// ReSharper disable CommentTypo


using Microsoft.EntityFrameworkCore;

namespace dn32.infra
{
    /// <inheritdoc />
    /// <summary>
    /// Contexto do EF no net Core
    /// </summary>
    [DnTipoDeBancoDeDadosAttribute(EnumTipoDeBancoDeDados.MYSQL)]
    public class EfContextMySQL : EfContext
    {
        public EfContextMySQL(string connectionString) : base(connectionString) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(ConnectionString, (o) =>
            {
                o.EnableRetryOnFailure();
            });
            base.OnConfiguring(optionsBuilder);
        }
    }
}