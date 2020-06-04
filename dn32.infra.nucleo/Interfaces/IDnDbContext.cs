using System;
using System.Threading.Tasks;

namespace dn32.infra.nucleo.Interfaces
{
    public interface IDnDbContext : IDisposable
    {
        bool EnableLogicalDeletion { get; set; }
        Task SaveChangesAsync();
        bool HaAlteracao { get; }
    }
}
