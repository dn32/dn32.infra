using System.Threading;
using System.Threading.Tasks;
using dn32.infra;
using Microsoft.EntityFrameworkCore;

namespace dn32.infra {
    public abstract class DnDbContext : DbContext, IDnDbContext {
        public abstract bool EnableLogicalDeletion { get; set; }

        public bool HaAlteracao => ChangeTracker.HasChanges ();

        //   public abstract Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}