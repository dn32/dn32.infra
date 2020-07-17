using System;
using System.Threading;
using System.Threading.Tasks;

namespace dn32.infra {
    public interface IDnDbContext : IDisposable {
        bool EnableLogicalDeletion { get; set; }
        Task<int> SaveChangesAsync (CancellationToken cancellationToken = default);
        bool HaAlteracao { get; }
    }
}