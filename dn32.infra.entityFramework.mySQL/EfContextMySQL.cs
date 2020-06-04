// ReSharper disable CommentTypo
using dn32.infra.atributos;
using dn32.infra.enumeradores;
using Microsoft.EntityFrameworkCore;

namespace dn32.infra.EntityFramework.MySQL
{
    /// <inheritdoc />
    /// <summary>
    /// Contexto do EF no net Core
    /// </summary>
    [DnTipoDeBancoDeDadosAtributo(EnumTipoDeBancoDeDados.MYSQL)]
    public class EfContextMySQL : EfContext
    {
        public EfContextMySQL(string connectionString) : base(connectionString)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(ConnectionString);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
