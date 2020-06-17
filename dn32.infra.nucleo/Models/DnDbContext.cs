using dn32.infra.nucleo.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace dn32.infra.nucleo.interfaces
{
    public abstract class DnDbContext : DbContext, IDnDbContext
    {
        public abstract bool EnableLogicalDeletion { get; set; }

        public bool HaAlteracao => ChangeTracker.HasChanges();

     //   public abstract Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
