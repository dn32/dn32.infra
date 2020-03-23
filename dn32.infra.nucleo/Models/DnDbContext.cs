using Microsoft.EntityFrameworkCore;

namespace dn32.infra.nucleo.interfaces
{
    public abstract class DnDbContext:DbContext
    {
        public abstract bool EnableLogicalDeletion { get; set; }
    }
}
