﻿using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo(@"dn32.infra.EntityFramework.SqlServer, PublicKey=00240000048000009400000006020000002400005253413100040000010001002d98533364f3b3fbd11e7a3f14cd73d169e1daabd62ba2d1e5bc6a48a9bc709a503960db0e76c190e7a8dcefaed037e539682d6a891b242ddb91a3ab20fbfa0c04fb6304c8903857e1ed75399850fca4037dd2c810749e75770e5d455e950ccb9d06cf6fea5f30b00557a29408ce4c45021c412eca32616f47809bfe2cf404cc")]
[assembly: InternalsVisibleTo(@"dn32.infra.EntityFramework.PostgreSQL, PublicKey=0024000004800000940000000602000000240000525341310004000001000100192d4ee01ba583399ab1d381c4301592f8520d29c628f3220e1550b2068e540e26886fa8d8b52618553f89fed1dccb18d5d3c07c548fca3c916a10823f411c23ef0e85bf0526ed94aa3cfbdf79a9595861348cfc369670f8ed9f7c4afd08de5f3cd87a0c7c6b1d8a0b94622c163a764813ba95d39dc44ea1baf7b663800a49bc")]
[assembly: InternalsVisibleTo(@"dn32.infra.EntityFramework.MySQL, PublicKey=0024000004800000940000000602000000240000525341310004000001000100617593ae2b67e94c33ea38be9727f7a4a0e18fe316ea3cddceaaadd51d47546be3f27dc1d1c6c84d0a0cb43db45a7c476479c7ebd881d76b5dad404cafd086743036bd3c929dbf14c759ff504d798ca1097eb96b02dde75ee1bc120adc0e94553298c8749271502eb50cb427db851b1a26044bcb8e8fae1acf106069d2a349c0")]
namespace dn32.infra
{
    /// <inheritdoc />
    /// <summary>
    /// Repositório base com entidade do sistema baseado em Entidade Framework.
    /// </summary>
    /// <typeparam Nome="TE">
    /// O tipo de entidade do repositório.
    /// </typeparam>
    public partial class DnEFRepository<TE> : DnRepositorio<TE> where TE : DnEntidadeBase
    {
        public DnEFRepository() { }

        #region PROPERTIES

        public override Type TipoDeObjetosTransacionais => typeof(EFTransactionObjects);

        public SessaoDeRequisicaoDoUsuario SessionRequest => Servico.SessaoDaRequisicao;

        /// <summary>
        /// A referência da sessão do EF.
        /// </summary>
        protected internal EfContext Session => ObjetosTransacionais.contexto as EfContext;

        /// <summary>
        /// A query contem a referência de todas as tabelas/documentos do banco de dados.
        /// </summary>
        protected internal IQueryable<TE> Query => this.ObjetosTransacionais.ObterObjetoQueryInterno<TE>();

        /// <summary>
        /// A referência de input de dados para o banco de dados.
        /// </summary>
        internal DbSet<TE> Input => this.ObjetosTransacionais.ObterObjetoInputInterno<TE>() as DbSet<TE>;

        // DnControladorDeServico<TE> IDnRepository<TE>.Servico { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        internal protected void RunTheContextValidation() => Servico.SessaoDaRequisicao.ContextoDeValidacao.Validate();

        #endregion

        public override void Inicializar()
        {
        }

        public override TE Desanexar(TE entity) => Desanexar<TE>(entity);

        public override TX Desanexar<TX>(TX entity)
        {
            if (entity != null)
            {
                if (entity.IsDnEntity())
                {
                    Session.Entry(entity).State = EntityState.Detached;
                }
            }

            return entity;
        }

        #region COMPOSITION

        /* Unmerged change from project 'dn32.infra.EntityFramework (netcoreapp3.1)'
        Before:
                protected void UpdateCompositionList(TE entity)
        After:
                protected void UpdateCompositionListAsync(TE entity)
        */
        protected async Task UpdateCompositionListAsync(TE entity, bool isUpdate)
        {
            var compositionProperties = entity.GetType().GetProperties().Where(x => x.IsDefined(typeof(DnComposicaoAttribute)));
            foreach (var compositionProperty in compositionProperties)
            {
                var attr = compositionProperty.GetCustomAttribute<DnComposicaoAttribute>();
                if (attr?.OperacaoAoSalvar == EnumTipoDeOperacaoParaComAsReferencias.Ignorar) { continue; }
                if (attr?.OperacaoAoSalvar == EnumTipoDeOperacaoParaComAsReferencias.Adicionar && isUpdate) { continue; }

                var compositionValue = compositionProperty.GetValue(entity);

                var compositionPropertyType = compositionProperty.PropertyType;

                if (compositionPropertyType.IsList())
                {
                    var compositionListValue = compositionValue.DnCast<IList>();

                    if (isUpdate)
                    {
                        var listType = compositionPropertyType.GenericTypeArguments[0];
                        var allPersistedForThisEntity = ListAllByForeignKey(entity, listType).DnCast<IList>();

                        var allPersistedForThisEntityForRemove = allPersistedForThisEntity;
                        if (compositionListValue != null)
                        {
                            foreach (var item in compositionListValue)
                            {
                                await CompleteEmptyKeysAsync(item);
                                allPersistedForThisEntityForRemove.Remove(item);
                            }
                        }

                        if (allPersistedForThisEntityForRemove.Count > 0)
                        {
                            foreach (var entityToRemove in allPersistedForThisEntityForRemove)
                            {
                                Session.Remove(entityToRemove);
                            }
                        }
                    }

                    if (compositionListValue != null)
                    {
                        foreach (var auth in compositionListValue)
                        {
                            lock (SessionRequest) { Session.EnableLogicalDeletion = false; }
                            var currentEntity = await FindAsync(auth);
                            lock (SessionRequest) { Session.EnableLogicalDeletion = true; }

                            if (currentEntity == null)
                            { //Adicionar
                                Session.Add(auth);
                            }
                            else
                            { //update
                                ((DbContext)ObjetosTransacionais.contexto).Entry(currentEntity).CurrentValues.SetValues(auth);
                            }
                        }
                    }
                }
                else
                {
                    await CompleteEmptyKeysAsync(compositionValue);

                    var list = ListAllByForeignKey(entity, compositionPropertyType).DnCast<IList>();
                    var currentEntity = list.Count == 1 ? list[0] : null;

                    if (currentEntity == null)
                    {
                        if (compositionValue == null)
                        { // Não tem no bd e nem no objeto
                            continue;
                        }
                        else
                        { // Não tem no BD e precisa adicionar
                            Session.Add(compositionValue);
                            continue;
                        }
                    }
                    else
                    {
                        if (compositionValue == null)
                        { // Tem no bd, mas precisa ser removido
                            Session.RemoveRange(currentEntity);
                            continue;
                        }
                        else
                        { // Tem no bd e precisa ser atualizado

                            var keyProperties = currentEntity.GetType().GetProperties().Where(x => x.IsDefined(typeof(KeyAttribute))).ToList();
                            foreach (var p in keyProperties)
                            {
                                var value = p.GetValue(currentEntity);
                                if (value != null)
                                {
                                    p.SetValue(compositionValue, value);
                                }
                            }

                          ((DbContext)ObjetosTransacionais.contexto).Entry(currentEntity).CurrentValues.SetValues(compositionValue);
                            continue;
                        }
                    }
                }
            }
        }

        protected void DefineForeignKeyOfCompositionsOrAggregations(TE entity)
        {
            var localProperties = entity.GetType().GetProperties();

            localProperties.ToList().ForEach(LocalProperty =>
            {
                var composition = LocalProperty.GetCustomAttribute<DnReferenciaAttribute>(true);
                if (composition == null) { return; }
                var externalProperties = LocalProperty.PropertyType.GetListTypeNonNull().GetProperties();
                var externalValue = LocalProperty.GetValue(entity);

                if (composition.ChavesExternas == null) throw new DesenvolvimentoIncorretoException(entity.GetType().Name + "- When indicating an aggregation or composition attribute, it is necessary to inform the properties {ChavesExternas}");

                for (int i = 0; i < composition.ChavesExternas.Length; i++)
                {
                    var externalKey = composition.ChavesExternas[i];
                    var localKey = composition.ChavesLocais[i];
                    var externalKeyProperty = externalProperties.Single(x => x.Name == externalKey);
                    var localKeyProperty = localProperties.Single(x => x.Name == localKey);

                    var localKeyValue = localKeyProperty.GetValue(entity);

                    object externalKeyValue = null;

                    if (!LocalProperty.PropertyType.IsList())
                    {
                        externalKeyValue = externalValue == null ? null : externalKeyProperty.GetValue(externalValue);
                    }

                    for (int i2 = 0; i < composition.ChavesExternas.Length; i++)
                    {
                        var ext = composition.ChavesExternas[i2];
                        var loca = composition.ChavesLocais[i2];

                        if (LocalProperty.PropertyType.IsList())
                        {
                            if (LocalProperty.GetValue(entity) is ICollection List)
                            {
                                foreach (var item in List)
                                {
                                    var property = item?.GetType().GetProperty(ext);
                                    property?.SetValue(item, localKeyValue);
                                }
                            }
                        }
                        else
                        {
                            if (localKeyProperty.IsKey())
                            {
                                if (localKeyValue != null && externalValue != null)
                                {
                                    externalKeyProperty.SetValue(externalValue, localKeyValue);
                                }
                            }
                            else
                            {
                                if (localKeyValue != null && externalValue != null)
                                {
                                    localKeyProperty.SetValue(entity, externalKeyValue);
                                }

                                if (composition is DnAgregacaoAttribute aggre && externalValue != null)
                                {
                                    if (Session.Entry(externalValue).State == EntityState.Added && aggre.PermitirAdicionar)
                                    {
                                        continue;
                                    }

                                    Session.Entry(externalValue).State = EntityState.Unchanged;
                                }
                            }
                        }
                    }
                }
            });
        }

        private async Task CompleteEmptyKeysAsync(object compositionValue)
        {
            if (compositionValue == null) { return; }
            var keyPoroperties = compositionValue.GetType().GetProperties().Where(x => x.IsDefined(typeof(DnCriarValorRandomicoAoAdicionarEntidadeAttribute))).ToList();
            //Todo - Permitir esse atributo somente em tipos primitivos
            foreach (var property in keyPoroperties)
            {
                var type = property.PropertyType;
                var value = property.GetValue(compositionValue);
                if (value.IsDnNull() || value.DnEquals(type.GetDnDefaultValue()))
                {
                    if (GetExistinEntityCode(compositionValue, property)) { return; }
                    var attribute = property.GetCustomAttribute<DnCriarValorRandomicoAoAdicionarEntidadeAttribute>();
                    if (attribute == null) { continue; }
                    await GenerateNewEntityCodes(compositionValue, property, attribute.TamanhoMaximo);
                }
            }
        }

        private async Task GenerateNewEntityCodes(object compositionValue, PropertyInfo property, int max = 0)
        {
            List<object> notExists;
            do
            {
                var list = new object[10];
                for (int i = 0; i < list.Length; i++)
                {
                    list[i] = UtilitarioDeRandomico.GetRandomValue(property, max);
                }

                notExists = await ExistOnListAsync(property, compositionValue.GetType(), list); // Verificar se esse cast vai funcionar
            }
            while (notExists.Count == 0);

            var value = notExists.Next();
            SessionRequest.SetCodeAvailableForEntity(compositionValue.GetType().FullName + property.Name, notExists);
            property.SetValue(compositionValue, value);
        }

        private bool GetExistinEntityCode(object compositionValue, PropertyInfo property)
        {
            var code = SessionRequest.GetCodeAvailableForEntity(compositionValue.GetType().FullName + property.Name);
            if (code != null)
            {
                property.SetValue(compositionValue, code);
            }

            return code != null;
        }

        #endregion

        #region SQL

        internal async Task<List<object>> ExistOnListAsync(PropertyInfo property, Type dbEntityType, object[] elements)
        {
            var outType = property.PropertyType;
            var sql = RepositoryUtil.ListToInSql(dbEntityType, elements, property);

            static object reader(DbDataReader reader)
            {
                return reader[0];
            }

            var list = await RawSqlQueryAsync(sql, reader);
            return elements.Except(list).ToList();
        }

        private IQueryable FromSqlByType(string sql, Type dbEntityType, params object[] parameters)
        {
            return GetType().GetMethod(nameof(FromSqlSelect), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.MakeGenericMethod(dbEntityType).Invoke(this, new object[] { sql, parameters }).DnCast<IQueryable>();
        }

        internal protected IQueryable<TE> FromSql(string sql, params object[] parameters)
        {
            return FromSqlSelect<TE>(sql, parameters);
        }

        internal protected IQueryable<TO> FromSqlSelect<TO>(string sql, params object[] parameters) where TO : DnEntidadeBase
        {
            var source = ObjetosTransacionais.ObterObjetoInputInterno<TO>() as DbSet<TO>;
            return source.FromSqlRaw(sql, parameters);
        }

        internal protected ICollection ListAllNotPaginate(string sql, Type dbEntityType)
        {
            var query = FromSqlByType(sql, dbEntityType);
            return typeof(Enumerable).GetMethod(nameof(Enumerable.ToList))?.MakeGenericMethod(dbEntityType).Invoke(null, new object[] { query }).DnCast<ICollection>();
        }

        #endregion

        #region ENTITY ITEMS

        public override async Task<object> FindAsync(object entity)
        {
            var type = entity.GetType();
            var method = GetType().GetMethod(nameof(FindSelectAsync))?.MakeGenericMethod(type);
            dynamic task = method?.Invoke(this, new object[] { entity });
            return await task;
        }

        public virtual ICollection ListAllByForeignKey(TE entity, Type dnEntityType)
        {
            var sql = RepositoryUtil.GetForeignKeyFilterSql(entity, dnEntityType, out bool nonKeys);
            if (nonKeys == false)
            {
                return ListAllNotPaginate(sql, dnEntityType);
            }

            return default;
        }

        /// <summary>
        /// Atualiza um item do banco de dados baseado em seu identificador.
        /// </summary>
        /// <param Nome="entity">
        /// Entidade a ser atualizada com o identificador preenchido.
        /// </param>

        public override async Task<TE> AtualizarAsync(TE entity)
        {
            RunTheContextValidation();

            DefineForeignKeyOfCompositionsOrAggregations(entity);

            lock (SessionRequest)
            {
                Session.EnableLogicalDeletion = false;
            }

            var currentEntity = await Servico.BuscarAsync(entity, true, false);
            if (currentEntity == null) throw new InvalidOperationException("Entidade não encontrada no banco de dados.");

            ((DbContext)ObjetosTransacionais.contexto).Entry(currentEntity).CurrentValues.SetValues(entity);

            lock (SessionRequest)
            {
                Session.EnableLogicalDeletion = true;
            }

            await UpdateCompositionListAsync(entity, true);

            return entity;
        }

        //Todo - tratar recuperação de exclusão lógica, como foi feito no Atualizar
        public override async Task AtualizarListaAsync(IEnumerable<TE> entities)
        {
            RunTheContextValidation();
            foreach (var entity in entities)
            {
                DefineForeignKeyOfCompositionsOrAggregations(entity);
                var currentEntity = await Servico.BuscarAsync(entity);
                ((DbContext)ObjetosTransacionais.contexto).Entry(currentEntity).CurrentValues.SetValues(entity);
                await UpdateCompositionListAsync(entity, true);
            }
        }

        public override void RemoverLista(IDnEspecificacao spec)
        {
            var list = spec.ObterSpec<TE>().ConverterParaIQueryable(Query).ToList();
            this.Input.RemoveRange(list);
        }

        public override async Task RemoverListaAsync(params TE[] entities)
        {
            var tasks = entities.Select(RemoverAsync).ToArray();
            await Task.WhenAll(tasks);
        }

        #endregion

        #region INTERNAL

        // private static string CreateSqlFromKeys(TE entity)
        // {
        // var tableName = entity.GetTableName();
        // var keyValues = entity.GetKeyValues().Select(x => $"({x.Key} = {x.Valor} and {x.Key} != 0)").ToArray();
        // var sql = $"select * from {tableName} where ";
        // sql += string.Join(" and ", keyValues);
        // return sql;
        // }

        #endregion
    }
}
