﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace dn32.infra
{
    public partial class DnEFRepository<TE>
    {
        internal protected async Task<int> CountSqlAsync(string sql, bool incluirExcluidosLogicamente = false)
        {
            if (incluirExcluidosLogicamente)
            {
                lock (SessionRequest)
                {
                    Session.EnableLogicalDeletion = false;
                }
            }

            var ret = await FromSql(sql).CountAsync();

            lock (SessionRequest)
            {
                Session.EnableLogicalDeletion = true;
            }

            return ret;
        }

        internal protected async Task<TO> FindSingleOrDefaultSqlAsync<TO>(string sql) where TO : DnEntidadeBase
        {
            try
            {
                return await FromSqlSelect<TO>(sql).SingleOrDefaultAsync();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"More than one record was found with the given keys. This is an indication of data with duplicate keys in the database. The table is {typeof(TE).GetTableName()}");
            }
        }

        public override async Task<bool> ExisteAsync(IDnEspecificacaoBase spec)
        {
            return await spec.ObterSpec<TE>().ConverterParaIQueryable(Query).AnyAsync();
        }

        public override async Task<bool> ExisteAlternativoAsync<TO>(IDnEspecificacaoBase spec)
        {
            return await spec.ObterSpecAlternativo<TE, TO>().ConverterParaIQueryable(Query).AnyAsync();
        }

        public override async Task<List<TE>> ListarAsync(IDnEspecificacao ispec, DnPaginacao pagination = null)
        {
            var spec = ispec.ObterSpec<TE>();
            var query = spec.ConverterParaIQueryable(Query);
            var taskList = await query.PaginarAsync<TE>(Servico, pagination, true);
            return await taskList.ToListAsync();
        }

        public override async Task<int> QuantidadeAsync(TE entity, bool incluirExcluidosLogicamente = false)
        {
            var sql = RepositoryUtil.GetKeyAndDnUniqueKeyFilterSql(entity);
            return await CountSqlAsync(sql, incluirExcluidosLogicamente);
        }

        public override async Task<int> QuantidadeAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec)
        {
            if (spec.TipoDeEntidade != typeof(TE))
            {
                var serviceName = $"{spec.TipoDeEntidade.Name}Servico";
                throw new DesenvolvimentoIncorretoException($"The type of input reported in the {spec} specification is not the same as that requested in the repository request.\r\nSpecification type: {spec.TipoDeEntidade}.\r\nRequisition Tipo: {typeof(TE)}\r\nThis usually occurs when you make use of the wrong service. Make sure that when invoking the method that is causing this error you are making use of the service: {serviceName}");
            }

            return await spec.ObterSpecAlternativo<TE, TO>().ConverterParaIQueryable(Query).CountAsync();
        }

        public override async Task<int> QuantidadeAsync(IDnEspecificacao spec)
        {
            return await spec.ObterSpec<TE>().ConverterParaIQueryable(Query).CountAsync();
        }

        public override async Task<int> QuantidadeTotalAsync()
        {
            return await Query.CountAsync();
        }

        public override async Task<bool> HaSomenteUmAsync(TE entity, bool incluirExcluidosLogicamente = false)
        {
            var sql = RepositoryUtil.GetKeyAndDnUniqueKeyFilterSql(entity);
            return await CountSqlAsync(sql, incluirExcluidosLogicamente) == 1;
        }

        #region SPEC TE

        public override async Task<TE> PrimeiroOuPadraoAsync(IDnEspecificacao spec)
        {
            var val = spec.ObterSpec<TE>().ConverterParaIQueryable(Query);
            return await val.FirstOrDefaultAsync();
        }

        public override async Task<TO> UnicoOuPadraoAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec)
        {
            var query = spec.ObterSpecAlternativo<TE, TO>().ConverterParaIQueryable(Query);
            return await query.FirstOrDefaultAsync();
        }

        public override async Task<TE> SingleOrDefaultAsync(IDnEspecificacao spec)
        {
            var val = spec.ObterSpec<TE>().ConverterParaIQueryable(Query);

            try
            {
                return await val.SingleOrDefaultAsync();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"More than one record was found with the given keys. This is an indication of data with duplicate keys in the database. The table is {typeof(TE).GetTableName()}");
            }
        }

        #endregion

        public override async Task<List<TO>> ListarAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> ispec, DnPaginacao pagination = null)
        {
            var spec = ispec.ObterSpecAlternativo<TE, TO>();
            var query = spec.ConverterParaIQueryable(Query);
            var DnPagination = await query.PaginarAsync<TO>(Servico, pagination, ef: true);
            return await DnPagination.ToListAsync();
        }

        public override async Task<TO> PrimeiroOuPadraoAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> ispec)
        {
            var spec = ispec.ObterSpecAlternativo<TE, TO>();
            var query = spec.ConverterParaIQueryable(Query);
            return await query.FirstOrDefaultAsync();
        }

        //public virtual Task ForEachAlternativoAsync<TO>(Expression<Func<TE, bool>> expression, Action<TO> action, CancellationToken cancellationToken = default)
        //{
        //    throw new NotImplementedException();
        //}

        public override async Task ForEachAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> ispec, Action<TO> action, CancellationToken cancellationToken = default)
        {
            var spec = ispec.ObterSpecAlternativo<TE, TO>();
            var query = spec.ConverterParaIQueryable(Query);
            await query.ForEachAsync(action, cancellationToken);
        }

        public override async Task ForEachAsync(Expression<Func<TE, bool>> expression, Action<TE> action, CancellationToken cancellationToken = default) =>
                                await Query.Where(expression).AsNoTracking().ForEachAsync(action, cancellationToken);

        internal protected async Task<bool> ExistsSqlAsync(string sql, bool incluirExcluidosLogicamente = false)
        {
            if (incluirExcluidosLogicamente)
            {
                lock (SessionRequest)
                {
                    Session.EnableLogicalDeletion = false;
                }
            }

            var ret = await FromSql(sql).AnyAsync();

            lock (SessionRequest)
            {
                Session.EnableLogicalDeletion = true;
            }

            return ret;
        }

        internal protected async Task<TE> FindSingleOrDefaultSqlAsync(string sql)
        {
            try
            {
                return await FromSql(sql).SingleOrDefaultAsync();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException($"More than one record was found with the given keys. This is an indication of data with duplicate keys in the database. The table is {typeof(TE).GetTableName()}");
            }
        }

        #region ENTITY ITEMS

        public override async Task<TE> BuscarAsync(TE entity) => await FindSelectAsync(entity);

        public async Task<TO> FindSelectAsync<TO>(TO entity) where TO : DnEntidadeBase
        {
            {
                var sql = RepositoryUtil.GetKeyFilterSql(entity, out bool nonKeys);
                if (nonKeys == false)
                {
                    var valueFound = await FindSingleOrDefaultSqlAsync<TO>(sql);
                    if (valueFound != null)
                    {
                        return valueFound;
                    }
                }
            }

            {
                var sql = RepositoryUtil.GetDnUniqueKeyFilterSql(entity, out bool nonKeys);
                if (nonKeys == false)
                {
                    var valueFound = await FindSingleOrDefaultSqlAsync<TO>(sql);
                    if (valueFound != null)
                    {
                        return valueFound;
                    }
                }
            }

            return null;
        }

        public override async Task<bool> ExisteAsync(TE entity, bool incluirExcluidosLogicamente = false)
        {
            var sql = RepositoryUtil.GetKeyAndDnUniqueKeyFilterSql(entity);
            return await ExistsSqlAsync(sql, incluirExcluidosLogicamente);
        }

        public override async Task AdicionarListaAsync(params TE[] entities)
        {
            RunTheContextValidation();
            foreach (var entity in entities)
            {
                DefineForeignKeyOfCompositionsOrAggregations(entity);
                await UpdateCompositionListAsync(entity, false);
            }

            await Input.AddRangeAsync(entities);
        }

        public override async Task<TE> AdicionarAsync(TE entity)
        {
            RunTheContextValidation();
            DefineForeignKeyOfCompositionsOrAggregations(entity);
            await UpdateCompositionListAsync(entity, false);
            await CompleteEmptyKeysAsync(entity);
            var ret = await Input.AddAsync(entity);
            return ret.Entity;
        }

        #endregion

        protected async Task<int> ExecuteSqlQueryAsync(string query)
        {
            using var command = Session.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;
            await Session.Database.OpenConnectionAsync();
            return await command.ExecuteNonQueryAsync();
        }

        public override async Task<TE> RemoverAsync(TE entity)
        {
            RunTheContextValidation();
            var teEntity = await Servico.BuscarAsync(entity, false);
            var ret = Input.Remove(teEntity).Entity;

            RemoveDnCompositionsAndDnAggregations(entity);
            return ret;
        }

        private void RemoveDnCompositionsAndDnAggregations(TE entity)
        {
            var type = entity.GetType();
            if (type.GetCustomAttribute<DnFormularioJsonAttribute>()?.EhTabelaIntermediaria == true) { return; }

            var compositionProperties = type.GetProperties().Where(x => x.IsDefined(typeof(DnComposicaoAttribute)) || x.IsDefined(typeof(DnAgregacaoDeMuitosParaMuitosAttribute)));
            foreach (var compositionProperty in compositionProperties)
            {
                var compositionPropertyType = compositionProperty.PropertyType;
                var compositionListElements = ListAllByForeignKey(entity, compositionPropertyType.GetListTypeNonNull());
                var dbSet = ObjetosTransacionais.ObterObjetoInputDataInterno(compositionPropertyType.GetListTypeNonNull());
                if (compositionListElements.Count > 0)
                {
                    var method = dbSet.GetType().GetMethods().Last(x => x.Name == "RemoverLista");
                    method.Invoke(dbSet, new[] { compositionListElements });
                }
            }
        }

        public override async Task EliminarTudoAsync()
        {
            var tableName = typeof(TE).GetTableName();
            var sql = $"TRUNCATE TABLE {tableName}";
            await ExecuteSqlQueryAsync(sql);
        }

        /// <summary>
        /// Exemplo:
        ///   public async Task<int> ProximoId()
        ///   {
        ///       int Leitor(DbDataReader reader)
        ///       {
        ///           return (int)reader[0];
        ///       }
        ///
        ///       return await RawSqlQueryAsync("SELECT TOP 10 Name, COUNT(*) FROM Users", Leitor).FirstOrDefault();
        ///   }
        /// </summary>
        /// <typeparam Nome="T"></typeparam>
        /// <param Nome="query"></param>
        /// <param Nome="map"></param>
        /// <returns></returns>
        protected async Task<List<T>> RawSqlQueryAsync<T>(string query, Func<DbDataReader, T> map)
        {
            using var command = Session.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            await Session.Database.OpenConnectionAsync();

            using var result = await command.ExecuteReaderAsync();
            var entities = new List<T>();

            while (await result.ReadAsync())
            {
                entities.Add(map(result));
            }

            return entities;
        }
    }
}