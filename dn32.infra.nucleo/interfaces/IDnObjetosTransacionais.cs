using System;
using System.Linq;

namespace dn32.infra
{
    public interface IDnObjetosTransacionais : IDisposable
    {
        IDnDbContext contexto { get; }

        IQueryable<TX> ObterObjetoQueryInterno<TX>() where TX : DnEntidadeBase;

        IQueryable<T> ObterObjetoInputInterno<T>() where T : DnEntidadeBase;

        IQueryable ObterObjetoInputDataInterno(Type tipo);
    }
}