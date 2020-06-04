using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using dn32.infra.dados;
using dn32.infra.Nucleo.Models;
using dn32.infra.nucleo.configuracoes;
using dn32.infra.nucleo.Interfaces;

namespace dn32.infra.nucleo.interfaces
{
    public interface IDnObjetosTransacionais : IDisposable
    {
        IDnDbContext contexto { get; }

        IQueryable<TX> ObterObjetoQueryInterno<TX>() where TX : EntidadeBase;

        IQueryable<T> ObterObjetoInputInterno<T>() where T : EntidadeBase;

        IQueryable ObterObjetoInputDataInterno(Type tipo);
    }
}
