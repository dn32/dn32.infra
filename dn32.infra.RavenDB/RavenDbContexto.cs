using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace dn32.infra {
    [DnTipoDeBancoDeDadosAtributo (EnumTipoDeBancoDeDados.RAVENDB)]
    public class RavenDbContexto : IDnDbContext {
        internal protected SessaoDeRequisicaoDoUsuario UserSessionRequest { get; internal set; }

        public bool EnableLogicalDeletion { get; set; }

        protected internal string ConnectionString { get; set; }

        internal IDocumentStore Store { get; set; }

        internal IAsyncDocumentSession Sessao { get; set; }

        public string NomeDoBD { get; set; }

        private string EnderecoDoCertificado { get; set; }

        public bool HaAlteracao => Sessao.Advanced.HasChanges;

        public RavenDbContexto (string connectionString) {
            ConnectionString = connectionString;
            Store = CreateDocumentStore ();
            Sessao = Store.OpenAsyncSession ();
            Sessao.Advanced.MaxNumberOfRequestsPerSession = 10000;
        }

        private IDocumentStore CreateDocumentStore () {
            NomeDoBD = Setup.ConfiguracoesGlobais.Valores["nomeDoBD"];
            EnderecoDoCertificado = Setup.ConfiguracoesGlobais.Valores["enderecoDoCertificado"];

            string serverURL = ConnectionString;
            string databaseName = NomeDoBD;

            var documentStore = new DocumentStore {
                Urls = new [] { serverURL },
                Database = databaseName
            };

            if (!string.IsNullOrWhiteSpace (EnderecoDoCertificado))
                documentStore.Certificate = new X509Certificate2 (EnderecoDoCertificado);

            documentStore.Initialize ();

            return documentStore;
        }

        public void Dispose () {
            Store.Dispose ();
            Sessao.Dispose ();
        }

        public async Task<int> SaveChangesAsync (CancellationToken cancellationToken = default) {
            await Sessao.SaveChangesAsync (cancellationToken);
            return 0;
        }

        //        internal protected delegate void EntityChangeEventHandler(ICollection<DnDadosDeEntidadeAlterada> DnEventEntity);
        //        internal protected event EntityChangeEventHandler EntityChangingEventEvent;
        //        internal protected event EntityChangeEventHandler EntityChangedEventEvent;

        //        protected internal string ConnectionString { get; set; }

        //        public EfContext(string connectionString)
        //        {
        //            ConnectionString = connectionString;
        //            //Database.EnsureDeleted();
        //            //Database.EnsureCreated();
        //            //Database.Migrate();
        //        }

        //        /// <summary>
        //        /// Todas as entidades de banco de dados são adicionados automaticamente.
        //        /// Use <see cref="NotDbEntityAttribute"/> se não desejar que uma entidade seja adicionada.
        //        /// </summary>
        //        /// <param Nome="modelBuilder">
        //        /// O model builder do EF.
        //        /// </param>
        //        protected override void OnModelCreating(ModelBuilder modelBuilder)
        //        {
        //            var exportedTypes = Setup.ObterEntidades().ToList();
        //            foreach (var type in exportedTypes)
        //            {
        //                if (type.IsDefined(typeof(NotMappedAttribute), false) || type.IsAbstract)
        //                {
        //                    continue;
        //                }

        //                if (type.IsSubclassOf(typeof(DnEntidade)))
        //                {
        //                    var keys = type.GetProperties().Where(x => x.GetCustomAttribute<KeyAttribute>(true) != null).Select(x => x.Name).ToArray();
        //                    if (keys.Length == 0)
        //                    {
        //                        throw new Exception($"The entity {type.Name} must contains a least one key");
        //                    }

        //                    var entity = modelBuilder.Entity(type);
        //                    entity.HasKey(keys);

        //                    if (UseLogicalDeletion(type, entity, modelBuilder))
        //                    {
        //                        entity.AddQueryFilter(IsAvailable());
        //                    }

        //                    SetEntity(entity, type);

        //                    var navigations = entity.Metadata.GetNavigations();
        //                    foreach (var property in type.GetProperties())
        //                    {
        //                        if (navigations.Any(x => x.Name == property.Name))
        //                        {
        //                            continue;
        //                        }

        //                        if (property.IsDefined(typeof(NotMappedAttribute), true))
        //                        {
        //                            entity.Ignore(property.Name);
        //                            continue;
        //                        }

        //                        if (property.PropertyType.IsNullableEnum())
        //                        {
        //                            if (property.PropertyType.IsNullable())
        //                            {
        //                                // Converte valores de enumeradores para null quando necessário
        //                                var method = GetType().GetMethod(nameof(ConvertNulableEnum), BindingFlags.NonPublic | BindingFlags.Instance)?.MakeGenericMethod(property.PropertyType.GetNonNullableType());
        //                                method?.Invoke(this, new object[] { entity, property });
        //                            }
        //                            else
        //                            {
        //                                if (!property.PropertyType.GetListTypeNonNull().IsDefined(typeof(DnUsarStringParaEnumeradoresNoBdAtributo)))
        //                                {
        //                                    entity.Property(property.Name).HasConversion<string>();// Converte os enumeradores para salvar o valor string no BD
        //                                }
        //                            }
        //                        }

        //                        SetEntityProperty(entity, type, property);
        //                    }
        //                }
        //            }

        //            base.OnModelCreating(modelBuilder);
        //        }

        //        protected void ConvertNulableEnum<TEnum>(EntityTypeBuilder entity, PropertyInfo property) where TEnum : Enum
        //        {
        //            if (typeof(TEnum).GetCustomAttribute<DnValorNuloParaEnumeradorAtributo>() is DnValorNuloParaEnumeradorAtributo DnEnumValueForSetNullAttribute)
        //            {
        //                ValueConverter converter = null;

        //                if (property.PropertyType.GetListTypeNonNull().IsDefined(typeof(DnUsarStringParaEnumeradoresNoBdAtributo)))
        //                {
        //                    converter = new ValueConverter<TEnum, int?>(v => v.GetHashCode() == DnEnumValueForSetNullAttribute.Valor ? null : (int?)v.GetHashCode(), v => (TEnum)Enum.ToObject(typeof(TEnum), v ?? 0));
        //                }
        //                else
        //                {
        //                    converter = new ValueConverter<TEnum, string>(v => v.GetHashCode() == DnEnumValueForSetNullAttribute.Valor ? null : v.ToString(), v => (TEnum)Enum.Parse(typeof(TEnum), v));
        //                }

        //                entity.Property(property.Name).HasConversion(converter);
        //            }
        //            else
        //            {
        //                if (!property.PropertyType.GetListTypeNonNull().IsDefined(typeof(DnUsarStringParaEnumeradoresNoBdAtributo)))
        //                {
        //                    entity.Property(property.Name).HasConversion<string>();// Converte os enumeradores para salvar o valor string no BD
        //                }
        //            }
        //        }

        //        protected virtual void SetEntityProperty(EntityTypeBuilder entity, Type type, PropertyInfo property) { }

        //        protected virtual void SetEntity(EntityTypeBuilder entity, Type type) { }

        //        protected static LoggerFactory ContextLogFactory = null;

        //        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //        {
        //#if DEBUG

        //            ContextLogFactory ??= new LoggerFactory(new[] { new DebugLoggerProvider() });
        //            optionsBuilder
        //                .UseLoggerFactory(ContextLogFactory)
        //                    .EnableSensitiveDataLogging();

        //#endif
        //        }

        //        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        //        {
        //            UpdateLogicalDeletion(ChangeTracker.Entries());

        //            var eventChange = BeforeSave();

        //            var ret = await base.SaveChangesAsync();

        //            AfterSave(eventChange);

        //            return ret;
        //        }

        //        protected virtual void UpdateLogicalDeletion(IEnumerable<EntityEntry> entries)
        //        {
        //        }

        //        protected virtual LambdaExpression IsAvailable()
        //        {
        //            throw new DesenvolvimentoIncorretoException($"The Enable {nameof(UseLogicalDeletion)} method set to 'true' requires the override of the {nameof(IsAvailable)} method in the context of the entity framework. Do not invoke the base.");
        //        }

        //        protected virtual bool UseLogicalDeletion(Type type, EntityTypeBuilder entity, ModelBuilder modelBuilder) => false;

        //        public override bool EnableLogicalDeletion { get; set; }

        //        internal protected SessaoDeRequisicaoDoUsuario UserSessionRequest { get; internal set; }

        //        private void AfterSave(List<DnDadosDeEntidadeAlterada> eventChangeList)
        //        {
        //            if (EntityChangedEventEvent == null) { return; }
        //            eventChangeList.ForEach(x => SetEventChangeCurrentValue(x));
        //            EntityChangedEventEvent.Invoke(eventChangeList);
        //        }

        //        private List<DnDadosDeEntidadeAlterada> BeforeSave()
        //        {
        //            var changedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Deleted || e.State == EntityState.Modified).ToList();
        //            var eventChangeList = changedEntities.Select(GetEventChange).Where(x => x != null).ToList();
        //            EntityChangingEventEvent?.Invoke(eventChangeList);
        //            return eventChangeList;
        //        }

        //        private void SetEventChangeCurrentValue(DnDadosDeEntidadeAlterada DnEventEntity)
        //        {
        //            var currentValuesGetValue = DnEventEntity.EntradaDeEntidade.CurrentValues.GetType().GetMethod("GetValue", new[] { typeof(IProperty) });
        //            var properties = DnEventEntity.EntradaDeEntidade.CurrentValues.Properties.ToList();

        //            DnEventEntity.Propriedades.ForEach(x =>
        //            {
        //                var property = properties.Next();
        //                x.NovoValor = currentValuesGetValue?.MakeGenericMethod(property.ClrType).Invoke(DnEventEntity.EntradaDeEntidade.CurrentValues, new[] { property });
        //            });
        //        }

        //        private DnDadosDeEntidadeAlterada GetEventChange(EntityEntry entityChanged)
        //        {
        //            var currentValuesGetValue = entityChanged.CurrentValues.GetType().GetMethod("GetValue", new[] { typeof(IProperty) });
        //            var originalValuesGetValue = entityChanged.OriginalValues.GetType().GetMethod("GetValue", new[] { typeof(IProperty) });

        //            var properties = entityChanged.OriginalValues.Properties.Select(x =>
        //            {
        //                return new DnDadosDePropriedadeAlterada
        //                {
        //                    NovoValor = currentValuesGetValue?.MakeGenericMethod(x.ClrType).Invoke(entityChanged.CurrentValues, new[] { x }),
        //                    ValorOriginal = originalValuesGetValue?.MakeGenericMethod(x.ClrType).Invoke(entityChanged.OriginalValues, new[] { x }),
        //                    NomeDaPropriedade = x.Name
        //                };
        //            }).ToList();

        //            var currentEntityType = entityChanged.Entity.GetType();

        //            if (currentEntityType.GetCustomAttribute<DnLogAtributo>()?.Apresentar == EnumApresentar.Ocultar)
        //            {
        //                return null;
        //            }

        //            return new DnDadosDeEntidadeAlterada
        //            {
        //                Propriedades = properties,
        //                EntidadeAtual = entityChanged.Entity,
        //                TipoDaEntidadeAtual = currentEntityType,
        //                EntradaDeEntidade = entityChanged,
        //                EstadoDaEntidade = entityChanged.State
        //            };
        //        }
    }
}