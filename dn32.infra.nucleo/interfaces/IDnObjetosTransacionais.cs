using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using dn32.infra.dados;

namespace dn32.infra.nucleo.interfaces
{
    public interface IDnObjetosTransacionais : IDisposable
    {
        DbContext Sessao { get; }

        IQueryable<TX> ObterObjetoQueryInterno<TX>() where TX : EntidadeBase;

        DbSet<T> ObterObjetoInputInterno<T>() where T : EntidadeBase;

        IQueryable ObterObjetoInputDataInterno(Type tipo);
    }
}
