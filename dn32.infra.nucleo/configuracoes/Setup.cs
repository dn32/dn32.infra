using dn32.infra.nucleo.atributos;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(@"dn32.infra.MongoDB, PublicKey=00240000048000009400000006020000002400005253413100040000010001001d60f9371826336d02a18cdc7b000ff4a0f0e691f4f55d1092a811e133392b3e2774473fa950df245818f02ddcd3bf19416633ee192bd88d7ed7944902c4b161da3b6ee7bb1aa2a48039bd716bda21b4ddc8c60c9c17a3e2d17be1fb61e0bd9d19f3713b126c03553ffdea885e09d845bc91def13ca9fed634c3c0cea1cd24e0")]
[assembly: InternalsVisibleTo(@"dn32.infra.RavenDB, PublicKey=00240000048000009400000006020000002400005253413100040000010001004937541a190ae55d1f8699f95f30229168f1197dba44da4ca1d93ddf769ae93c94667a1bca78b2ae5817773d33b88eb5fb98b8b153ca7b637f0d2023c50dd125b1f13150ae7b57b99127a0b51267b82a5987f18f67f2916571939b75f8953046a4c110efe84ed22170536460d6d9b1f52bbfd382c6570c0878f2054656a2a19d")]
[assembly: InternalsVisibleTo(@"dn32.infra.EntityFramework, PublicKey=00240000048000009400000006020000002400005253413100040000010001000da51e0f449f6ee7879b256b497e9f64eda760b5fac3d47a4ba8a54664303024f451098b69154691fad078fe77ee79ac2b6a9770fd7a6555a4c49a2a58e82f411939e1eb44ac4a1327acdd13f2c8ec7698644d019f04197838434be8cb53877f1d22acab90ae7735acc363fdb393a11fa34afe780d1c5fb26f37a8fd6e4d9b9f")]
[assembly: InternalsVisibleTo(@"dn32.infra.Doc, PublicKey=00240000048000009400000006020000002400005253413100040000010001008963bf4072062c4090dd8b8b1b3335b78ac84c4e55c7903a918af1d62ecf0e2ab5504ca1fa722b67f5968cdbbf2f1436cc9303018d57511caefbae6cf903f681d721a1122bcdc4f35fa4aafade1e9900468a69aba391d3e9c2eb3087bd37727bbcc30f704666c62beccdca492d8e5467088b696c39306fa582637041a8c40dc4")]
namespace dn32.infra
{
    public static class Setup
    {
        #region PROPRIEDADES

        public static Dictionary<Type, Type> Models { get; private set; }

        public static Dictionary<Type, Type> Controllers { get; private set; }

        public static bool Inicializado { get; set; }

        public static DnConfiguracoesGlobais ConfiguracoesGlobais { get; set; }

        internal static Dictionary<Type, Type> Servicos { get; set; }

        internal static Dictionary<Type, Type> Repositorios { get; set; }

        internal static Dictionary<Type, Type> Validacoes { get; set; }

        internal static ConcurrentDictionary<Guid, SessaoDeRequisicaoDoUsuario> SessoesDeRequisicoesDeUsuarios { get; set; }

        internal static List<Type> TodosOsTipos { get; set; }

        private static object TravaDeInicializacao { get; set; } = new object();

        private static IServiceCollection ServiceCollection { get; set; }

        #endregion

        #region MÉTODOS PÚBLICOS

        #region DEFINIÇÕES

        public static DnConfiguracoesGlobais DefinirTipoGenericoDeServico<S>(this DnConfiguracoesGlobais configuracoes) where S : DnServicoBase
        {
            if (configuracoes != null)
                configuracoes.TipoGenericoDeServico = typeof(S).GetGenericTypeDefinition();

            return configuracoes;
        }

        public static DnConfiguracoesGlobais DefinirTipoGenericoDeRepositorio<R>(this DnConfiguracoesGlobais configuracoes) where R : IDnRepositorioTransacional
        {
            if (configuracoes != null)
                configuracoes.TipoGenericoDeRepositorio = typeof(R).GetGenericTypeDefinition();

            return configuracoes;
        }

        public static DnConfiguracoesGlobais DefinirTipoGenericoDeValidacao<V>(this DnConfiguracoesGlobais configuracoes) where V : DnValidacaoTransacional
        {
            if (configuracoes != null)
                configuracoes.TipoGenericoDeValidacao = typeof(V).GetGenericTypeDefinition();

            return configuracoes;
        }

        public static DnConfiguracoesGlobais DefinirTipoGenericoDeController<C>(this DnConfiguracoesGlobais configuracoes) where C : DnControllerBase
        {
            if (configuracoes != null)
                configuracoes.TipoGenericoDeController = typeof(C).GetGenericTypeDefinition();

            return configuracoes;
        }

        public static DnConfiguracoesGlobais NaoMostrarLogsDoBDEmDebug(this DnConfiguracoesGlobais configuracoes)
        {
            if (configuracoes != null)
                configuracoes.MostrarLogsDoBDEmDebug = false;

            return configuracoes;
        }

        public static DnConfiguracoesGlobais DefinirTipoGenericoDeSessaoDeRequisicao<T>(this DnConfiguracoesGlobais configuracoes) where T : SessaoDeRequisicaoDoUsuario
        {
            if (configuracoes != null)
                configuracoes.TipoDeSessaoDeRequisicaoDeUsuario = typeof(T);

            return configuracoes;
        }

        internal static DnConfiguracoesGlobais DefinirFabricaDeRepositorio(this DnConfiguracoesGlobais configuracoes, DnFrabricaDeRepositorioBase fabricaDeRepositorio)
        {
            if (configuracoes != null)
                configuracoes.FabricaDeRepositorio = fabricaDeRepositorio;

            return configuracoes;
        }

        #endregion

        public static List<Type> ObterEntidades() => Models.Values.Where(x =>
          !x.IsAbstract &&
          x.IsPublic &&
          !x.IsDefined(typeof(NotMappedAttribute), false) &&
          x.IsSubclassOf(typeof(DnEntidade))
        ).ToList();

        public static DnConfiguracoesGlobais UsarJWT<S>(this DnConfiguracoesGlobais configuracoes, InformacoesDoJWT informacoesDoJWT) where S : DnServicoDeAutenticacao
        {
            configuracoes.InformacoesDoJWT = informacoesDoJWT;
            configuracoes.InformacoesDoJWT.DnAuthenticationServiceType = typeof(S);
            return configuracoes;
        }

        public static DnConfiguracoesGlobais DefinirComoAmbienteDeTeste(this DnConfiguracoesGlobais configuracoes)
        {
            configuracoes.EhAmbienteDeTeste = true;
            return configuracoes;
        }

        public static DnConfiguracoesGlobais AdicionarStringDeConexao<T>(
            this DnConfiguracoesGlobais configuracoes,
            string stringDeConexao,
            bool criarOBancoDeDadosCasoNaoExista = true,
            string identificadorDaConexao = "",
            bool conexaoDeTeste = false
            ) where T : IDnDbContext
        {
            return configuracoes.AdicionarStringDeConexao(_ => stringDeConexao, typeof(T), criarOBancoDeDadosCasoNaoExista, identificadorDaConexao, conexaoDeTeste);
        }

        public static DnConfiguracoesGlobais AdicionarStringDeConexao(
                this DnConfiguracoesGlobais configuracoes,
                string stringDeConexao,
                Type tipoDoContexto,
                bool criarOBancoDeDadosCasoNaoExista = true,
                string identificadorDaConexao = "",
                bool conexaoDeTeste = false
                ) =>
            configuracoes.AdicionarStringDeConexao(_ => stringDeConexao, tipoDoContexto, criarOBancoDeDadosCasoNaoExista, identificadorDaConexao, conexaoDeTeste);

        public static DnConfiguracoesGlobais AdicionarStringDeConexao(
            this DnConfiguracoesGlobais configuracoes,
            Func<SessaoDeRequisicaoDoUsuario, string> obterStringDeConexao,
            Type tipoDoContexto,
            bool criarOBancoDeDadosCasoNaoExista = true,
            string identificadorDaConexao = "",
            bool conexaoDeTeste = false
            )
        {
            if (configuracoes == null)
            {
                return configuracoes;
            }

            configuracoes.AdicionarConexao(
                new Conexao
                {
                    ObterStringDeConexao = obterStringDeConexao,
                    TipoDoContexto = tipoDoContexto,
                    IdentificadorDaConexao = identificadorDaConexao,
                    CriarOBancoDeDadosCasoNaoExista = criarOBancoDeDadosCasoNaoExista,
                    ConexaoDeTeste = conexaoDeTeste
                });

            return configuracoes;
        }

        //Todo no boot da aplicação, checar se os tipos de contexto possuem o atributo do tipo de BD
        //Todo - checar ainda se não tem identificador igual
        public static IServiceCollection Compilar(this DnConfiguracoesGlobais configuracoes)
        {
            ConfiguracoesGlobais = configuracoes;
            return ServiceCollection;
        }

        public static DnConfiguracoesGlobais AdicionarDnArquitetura(this IMvcBuilder builder, JsonSerializerSettings jsonSerializerSettings = null)
        {
            if (jsonSerializerSettings is null)
                jsonSerializerSettings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver(), NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore };
            //throw new ArgumentNullException (nameof (jsonSerializerSettings));

            ExtensoesJson.ConfiguracoesDeSerializacao = jsonSerializerSettings;
            ServiceCollection = builder.Services;
            InicializacaoInterna();
            builder.ConfigureApplicationPartManager(apm => apm.FeatureProviders.Add(new FabricaDeController()));

            ConfiguracoesGlobais = ConfiguracoesGlobais == null ? new DnConfiguracoesGlobais() : ConfiguracoesGlobais;
            return ConfiguracoesGlobais;
        }

        internal static void InicializacaoInterna()
        {
            lock (TravaDeInicializacao)
            {
                if (Inicializado) { return; }
                InicializarObjetos();
                CarregarAssemblies();
                ExecutarValidacoes();
            }
        }

        private static void InicializarObjetos()
        {
            Inicializado = true;
            Servicos = new Dictionary<Type, Type>();
            Repositorios = new Dictionary<Type, Type>();
            Validacoes = new Dictionary<Type, Type>();
            Models = new Dictionary<Type, Type>();
            Controllers = new Dictionary<Type, Type>();
            SessoesDeRequisicoesDeUsuarios = new ConcurrentDictionary<Guid, SessaoDeRequisicaoDoUsuario>();
            Servicos.Add(typeof(DnEntidade), typeof(DnServico<DnEntidade>));
            Repositorios.Add(typeof(DnEntidade), typeof(DnRepositorio<DnEntidade>));
            Validacoes.Add(typeof(DnEntidade), typeof(DnValidacao<DnEntidade>));
            Controllers.Add(typeof(DnEntidade), typeof(DnController<DnEntidade>));
        }

        private static void CarregarAssemblies()
        {
            TodosOsTipos = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic)
                .OrderBy((Func<Assembly, string>)(x => x.FullName))
                .SelectMany(x => GetExportedTypes(x))
                .ToList();
        }

        private static IEnumerable<Type> GetExportedTypes(Assembly x)
        {
            try
            {
                return x.ExportedTypes;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                return new List<Type>();
            }
        }

        private static void ExecutarValidacoes()
        {
            var tipos = TodosOsTipos;
            var servicos = tipos.Where(x => x.IsSubclassOf(typeof(DnServicoTransacional))).ToList();

            ValidateIfAllServicePropertiesNotHaveTheSetMethod(servicos);
            ValidateIfAllServicePropertiesAreVirtual(servicos);
            ValidateIfAllServicePropertiesNotHavePublic(servicos);
            ValidateIfAllServicePropertiesHaveDefaultConstructor(servicos);

            ValidarEspecificacoes(tipos.Where(x => x.IsSubclassOf(typeof(DnEspecificacaoBase))).ToList());
            ValidarControllers(tipos.Where(x => x.IsSubclassOf(typeof(DnControllerBase))).ToList());

            tipos.Select(x => GlobalUtil.GetDnEntityType(x, typeof(DnServico<DnEntidadeBase>)))
                .Where(x => x.Item1 != null).ToList()
                .ForEach(AddService);

            tipos.Where(x => x.Is(typeof(IDnRepositorioTransacional)))
                .Select(x => GlobalUtil.GetDnEntityType(x, typeof(DnRepositorio<DnEntidadeBase>)))
                .Where(x => x.Item1 != null).ToList()
                .ForEach(AddRepository);

            tipos.Select(x => GlobalUtil.GetDnEntityType(x, typeof(DnValidacao<DnEntidadeBase>)))
                .Where(x => x.Item1 != null).ToList()
                .ForEach(AddValidation);

            tipos.Select(x => GlobalUtil.GetDnEntityType(x, typeof(DnEntidadeBase)))
                .Where(x => x.Item1 != null && x.Item2 != typeof(DnEntidadeBase)).ToList()
                .ForEach(AddModel);

            tipos.Select(x => GlobalUtil.GetDnEntityType(x, typeof(DnController<DnEntidadeBase>)))
                .Where(x => x.Item1 != null).ToList()
                .ForEach(AddController);

            // Todo - Não me recordo o motivo de estar comentado, mas acredito que tenha que descomentar
            // ValidateIfAllMethodsAreVirtual(Services.Valores.ToList()); // To intercept
            // ValidateIfAllMethodsAreVirtual(Repositories.Valores.ToList()); // To intercept
            // ValidateIfAllMethodsAreVirtual(Validations.Valores.ToList()); //It is not necessary
            CheckErrorInTheRepository(Repositorios.Values.ToList());

            // DbSetup(createDatabaseIfNotExists);
        }

        private static void ValidarEspecificacoes(List<Type> especificacoes)
        {
            especificacoes.ForEach(type =>
            {
                if (type.GetConstructors().Any(x => x.GetParameters().Any()))
                    throw new DesenvolvimentoIncorretoException($"A especificação '{type}' possui parâmetros no construtor e isso não é permitido. Crie um método para passar os parâmetros.");
            });
        }

        private static void ValidarControllers(List<Type> controladores)
        {
            controladores.ForEach(type =>
            {
                if (type.GetMethods().Any(x => x.IsPublic && x.GetParameters().Any(y => y.ParameterType.IsSubclassOf(typeof(DnEspecificacaoBase)))))
                    throw new DesenvolvimentoIncorretoException($"O controlador '{type}' possui um ou mais métosos público(s) que recebe(m) uma especificacao como parâmetro. Isso não é permitido.");
            });
        }

        #endregion

        #region MÉTODOS INTERNOS

        internal static SessaoDeRequisicaoDoUsuario ObterSessaoDeUmaRequisicao(Guid identificadorDaSessao)
        {
            if (!SessoesDeRequisicoesDeUsuarios.TryGetValue(identificadorDaSessao, out var sessao))
                throw new Exception($"Não foi encontrada uma sessão de requisição com o identificadorDaSessao: '{identificadorDaSessao}'");

            return sessao;
        }

        internal static void AdicionarSessaoDeRequisicao(SessaoDeRequisicaoDoUsuario sessaoDeRequisicaoDoUsuario) =>
            SessoesDeRequisicoesDeUsuarios.TryAdd(sessaoDeRequisicaoDoUsuario.IdentificadorDaSessao, sessaoDeRequisicaoDoUsuario);

        internal static void RemoverSessaoDeRequisicao(Guid sessionId) => SessoesDeRequisicoesDeUsuarios.TryRemove(sessionId, out _);

        #endregion

        #region PRIVATE
        //Todo - Traduzir
        private static void ValidateIfAllServicePropertiesHaveDefaultConstructor(IEnumerable<Type> tipos)
        {
            foreach (var type in tipos)
            {
                if (type.IsAbstract)
                {
                    continue;
                }

                var generic = type.GetGenericArguments();
                if (generic.Length > 0 && generic.First().Name == "T")
                {
                    continue;
                }

                var defaultConstructor = type.GetConstructors().Any(x => !x.GetParameters().Any());
                if (!defaultConstructor)
                {
                    throw new DesenvolvimentoIncorretoException($"Every repository must have an empty constructor. {type}");
                }
            }
        }

        private static void ValidateIfAllServicePropertiesNotHaveTheSetMethod(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                if (type == null) { continue; }
                var serviceProperties = type.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(x => x.GetMethod?.IsPrivate == false && x.GetMethod?.IsVirtual == true && x.PropertyType.IsSubclassOf(typeof(DnServicoBase))).ToList();

                if (serviceProperties == null)
                {
                    continue;
                }

                foreach (var prop in serviceProperties)
                {
                    if (prop.GetCustomAttribute<DnIgnorarInjecaoAttribute>() != null) continue;
                    if (prop.SetMethod != null)
                    {
                        throw new DesenvolvimentoIncorretoException($"The property {type}.{prop.Name} has a set method. Servico properties are not allowed to have the set method.");
                    }
                }
            }
        }

        private static void ValidateIfAllServicePropertiesAreVirtual(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var serviceProperties = type?.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(x => x.GetMethod?.IsVirtual == false && x.PropertyType.IsSubclassOf(typeof(DnServicoBase)))
                    .Where(x => x.GetCustomAttribute<DnIgnorarInjecaoAttribute>() == null)
                    .ToList();

                if (serviceProperties != null && serviceProperties.Any())
                {
                    throw new DesenvolvimentoIncorretoException($"All service properties must be protected virtual. {type}.{serviceProperties.First().Name}");
                }
            }
        }

        private static void ValidateIfAllServicePropertiesNotHavePublic(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var serviceProperties = type?.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(x => x.GetMethod?.IsPublic == true && x.PropertyType.IsSubclassOf(typeof(DnServicoBase))).ToList();

                if (serviceProperties != null && serviceProperties.Any())
                {
                    throw new DesenvolvimentoIncorretoException($"All repository properties must be protected virtual.{type}.{serviceProperties.First().Name}");
                }
            }
        }

        private static void AddModel(Tuple<Type, Type> service)
        {
            if (Models.ContainsKey(service.Item2))
            {
                throw new DesenvolvimentoIncorretoException($"There are two entity classes with the same Nome {service.Item2.Name}. This is not allowed.");
            }

            Models.Add(service.Item2, service.Item2);
        }

        private static void AddService(Tuple<Type, Type> service)
        {
            if (Servicos.ContainsKey(service.Item1))
            {
                throw new DesenvolvimentoIncorretoException($"There are two service classes with the same Nome {service.Item1} -  {service.Item2}. This is not allowed.");
            }

            Servicos.Add(service.Item1, service.Item2);
        }

        private static void AddValidation(Tuple<Type, Type> validation)
        {
            if (Validacoes.ContainsKey(validation.Item1))
            {
                throw new DesenvolvimentoIncorretoException($"There are two validation classes with the same Nome {validation.Item1} - {validation.Item2}. This is not allowed.");
            }

            Validacoes.Add(validation.Item1, validation.Item2);
        }

        private static void AddController(Tuple<Type, Type> controller)
        {
            if (Controllers.ContainsKey(controller.Item1))
            {
                throw new DesenvolvimentoIncorretoException($"There are two controller classes with the same Nome {controller.Item1} - {controller.Item2}. This is not allowed.");
            }

            Controllers.Add(controller.Item1, controller.Item2);
        }

        private static void AddRepository(Tuple<Type, Type> repository)
        {
            if (Repositorios.ContainsKey(repository.Item1))
            {
                throw new DesenvolvimentoIncorretoException($"There are two entity repository with the same Nome {repository.Item1} - {repository.Item2}. This is not allowed.");
            }

            Repositorios.Add(repository.Item1, repository.Item2);
        }

        private static void CheckErrorInTheRepository(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                foreach (var method in methods)
                {
                    var name = $"{type.Name}.{method.Name}";
                    if (method.IsPublic && method.ReturnType.Name == typeof(IEnumerable<DnEntidade>).Name || method.ReturnType.Name == typeof(IQueryable<DnEntidade>).Name)
                    {
                        throw new DesenvolvimentoIncorretoException($"The use of non-materialized returns in repositories is not allowed. Change the return type and execute the ToList before the return in the {name}.");
                    }

                    var parameters = method.GetParameters().Select(x => x.ParameterType).ToList();
                    foreach (var parameter in parameters)
                    {
                        if (parameter.Name.StartsWith("Func", StringComparison.CurrentCultureIgnoreCase) && method.Name != "RawSqlQuery")
                        {
                            throw new DesenvolvimentoIncorretoException($"You should not use Func as the input parameter of the repository methods, since Func requires the materialization of the entire list of entities. {name}");
                        }
                    }
                }
            }
        }

        #endregion
    }
}