using dn32.infra.dados;
using dn32.infra.extensoes;
using dn32.infra.nucleo.interfaces;
using dn32.infra.nucleo.atributos;
using dn32.infra.Nucleo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using dn32.infra.excecoes;

namespace dn32.infra.EntityFramework
{
    public partial class DnEFRepository<TE>
    {
        internal protected async Task<int> CountSqlAsync(string sql, bool includeExcludedLogically = false)
        {
            if (includeExcludedLogically)
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

        internal protected async Task<TO> FindSingleOrDefaultSqlAsync<TO>(string sql) where TO : EntidadeBase
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
            return await GetSpec(spec).ConverterParaIQueryable(Query).AnyAsync();
        }

        public override async Task<bool> ExisteAlternativoAsync<TO>(IDnEspecificacaoBase spec)
        {
            return await GetSpecSelect<TO>(spec).ConverterParaIQueryable(Query).AnyAsync();
        }

        public override async Task<List<TE>> ListarAsync(IDnEspecificacao ispec, DnPaginacao pagination = null)
        {
            var spec = GetSpec(ispec);
            var query = spec.ConverterParaIQueryable(Query);
            var taskList = await DnPaginateAsync(query, pagination);
            return await taskList.ToListAsync();
        }

        public override async Task<int> QuantidadeAsync(TE entity, bool includeExcludedLogically = false)
        {
            var sql = RepositoryUtil.GetKeyAndDnUniqueKeyFilterSql(entity);
            return await CountSqlAsync(sql, includeExcludedLogically);
        }

        public override async Task<int> QuantidadeAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec)
        {
            if (spec.TipoDeEntidade != typeof(TE))
            {
                var serviceName = $"{spec.TipoDeEntidade.Name}Servico";
                throw new DesenvolvimentoIncorretoException($"The type of input reported in the {spec} specification is not the same as that requested in the repository request.\r\nSpecification type: {spec.TipoDeEntidade}.\r\nRequisition Tipo: {typeof(TE)}\r\nThis usually occurs when you make use of the wrong service. Make sure that when invoking the method that is causing this error you are making use of the service: {serviceName}");
            }

            return await GetSpecSelect<TO>(spec).ConverterParaIQueryable(Query).CountAsync();
        }

        public override async Task<int> QuantidadeAsync(IDnEspecificacao spec)
        {
            return await GetSpec(spec).ConverterParaIQueryable(Query).CountAsync();
        }

        public override async Task<int> QuantidadeTotalAsync()
        {
            return await Query.CountAsync();
        }

        public override async Task<bool> HaSomenteUmAsync(TE entity, bool includeExcludedLogically = false)
        {
            var sql = RepositoryUtil.GetKeyAndDnUniqueKeyFilterSql(entity);
            return await CountSqlAsync(sql, includeExcludedLogically) == 1;
        }


        #region SPEC TE

        public override async Task<TE> PrimeiroOuPadraoAsync(IDnEspecificacao spec)
        {
            var val = GetSpec(spec).ConverterParaIQueryable(Query);
            return await val.FirstOrDefaultAsync();
        }

        public override async Task<TO> UnicoOuPadraoAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec)
        {
            var query = GetSpecSelect<TO>(spec).ConverterParaIQueryable(Query);
            return await query.FirstOrDefaultAsync();
        }

        public override async Task<TE> SingleOrDefaultAsync(IDnEspecificacao spec)
        {
            var val = GetSpec(spec).ConverterParaIQueryable(Query);

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
            var spec = GetSpecSelect<TO>(ispec);
            var query = spec.ConverterParaIQueryable(Query);
            var DnPagination = await DnPaginateAsync(query, pagination);
            return await DnPagination.ToListAsync();
        }

        public override async Task<TO> PrimeiroOuPadraoAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> ispec)
        {
            var spec = GetSpecSelect<TO>(ispec);
            var query = spec.ConverterParaIQueryable(Query);
            return await query.FirstOrDefaultAsync();
        }

        internal protected async Task<bool> ExistsSqlAsync(string sql, bool includeExcludedLogically = false)
        {
            if (includeExcludedLogically)
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

        public async Task<TO> FindSelectAsync<TO>(TO entity) where TO : EntidadeBase
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

        public override async Task<bool> ExisteAsync(TE entity, bool includeExcludedLogically = false)
        {
            var sql = RepositoryUtil.GetKeyAndDnUniqueKeyFilterSql(entity);
            return await ExistsSqlAsync(sql, includeExcludedLogically);
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

        protected async Task<IQueryable<TX>> DnPaginateAsync<TX>(IQueryable<TX> query, DnPaginacao pagination = null)
        {
            if (pagination == null)
            {
                pagination = GetPagination() ?? DnPaginacao.Criar(0, true, 20);
            }

            pagination.QuantidadeTotalDeItens = await query.CountAsync();
            SessionRequest.Paginacao = pagination;

            return query.Skip(pagination.Salto).Take(pagination.ItensPorPagina);
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
            if (type.GetCustomAttribute<DnFormularioJsonAtributo>()?.EhTabelaIntermediaria == true) { return; }

            var compositionProperties = type.GetProperties().Where(x => x.IsDefined(typeof(DnComposicaoAtributo)) || x.IsDefined(typeof(DnAgregacaoDeMuitosParaMuitosAtributo)));
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
