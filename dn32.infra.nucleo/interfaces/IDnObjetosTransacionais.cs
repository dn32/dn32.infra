using System;
using System.Linq;




using Microsoft.EntityFrameworkCore;

namespace dn32.infra
{
    public interface IDnObjetosTransacionais : IDisposable
    {
        IDnDbContext contexto { get; }

        IQueryable<TX> ObterObjetoQueryInterno<TX>() where TX : EntidadeBase;

        IQueryable<T> ObterObjetoInputInterno<T>() where T : EntidadeBase;

        IQueryable ObterObjetoInputDataInterno(Type tipo);
    }
}