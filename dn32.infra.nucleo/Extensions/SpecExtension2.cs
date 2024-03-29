﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace dn32.infra
{
    public static class SpecExtension2
    {
        public static IQueryable<T> ObterInclusoes<T>(this IQueryable<T> query, bool ehLista) where T : DnEntidade
        {
            if (typeof(T).Is(typeof(IDnEntidadeComInclusao)))
            {
                var entity = Activator.CreateInstance(typeof(T)).DnCast<IDnEntidadeComInclusao>();
                var inclusions = ehLista ? entity.InclusoesParaLista : entity.InclusoesParaUm;
                inclusions.ToList().ForEach(x => { query = query.Include(x); });
            }

            return query;
        }
    }
}