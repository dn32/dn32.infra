// ReSharper disable CommentTypo
using Microsoft.EntityFrameworkCore;
using Oracle.EntityFrameworkCore;

namespace dn32.infra
{
    /// <inheritdoc />
    /// <summary>
    /// Contexto do EF no net Core
    /// </summary>
    [DnTipoDeBancoDeDadosAttribute(EnumTipoDeBancoDeDados.ORACLE)]
    public class EfContextOracle : EfContext
    {

        public EfContextOracle(string connectionString) : base(connectionString)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Oracle.EntityFrameworkCore.UseOracle(optionsBuilder, ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}