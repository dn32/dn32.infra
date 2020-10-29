using Microsoft.Extensions.Primitives;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace dn32.infra
{
    /// <inheritdoc />
    /// <summary>
    /// Repositório base com entidade do sistema baseado em Entidade Framework.
    /// </summary>
    /// <typeparam Nome="TE">
    /// O tipo de entidade do repositório.
    /// </typeparam>
    public class DnRavenDbRepositorio<TE> : DnRepositorio<TE> where TE : RavenDBEntidadeBase
    {
        public DnRavenDbRepositorio() { }

        public RavenDBObjetosDeTransacao ObjetosTransacionaisRavenDB => ObjetosTransacionais as RavenDBObjetosDeTransacao;

        protected internal IAsyncDocumentSession Sessao => ObjetosTransacionaisRavenDB.Contexto.Sessao;

        protected internal IQueryable<TE> Query => this.ObjetosTransacionais.ObterObjetoQueryInterno<TE>();

        public override Type TipoDeObjetosTransacionais => typeof(RavenDBObjetosDeTransacao);


        public override void Inicializar()
        {
        }

        public override async Task<TE> AdicionarAsync(TE entity)
        {
            await Sessao.StoreAsync(entity);
            return entity;
        }

        public override async Task AdicionarListaAsync(TE[] entities)
        {
            foreach (var entity in entities)
                await Sessao.StoreAsync(entity, entity.Id);
        }

        public override Task<TE> AtualizarAsync(TE entity)
        {
            throw new NotImplementedException();
        }

        public override Task AtualizarListaAsync(IEnumerable<TE> entities)
        {
            throw new NotImplementedException();
        }

        public override Task<TE> BuscarAsync(TE entity)
        {
            throw new NotImplementedException();
        }

        public override Task EliminarTudoAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<bool> ExisteAlternativoAsync<TO>(IDnEspecificacaoBase spec)
        {
            throw new NotImplementedException();
        }

        private DnEspecificacaoAlternativa<TE, TO> GetSpecSelect<TO>(IDnEspecificacaoBase spec1)
        {
            if (spec1 is IDnEspecificacaoAlternativaGenerica<TO> spec)
            {
                if (spec.TipoDeEntidade != typeof(TE))
                {
                    var serviceName = $"{spec.TipoDeEntidade.Name}Servico";
                    throw new DesenvolvimentoIncorretoException($"The type of input reported in the {spec} specification is not the same as that requested in the repository request.\r\nSpecification type: {spec.TipoDeEntidade}.\r\nRequisition Tipo: {typeof(TE)}\r\nThis usually occurs when you make use of the wrong service. Make sure that when invoking the method that is causing this error you are making use of the service: {serviceName}");
                }

                if (spec.TipoDeRetorno != typeof(TO))
                {
                    var serviceName = $"{typeof(TE).Name}Servico";
                    throw new DesenvolvimentoIncorretoException($"The type of output reported in the {spec} specification is not the same as that requested in the repository request.\r\nSpecification type: {spec.TipoDeEntidade}.\r\nRequisition Tipo: {typeof(TO)}\r\nThis usually occurs when you make use of the wrong service. Make sure that when invoking the method that is causing this error you are making use of the service: {serviceName}");
                }

                return spec as DnEspecificacaoAlternativa<TE, TO>;
            }

            throw new DesenvolvimentoIncorretoException("The specification is of a different type than expected");
        }

        public new RavenDBObjetosDeTransacao ObjetosTransacionais => base.ObjetosTransacionais as RavenDBObjetosDeTransacao;

        public override async Task<bool> ExisteAsync(IDnEspecificacaoBase spec)
        {
            var existe = await spec.ObterSpec<TE>().ConverterParaIQueryable(Query).AnyAsync();
            return existe;
        }

        public override async Task<TE> SingleOrDefaultAsync(IDnEspecificacao spec)
        {
            var entidade = await spec.ObterSpec<TE>().ConverterParaIQueryable(Query).FirstOrDefaultAsync();
            return entidade;
        }

        public override TE Desanexar(TE entity) => entity;

        public override TX Desanexar<TX>(TX entity) => entity;

        public override async Task<bool> ExisteAsync(TE entity, bool incluirExcluidosLogicamente = false)
        {
            return await Query.AnyAsync(x => x.Id == entity.Id);
        }

        public override Task<bool> HaSomenteUmAsync(TE entity, bool incluirExcluidosLogicamente)
        {
            throw new NotImplementedException();
        }

        public override Task<List<TO>> ListarAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> ispec, DnPaginacao pagination = null)
        {
            throw new NotImplementedException();
        }

        public override Task<TO> PrimeiroOuPadraoAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec)
        {
            throw new NotImplementedException();
        }

        public override Task<TE> PrimeiroOuPadraoAsync(IDnEspecificacao spec)
        {
            throw new NotImplementedException();
        }

        public override Task<int> QuantidadeAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec)
        {
            throw new NotImplementedException();
        }

        public override Task<int> QuantidadeAsync(TE entity, bool incluirExcluidosLogicamente)
        {
            throw new NotImplementedException();
        }

        public override Task<int> QuantidadeAsync(IDnEspecificacao spec)
        {
            throw new NotImplementedException();
        }

        public override Task<int> QuantidadeTotalAsync()
        {
            throw new NotImplementedException();
        }

        public override Task<TE> RemoverAsync(TE entity)
        {
            throw new NotImplementedException();
        }

        public override void RemoverLista(IDnEspecificacao spec)
        {
            throw new NotImplementedException();
        }

        public override Task RemoverListaAsync(params TE[] entities)
        {
            throw new NotImplementedException();
        }

        public override Task<TO> UnicoOuPadraoAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> spec)
        {
            throw new NotImplementedException();
        }

        public override Task<object> FindAsync(object entity)
        {
            throw new NotImplementedException();
        }

        public override Task<List<TE>> ListarAsync(IDnEspecificacao spec, DnPaginacao pagination = null)
        {
            throw new NotImplementedException();
        }

        public override Task ForEachAsync(Expression<Func<TE, bool>> expression, Action<TE> action, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public override Task ForEachAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> ispec, Action<TO> action, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        //        #region PROPERTIES

        //        public IDnObjetosTransacionais ObjetosTransacionais { get; set; }

        //        public virtual Type TipoDeObjetosTransacionais => typeof(TransactionObjects);

        //        public SessaoDeRequisicaoDoUsuario SessionRequest => Servico.SessaoDaRequisicao;

        //        /// <summary>
        //        /// A referência da sessão do EF.
        //        /// </summary>
        //        protected internal EfContext Session => ObjetosTransacionais.Sessao as EfContext;

        //        /// <summary>
        //        /// A query contem a referência de todas as tabelas/documentos do banco de dados.
        //        /// </summary>
        //        protected internal IQueryable<TE> Query => this.ObjetosTransacionais.ObterObjetoQueryInterno<TE>();

        //        /// <summary>
        //        /// A referência de input de dados para o banco de dados.
        //        /// </summary>
        //        internal DbSet<TE> Input => this.ObjetosTransacionais.ObterObjetoInputInterno<TE>() as DbSet<TE>;

        //        /// <summary>
        //        /// O serviço qual esse repositório representa.
        //        /// </summary>
        //        public DnServico<TE> Servico { get; set; }

        //        // DnControladorDeServico<TE> IDnRepository<TE>.Servico { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        //        internal protected void RunTheContextValidation() => Servico.SessaoDaRequisicao.ContextoDeValidacao.Validate();

        //        #endregion

        //        public virtual TE Desanexar(TE entity) => Desanexar<TE>(entity);

        //        public virtual TX Desanexar<TX>(TX entity)
        //        {
        //            if (entity != null)
        //            {
        //                if (entity.IsDnEntity())
        //                {
        //                    Session.Entry(entity).State = EntityState.Detached;
        //                }
        //            }

        //            return entity;
        //        }

        //        #region COMPOSITION

        //        /* Unmerged change from project 'dn32.infra.EntityFramework (netcoreapp3.1)'
        //        Before:
        //                protected void UpdateCompositionList(TE entity)
        //        After:
        //                protected void UpdateCompositionListAsync(TE entity)
        //        */
        //        protected async Task UpdateCompositionListAsync(TE entity, bool isUpdate)
        //        {
        //            var compositionProperties = entity.GetType().GetProperties().Where(x => x.IsDefined(typeof(DnComposicaoAttribute)));
        //            foreach (var compositionProperty in compositionProperties)
        //            {
        //                var attr = compositionProperty.GetCustomAttribute<DnComposicaoAttribute>();
        //                if (attr?.OperacaoAoSalvar == EnumTipoDeOperacaoParaComAsReferencias.Ignorar) { continue; }
        //                if (attr?.OperacaoAoSalvar == EnumTipoDeOperacaoParaComAsReferencias.Adicionar && isUpdate) { continue; }

        //                var compositionValue = compositionProperty.GetValue(entity);

        //                var compositionPropertyType = compositionProperty.PropertyType;

        //                if (compositionPropertyType.IsList())
        //                {
        //                    var compositionListValue = compositionValue.DnCast<IList>();

        //                    if (isUpdate)
        //                    {
        //                        var listType = compositionPropertyType.GenericTypeArguments[0];
        //                        var allPersistedForThisEntity = ListAllByForeignKey(entity, listType).DnCast<IList>();

        //                        var allPersistedForThisEntityForRemove = allPersistedForThisEntity;
        //                        if (compositionListValue != null)
        //                        {
        //                            foreach (var item in compositionListValue)
        //                            {
        //                                await CompleteEmptyKeysAsync(item);
        //                                allPersistedForThisEntityForRemove.Remove(item);
        //                            }
        //                        }

        //                        if (allPersistedForThisEntityForRemove.Count > 0)
        //                        {
        //                            foreach (var entityToRemove in allPersistedForThisEntityForRemove)
        //                            {
        //                                Session.Remove(entityToRemove);
        //                            }
        //                        }
        //                    }

        //                    if (compositionListValue != null)
        //                    {
        //                        foreach (var auth in compositionListValue)
        //                        {
        //                            lock (SessionRequest) { Session.EnableLogicalDeletion = false; }
        //                            var currentEntity = await FindAsync(auth);
        //                            lock (SessionRequest) { Session.EnableLogicalDeletion = true; }

        //                            if (currentEntity == null)
        //                            { //Adicionar
        //                                Session.Add(auth);
        //                            }
        //                            else
        //                            { //update
        //                                ObjetosTransacionais.Sessao.Entry(currentEntity).CurrentValues.SetValues(auth);
        //                            }
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    await CompleteEmptyKeysAsync(compositionValue);

        //                    var list = ListAllByForeignKey(entity, compositionPropertyType).DnCast<IList>();
        //                    var currentEntity = list.Count == 1 ? list[0] : null;

        //                    if (currentEntity == null)
        //                    {
        //                        if (compositionValue == null)
        //                        { // Não tem no bd e nem no objeto
        //                            continue;
        //                        }
        //                        else
        //                        { // Não tem no BD e precisa adicionar
        //                            Session.Add(compositionValue);
        //                            continue;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (compositionValue == null)
        //                        { // Tem no bd, mas precisa ser removido
        //                            Session.RemoveRange(currentEntity);
        //                            continue;
        //                        }
        //                        else
        //                        { // Tem no bd e precisa ser atualizado

        //                            var keyProperties = currentEntity.GetType().GetProperties().Where(x => x.IsDefined(typeof(KeyAttribute))).ToList();
        //                            foreach (var p in keyProperties)
        //                            {
        //                                var value = p.GetValue(currentEntity);
        //                                if (value != null)
        //                                {
        //                                    p.SetValue(compositionValue, value);
        //                                }
        //                            }

        //                            ObjetosTransacionais.Sessao.Entry(currentEntity).CurrentValues.SetValues(compositionValue);
        //                            continue;
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        protected void DefineForeignKeyOfCompositionsOrAggregations(TE entity)
        //        {
        //            var localProperties = entity.GetType().GetProperties();

        //            localProperties.ToList().ForEach(LocalProperty =>
        //            {
        //                var composition = LocalProperty.GetCustomAttribute<DnReferenciaAttribute>(true);
        //                if (composition == null) { return; }
        //                var externalProperties = LocalProperty.PropertyType.GetListTypeNonNull().GetProperties();
        //                var externalValue = LocalProperty.GetValue(entity);

        //                if (composition.ChavesExternas == null) throw new DesenvolvimentoIncorretoException(entity.GetType().Name + "- When indicating an aggregation or composition attribute, it is necessary to inform the properties {ChavesExternas}");

        //                for (int i = 0; i < composition.ChavesExternas.Length; i++)
        //                {
        //                    var externalKey = composition.ChavesExternas[i];
        //                    var localKey = composition.ChavesLocais[i];
        //                    var externalKeyProperty = externalProperties.Single(x => x.Name == externalKey);
        //                    var localKeyProperty = localProperties.Single(x => x.Name == localKey);

        //                    var localKeyValue = localKeyProperty.GetValue(entity);

        //                    object externalKeyValue = null;

        //                    if (!LocalProperty.PropertyType.IsList())
        //                    {
        //                        externalKeyValue = externalValue == null ? null : externalKeyProperty.GetValue(externalValue);
        //                    }

        //                    for (int i2 = 0; i < composition.ChavesExternas.Length; i++)
        //                    {
        //                        var ext = composition.ChavesExternas[i2];
        //                        var loca = composition.ChavesLocais[i2];

        //                        if (LocalProperty.PropertyType.IsList())
        //                        {
        //                            if (LocalProperty.GetValue(entity) is ICollection List)
        //                            {
        //                                foreach (var item in List)
        //                                {
        //                                    var property = item?.GetType().GetProperty(ext);
        //                                    property?.SetValue(item, localKeyValue);
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            if (localKeyProperty.IsKey())
        //                            {
        //                                if (localKeyValue != null && externalValue != null)
        //                                {
        //                                    externalKeyProperty.SetValue(externalValue, localKeyValue);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                if (localKeyValue != null && externalValue != null)
        //                                {
        //                                    localKeyProperty.SetValue(entity, externalKeyValue);
        //                                }

        //                                if (composition is DnAgregacaoAttribute aggre && externalValue != null)
        //                                {
        //                                    if (Session.Entry(externalValue).State == EntityState.Added && aggre.PermitirAdicionar)
        //                                    {
        //                                        continue;
        //                                    }

        //                                    Session.Entry(externalValue).State = EntityState.Unchanged;
        //                                }
        //                            }
        //                        }
        //                    }
        //                }
        //            });
        //        }

        //        private async Task CompleteEmptyKeysAsync(object compositionValue)
        //        {
        //            if (compositionValue == null) { return; }
        //            var keyPoroperties = compositionValue.GetType().GetProperties().Where(x => x.IsDefined(typeof(DnCriarValorRandomicoAoAdicionarEntidadeAttribute))).ToList();
        //            //Todo - Permitir esse atributo somente em tipos primitivos
        //            foreach (var property in keyPoroperties)
        //            {
        //                var type = property.PropertyType;
        //                var value = property.GetValue(compositionValue);
        //                if (value.IsDnNull() || value.DnEquals(type.GetDnDefaultValue()))
        //                {
        //                    if (GetExistinEntityCode(compositionValue, property)) { return; }
        //                    var attribute = property.GetCustomAttribute<DnCriarValorRandomicoAoAdicionarEntidadeAttribute>();
        //                    if (attribute == null) { continue; }
        //                    await GenerateNewEntityCodes(compositionValue, property, attribute.TamanhoMaximo);
        //                }
        //            }
        //        }

        //        private async Task GenerateNewEntityCodes(object compositionValue, PropertyInfo property, int max = 0)
        //        {
        //            List<object> notExists;
        //            do
        //            {
        //                var list = new object[10];
        //                for (int i = 0; i < list.Length; i++)
        //                {
        //                    list[i] = UtilitarioDeRandomico.GetRandomValue(property, max);
        //                }

        //                notExists = await ExistOnListAsync(property, compositionValue.GetType(), list); // Verificar se esse cast vai funcionar
        //            }
        //            while (notExists.Count == 0);

        //            var value = notExists.Next();
        //            SessionRequest.SetCodeAvailableForEntity(compositionValue.GetType().FullName + property.Name, notExists);
        //            property.SetValue(compositionValue, value);
        //        }

        //        private bool GetExistinEntityCode(object compositionValue, PropertyInfo property)
        //        {
        //            var code = SessionRequest.GetCodeAvailableForEntity(compositionValue.GetType().FullName + property.Name);
        //            if (code != null)
        //            {
        //                property.SetValue(compositionValue, code);
        //            }

        //            return code != null;
        //        }

        //        #endregion

        //        #region SQL

        //        internal async Task<List<object>> ExistOnListAsync(PropertyInfo property, Type dbEntityType, object[] elements)
        //        {
        //            var outType = property.PropertyType;
        //            var sql = RepositoryUtil.ListToInSql(dbEntityType, elements, property);

        //            static object reader(DbDataReader reader)
        //            {
        //                return reader[0];
        //            }

        //            var list = await RawSqlQueryAsync(sql, reader);
        //            return elements.Except(list).ToList();
        //        }

        //        private IQueryable FromSqlByType(string sql, Type dbEntityType, params object[] parameters)
        //        {
        //            return GetType().GetMethod(nameof(FromSqlSelect), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)?.MakeGenericMethod(dbEntityType).Invoke(this, new object[] { sql, parameters }).DnCast<IQueryable>();
        //        }

        //        internal protected IQueryable<TE> FromSql(string sql, params object[] parameters)
        //        {
        //            return FromSqlSelect<TE>(sql, parameters);
        //        }

        //        internal protected IQueryable<TO> FromSqlSelect<TO>(string sql, params object[] parameters) where TO : DnEntidadeBase
        //        {
        //            var source = ObjetosTransacionais.ObterObjetoInputInterno<TO>();

        //#if NETCOREAPP3_1
        //            return source.FromSqlRaw(sql, parameters);
        //#else
        //            return source.FromSql(sql, parameters);
        //#endif
        //        }

        //        internal protected ICollection ListAllNotPaginate(string sql, Type dbEntityType)
        //        {
        //            var query = FromSqlByType(sql, dbEntityType);
        //            return typeof(Enumerable).GetMethod(nameof(Enumerable.ToList))?.MakeGenericMethod(dbEntityType).Invoke(null, new object[] { query }).DnCast<ICollection>();
        //        }

        //        #endregion

        //        #region ENTITY ITEMS

        //        public virtual async Task<object> FindAsync(object entity)
        //        {
        //            var type = entity.GetType();
        //            var method = GetType().GetMethod(nameof(FindSelectAsync))?.MakeGenericMethod(type);
        //            dynamic task = method?.Invoke(this, new object[] { entity });
        //            return await task;
        //        }

        //        public virtual ICollection ListAllByForeignKey(TE entity, Type dnEntityType)
        //        {
        //            var sql = RepositoryUtil.GetForeignKeyFilterSql(entity, dnEntityType, out bool nonKeys);
        //            if (nonKeys == false)
        //            {
        //                return ListAllNotPaginate(sql, dnEntityType);
        //            }

        //            return default;
        //        }

        //        /// <summary>
        //        /// Atualiza um item do banco de dados baseado em seu identificador.
        //        /// </summary>
        //        /// <param Nome="entity">
        //        /// Entidade a ser atualizada com o identificador preenchido.
        //        /// </param>

        //        public virtual async Task<TE> AtualizarAsync(TE entity)
        //        {
        //            RunTheContextValidation();

        //            DefineForeignKeyOfCompositionsOrAggregations(entity);

        //            lock (SessionRequest)
        //            {
        //                Session.EnableLogicalDeletion = false;
        //            }

        //            var currentEntity = await Servico.BuscarAsync(entity, true, false);
        //            if (currentEntity == null) throw new InvalidOperationException("Entidade não encontrada no banco de dados.");

        //            ObjetosTransacionais.Sessao.Entry(currentEntity).CurrentValues.SetValues(entity);

        //            lock (SessionRequest)
        //            {
        //                Session.EnableLogicalDeletion = true;
        //            }

        //            await UpdateCompositionListAsync(entity, true);

        //            return entity;
        //        }

        //        //Todo - tratar recuperação de exclusão lógica, como foi feito no Atualizar
        //        public virtual async Task AtualizarListaAsync(IEnumerable<TE> entities)
        //        {
        //            RunTheContextValidation();
        //            foreach (var entity in entities)
        //            {
        //                DefineForeignKeyOfCompositionsOrAggregations(entity);
        //                var currentEntity = await Servico.BuscarAsync(entity);
        //                ObjetosTransacionais.Sessao.Entry(currentEntity).CurrentValues.SetValues(entity);
        //                await UpdateCompositionListAsync(entity, true);
        //            }
        //        }

        //        public virtual void RemoverLista(IDnEspecificacao spec)
        //        {
        //            var list = GetSpec(spec).ConverterParaIQueryable(Query).ToList();
        //            this.Input.RemoveRange(list);
        //        }

        //        public virtual async Task RemoverListaAsync(params TE[] entities)
        //        {
        //            var tasks = entities.Select(RemoverAsync).ToArray();
        //            await Task.WhenAll(tasks);
        //        }

        //        #endregion

        //        #region INTERNAL

        //        private DnEspecificacaoAlternativa<TE, TO> GetSpecSelect<TO>(IDnEspecificacaoBase spec1)
        //        {
        //            if (spec1 is IDnEspecificacaoAlternativaGenerica<TO> spec)
        //            {
        //                if (spec.TipoDeEntidade != typeof(TE))
        //                {
        //                    var serviceName = $"{spec.TipoDeEntidade.Name}Servico";
        //                    throw new DesenvolvimentoIncorretoException($"The type of input reported in the {spec} specification is not the same as that requested in the repository request.\r\nSpecification type: {spec.TipoDeEntidade}.\r\nRequisition Tipo: {typeof(TE)}\r\nThis usually occurs when you make use of the wrong service. Make sure that when invoking the method that is causing this error you are making use of the service: {serviceName}");
        //                }

        //                if (spec.TipoDeRetorno != typeof(TO))
        //                {
        //                    var serviceName = $"{typeof(TE).Name}Servico";
        //                    throw new DesenvolvimentoIncorretoException($"The type of output reported in the {spec} specification is not the same as that requested in the repository request.\r\nSpecification type: {spec.TipoDeEntidade}.\r\nRequisition Tipo: {typeof(TO)}\r\nThis usually occurs when you make use of the wrong service. Make sure that when invoking the method that is causing this error you are making use of the service: {serviceName}");
        //                }

        //                return spec as DnEspecificacaoAlternativa<TE, TO>;
        //            }

        //            throw new DesenvolvimentoIncorretoException("The specification is of a different type than expected");
        //        }

        //        protected DnEspecificacao<TE> GetSpec(IDnEspecificacaoBase spec1)
        //        {
        //            if (spec1 is DnEspecificacao<TE> spec)
        //            {
        //                return spec as DnEspecificacao<TE>;
        //            }

        //            throw new DesenvolvimentoIncorretoException("The specification is of a different type than expected");
        //        }

        //        // private static string CreateSqlFromKeys(TE entity)
        //        // {
        //        // var tableName = entity.GetTableName();
        //        // var keyValues = entity.GetKeyValues().Select(x => $"({x.Key} = {x.Valor} and {x.Key} != 0)").ToArray();
        //        // var sql = $"select * from {tableName} where ";
        //        // sql += string.Join(" and ", keyValues);
        //        // return sql;
        //        // }

        //        private DnPaginacao GetPagination()
        //        {
        //            var currentPageInt = int.TryParse(GetParameter(Parametros.NomePaginaAtual), out var currentPageInt_) ? currentPageInt_ : 0;
        //            var itemsPerPageInt = int.TryParse(GetParameter(Parametros.NomeItensPorPagina), out var itemsPerPageInt_) ? itemsPerPageInt_ : 20;
        //            var startAtZeroBool = !bool.TryParse(GetParameter(Parametros.NomeIniciarNaPaginaZero), out var startAtZeroBool_) || startAtZeroBool_;

        //            return DnPaginacao.Criar(currentPageInt, startAtZeroBool, itemsPerPageInt);
        //        }

        //        private string GetParameter(string key)
        //        {
        //            if (Servico.SessaoDaRequisicao.SessaoSemContexto) return string.Empty;

        //            Servico.SessaoDaRequisicao.LocalHttpContext.Request.Headers.TryGetValue(key, out StringValues value);
        //            if (!string.IsNullOrEmpty(value))
        //            {
        //                return value;
        //            }

        //            if (Servico.SessaoDaRequisicao.LocalHttpContext.Request.Method == "GET" || Servico.SessaoDaRequisicao.LocalHttpContext.Request.HasFormContentType == false)
        //            {
        //                return "";
        //            }

        //            return Servico.SessaoDaRequisicao.LocalHttpContext.Request.Form[key];
        //        }

        //        #endregion
    }
}