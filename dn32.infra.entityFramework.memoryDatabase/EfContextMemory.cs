// ReSharper disable CommentTypo
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using dn32.infra.atributos;
using dn32.infra.enumeradores;

namespace dn32.infra.EntityFramework.MemoryDatabase
{
    /// <inheritdoc />
    /// <summary>
    /// Contexto do EF no net Core
    /// </summary>
    [DnTipoDeBancoDeDadosAtributo(EnumTipoDeBancoDeDados.MEMORY)]
    public class EfContextMemory : EfContext
    {
        public EfContextMemory(string connectionString) : base(connectionString)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(x =>
            {
                x.Ignore(eventIds: InMemoryEventId.TransactionIgnoredWarning);
            });

            optionsBuilder.UseInMemoryDatabase(ConnectionString);
        }
    }
}
