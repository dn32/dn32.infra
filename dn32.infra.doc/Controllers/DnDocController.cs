using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web;







using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dn32.infra
{

#if (!DEBUG)
    [ResponseCache (Duration = 60000, Location = ResponseCacheLocation.Client)]
#endif
    [AllowAnonymous]
    public partial class DnDocController : Controller
    {
        #region PROPERTIES

        private static Dictionary<string, Type> Models { get; set; }
        private static Dictionary<string, Type> AllTypes { get; set; }
        private static List<EntityModelAndName> AllEntities { get; set; }
        private static List<EntityModelAndName> AllModel { get; set; }
        private static object InitialLock { get; set; } = new object();

        #endregion

        public DnDocController()
        {
            Initialize();
        }

        [Route("dndoc"), Route("dndoc/Index")]
        public IActionResult Index()
        {
            return View(AllEntities);
        }

        [Route("dndoc/acao")]
        public IActionResult Acao(string servico, string nomeDaAcao)
        {
            if (Models.TryGetValue<string, Type>(servico, StringComparison.InvariantCultureIgnoreCase, out Type type))
            {
                if (Setup.Controladores.TryGetValue(type, out Type controllerType))
                {
                    if (controllerType.GetCustomAttribute<DnDocAttribute>()?.Apresentacao == EnumApresentar.Ocultar)
                    {
                        throw new InvalidOperationException("DnDocAttributeAttribute is EnumDnMostrar.Hidden");
                    }

                    var model = type.GetDnJsonSchema(false);
                    if (model != null)
                    {
                        var routeAtributeController = controllerType.GetCustomAttributes<RouteAttribute>(true).FirstOrDefault();
                        var actionMethod = controllerType
                            .GetMethods()
                            .Where(method => method.IsPublic && !method.IsDefined(typeof(NonActionAttribute)))
                            .Where(method => !method.Name.StartsWith("get_") && !method.Name.Equals("Dispose") && !method.Name.Equals("GetType") && !method.Name.StartsWith("set_"))
                            .Where(method => method.GetCustomAttribute<DnDocAttribute>()?.Apresentacao != EnumApresentar.Ocultar)
                            .FirstOrDefault(method => method.Name.Equals(nomeDaAcao, StringComparison.InvariantCultureIgnoreCase));

                        if (actionMethod == null) { return Content("Action not found"); }

                        var actionData = GetActionData(actionMethod, type, controllerType, routeAtributeController).First();

                        return View(actionData);
                    }
                    else
                    {
                        return Content("JsonSchema not found");
                    }
                }
                else
                {
                    return Content("Controller not found");
                }
            }
            else
            {
                throw new InvalidOperationException($"Servico {servico} not found");
            }
        }

        [Route("dndoc/Modelo")]
        public IActionResult Modelo(string nome)
        {
            if (AllTypes.TryGetValue(nome, out Type type))
            {
                var jsonSchema = type.GetDnJsonSchema(false);
                jsonSchema.Formulario.Nome = type.GetFriendlyName();
                jsonSchema.Desabilitado = type.GetCustomAttribute<DnDesabilitadoAttribute>(true)?.Motivo ?? "";
                jsonSchema.Propriedades.Where(x => x.Propriedade.GetCustomAttribute<DnDocAttribute>()?.Apresentacao != EnumApresentar.Ocultar).ToList()
                    .ForEach(x =>
                    {
                        x.Descricao = x.Descricao?.G() ?? x.Propriedade.GetCustomAttribute<DnPropriedadeJsonAtributo>(true)?.Descricao;
                        x.Link = GetModelLink(x.Tipo);
                    });

                if (type.IsNullableEnum())
                {
                    jsonSchema.Propriedades = type.GetFields().Where(x => x.Name != "value__").Select(x =>
                     new DnPropriedadeJsonAtributo
                     {
                         NomeDaPropriedade = x.Name,
                         Nome = (x.GetCustomAttribute<DnPropriedadeJsonAtributo>(true)?.Descricao ?? x.Name),
                         Descricao = (x.GetCustomAttribute<DescriptionAttribute>(true)?.Description ?? x.GetCustomAttribute<DnPropriedadeJsonAtributo>(true)?.Descricao ?? x.Name).G(),
                         Formulario = EnumTipoDeComponenteDeFormularioDeTela.Texto,
                         Tipo = x.FieldType.BaseType,
                         Desabilitado = x.GetCustomAttribute<DnDesabilitadoAttribute>(true)?.Motivo ?? "",
                         Valor = (int)(x.GetValue(0) ?? 0),
                         EhEnumerador = true
                     }).ToList();
                }

                return View(jsonSchema);
            }

            return Content("Model not found");
        }

        [Route("dndoc/Entidade")]
        public IActionResult Entidade()
        {
            return View(AllEntities);
        }

        [Route("dndoc/modeloNaoEntidade")]
        public IActionResult ModeloNaoEntidade()
        {
            return View(AllModel);
        }

        [Route("dndoc/Servico")]
        public IActionResult Servico(string nome)
        {
            if (Models.TryGetValue<string, Type>(nome, StringComparison.InvariantCultureIgnoreCase, out Type type))
            {
                if (Setup.Controladores.TryGetValue(type, out Type controllerType))
                {
                    if (controllerType.GetCustomAttribute<DnDocAttribute>()?.Apresentacao == EnumApresentar.Ocultar)
                    {
                        throw new InvalidOperationException("DnDocAttributeAttribute is EnumDnMostrar.Hidden");
                    }

                    var model = type.GetDnJsonSchema(false);
                    if (model != null)
                    {
                        var routeAtributeController = controllerType.GetCustomAttributes<RouteAttribute>(true).FirstOrDefault();
                        var actions = controllerType
                            .GetMethods()
                            .Where(method => method.IsPublic && !method.IsDefined(typeof(NonActionAttribute)))
                            .Where(method => !method.Name.StartsWith("get_") && !method.Name.Equals("Dispose") && !method.Name.Equals("GetType") && !method.Name.StartsWith("set_"))
                            .Where(method => method.GetCustomAttribute<DnDocAttribute>()?.Apresentacao != EnumApresentar.Ocultar)
                            .SelectMany(action =>
                            {
                                return GetActionData(action, type, controllerType, routeAtributeController);
                            })
                            .ToList();

                        actions = actions.OrderBy(x => x.Name).OrderBy(x => x.OrderMethod).ToList();
                        ViewBag.actions = actions;
                        return View(model);
                    }
                    else
                    {
                        return Content("JsonSchema not found");
                    }
                }
                else
                {
                    return Content("Controller not found");
                }
            }
            else
            {
                throw new InvalidOperationException($"Servico {nome} not found");
            }
        }

        #region PRIVATE

        private string GetReturn(MethodInfo methodInfo)
        {
            var returnType = methodInfo.ReturnType.GetTaskType();
            return returnType.GetFriendlyName(false, true);
        }

        private EnumParameterSouce GetParameterSource(ParameterInfo parameterInfo, int orderMethod)
        {
            if (parameterInfo.IsDefined(typeof(FromQueryAttribute))) { return EnumParameterSouce.Query; }
            if (parameterInfo.IsDefined(typeof(FromBodyAttribute))) { return EnumParameterSouce.Body; }
            if (parameterInfo.IsDefined(typeof(FromFormAttribute))) { return EnumParameterSouce.Form; }
            if (parameterInfo.IsDefined(typeof(FromRouteAttribute))) { return EnumParameterSouce.Route; }
            if (parameterInfo.IsDefined(typeof(FromHeaderAttribute))) { return EnumParameterSouce.Header; }
            if (parameterInfo.IsDefined(typeof(FromServicesAttribute))) { return EnumParameterSouce.Service; }
            if (orderMethod == 2 || orderMethod == 3) { return EnumParameterSouce.Body; } else return EnumParameterSouce.Query;
        }

        internal static string GetModelLink(Type type)
        {
            var fullName = type.GetListTypeNonNull().FullName;
            if (string.IsNullOrWhiteSpace(fullName)) { return string.Empty; }
            if (AllTypes.TryGetValue(type.GetListTypeNonNull().FullName, out _)) { return $"/DnDoc/modelo?Nome={type.GetListTypeNonNull().FullName}"; }
            return string.Empty;
        }

        private DnActionSchema[] GetActionData(MethodInfo action, Type type, Type controllerType, RouteAttribute routeAtributeController)
        {
            var mets = action.GetCustomAttributes<HttpMethodAttribute>() ?? new List<HttpGetAttribute> { new HttpGetAttribute() };
            return mets.Select(x => Onter(x)).ToArray();

            DnActionSchema Onter(HttpMethodAttribute met)
            {
                var routeAtributeAction = action.GetCustomAttribute<RouteAttribute>(false);
                var routerAttribute = routeAtributeAction?.Template ?? routeAtributeController?.Template;
                var template = routerAttribute ?? met.Template ?? action.Name;

                var route = template.Replace("[controller]", controllerType.Name.Remove("Controller"), StringComparison.InvariantCultureIgnoreCase);
                route = route.Replace("[action]", action.Name, StringComparison.InvariantCultureIgnoreCase);
                var name = route.Split("/").Last();
                var method = met.HttpMethods.FirstOrDefault().Replace("DELETE", "DEL");
                var methodName = action.Name;

                var returnType = GetReturn(action);

                var orderMethod = method
                switch
                {
                    "GET" => 1,
                    "POST" => 2,
                    "PUT" => 3,
                    "DEL" => 4,
                    _ => 5,
                };

                var parameters = action.GetParameters().Select(x =>
                  new DocParameter
                  {
                      Link = GetModelLink(x.ParameterType),
                      Type = x.ParameterType,
                      Name = x.Name,
                      Description = (x.GetCustomAttribute<DescriptionAttribute>(true)?.Description ?? x.GetCustomAttribute<DnPropriedadeJsonAtributo>(true)?.Descricao ?? x.Name).G(),
                      Source = GetParameterSource(x, orderMethod),
                      Example = x.ParameterType.GetExampleValueString()
                  }).ToList();

                var description = action.GetCustomAttribute<DescriptionAttribute>()?.Description;
                var DnAction = action.GetCustomAttribute<DnActionAtributo>();

                if (DnAction?.Paginacao == true)
                {
                    parameters.AddRange(new[] {
                        new DocParameter (Parametros.NomePaginaAtual, typeof (string), EnumParameterSouce.Header, "A página atual", "1"),
                            new DocParameter (Parametros.NomeItensPorPagina, typeof (string), EnumParameterSouce.Header, "O número de páginas", "10"),
                            new DocParameter (Parametros.NomeIniciarNaPaginaZero, typeof (bool), EnumParameterSouce.Header, "Se a primeira página é 0.", "true")
                    });
                }

                if (DnAction?.EspecificacaoDinamica == true)
                {
                    parameters.AddRange(new[] {
                        //new DocParameter(Parametros.NomePropertyToIgnore, typeof(string), EnumParameterSouce.Header, "The properties you want to ignore in the query", "Code,Adress.Code"),
                        new DocParameter (Parametros.NomePropriedadesDesejadas, typeof (string), EnumParameterSouce.Header, "As propriedades úteis para sua listagem", "Nome,Sobrenome,Endereco.Cidade"),
                            new DocParameter (Parametros.NomePropriedadesDeOrdenacao, typeof (string), EnumParameterSouce.Header, "As propriedades a usar como ordenacao", "Nome,Sobrenome".G ())
                    });
                }

                var anonimo = controllerType.GetCustomAttribute<AllowAnonymousAttribute>(true) != null || action.GetCustomAttribute<AllowAnonymousAttribute>(true) != null;
                if (Setup.ConfiguracoesGlobais.InformacoesDoJWT != null && !anonimo)
                {
                    parameters.Add(new DocParameter("Authorization", typeof(string), EnumParameterSouce.Header, "O token de autenticação", "Bearer xxxxx"));
                }

                var action_ = new DnActionSchema
                {
                    ControllerType = controllerType,
                    EntityType = type,
                    Action = action,
                    Name = name,
                    Route = route,
                    Method = method,
                    OrderMethod = orderMethod,
                    Parameters = parameters,
                    Description = description.G(),
                    ApiBaseUrl = DnDocExtension.ApiBaseUrl,
                    ReturnType = returnType,
                    MethodName = methodName,
                    Desabilitado = action.GetCustomAttribute<DnDesabilitadoAttribute>(true)?.Motivo ?? ""
                };

                action_.Example = GetExampleAction(action_);

                return action_;
            }
        }

        private List<string> JsonToQueryString(string json)
        {
            var jObj = (JObject)JsonConvert.DeserializeObject(json);
            if (jObj == null) { return default; }
            return jObj.Children().Cast<JProperty>().Select(jp => jp.Name + "=" + HttpUtility.UrlEncode(jp.Value.ToString())).ToList();
        }

        private string GetExampleAction(DnActionSchema action)
        {
            var parametersArray = action.Parameters.Where(x => x.Source == EnumParameterSouce.Header).Select(x => $"xhr.setRequestHeader(\"{x.Name}\", \"{x.Example}\");").ToArray();
            var parametersQueryArray = action.Parameters.Where(x => x.Source == EnumParameterSouce.Query).Where(x => !x.Type.IsDnEntity()).Select(x => $"{x.Name}={x.Example}").ToList();
            var parametersQueryArray3 = action.Parameters.Where(x => x.Source == EnumParameterSouce.Query).Where(x => x.Type.IsDnEntity()).SelectMany(x => JsonToQueryString(x.Example)).ToList();
            parametersQueryArray.AddRange(parametersQueryArray3);

            var parametersQueryArrayString = "";

            if (parametersQueryArray.Count > 0)
            {
                parametersQueryArrayString = "?" + string.Join("&", parametersQueryArray);
            }

            var parametersString = string.Join('\n', parametersArray);
            var dataExample = "xhr.send();";

            if (action.Method == "POST" || action.Method == "PUT")
            {
                dataExample =
                    $@"var data = JSON.stringify({{
    ""test"": ""a""
}});

xhr.send(data);";
            }

            var example =
                $@"var xhr = new XMLHttpRequest();
xhr.withCredentials = true;
xhr.open(""{action.Method}"", ""{action.ApiBaseUrl}{action.Route}{parametersQueryArrayString}"");
{parametersString}

xhr.addEventListener(""readystatechange"", function() {{
    if (this.readyState === 4)
    {{
        console.log(this.responseText);
    }}
}});

{dataExample}
";
            return example;
        }

        private void Initialize()
        {
            lock (InitialLock)
            {
                if (Models == null)
                {
                    Models = new Dictionary<string, Type>();
                    var entities = Setup.ObterEntidades();
                    entities.ForEach(type =>
                    {

                        if (Setup.Controladores.TryGetValue(type, out Type controllerType))
                        {
                            if (controllerType.GetCustomAttribute<DnDocAttribute>()?.Apresentacao == EnumApresentar.Ocultar)
                            {
                                return;
                            }
                        }

                        Models.TryAdd(type.Name, type);
                    });
                }

                if (AllTypes == null)
                {
                    AllTypes = Setup.TodosOsTipos
                        .GroupBy(x => x.FullName)
                        .Select(x => x.First())
                        .Where(x => x.GetCustomAttribute<DnDocAttribute>()?.Apresentacao != EnumApresentar.Ocultar)
                        .Where(x => x.GetCustomAttribute<DnControladorApiAtributo>()?.GerarAutomaticamente != false)
                        .OrderBy(x => x.Name)
                        .ToDictionary(x => x.FullName, x => x);
                }

                AllEntities = Models.Values
                    .Where(x => x.GetCustomAttribute<DnDocAttribute>()?.Apresentacao != EnumApresentar.Ocultar)
                    .Where(x => x.GetCustomAttribute<DnControladorApiAtributo>()?.GerarAutomaticamente != false)
                    .Select(x => new EntityModelAndName
                    {
                        Description = (x.GetCustomAttribute<DescriptionAttribute>(true)?.Description ?? x.GetCustomAttribute<DnFormularioJsonAtributo>(true)?.Descricao ?? x.Name).G(),
                        FriendlyName = x.GetCustomAttribute<DnFormularioJsonAtributo>(true)?.Nome ?? x.GetFriendlyName(),
                        Name = x.Name, //.ToDnJsonStringNormalized(),
                        FullName = x.FullName,
                        Grupo = x.GetCustomAttribute<DnFormularioJsonAtributo>(true)?.Grupo ?? "",
                    })
                    .OrderBy(x => x.Name)
                    .ToList();

                AllModel = AllTypes.Values
                    .Where(x => x != null)
                    .Where(x => !Setup.Modelos.ContainsKey(x))
                    .Where(x => !Setup.Servicos.ContainsKey(x))
                    .Where(x => !Setup.Repositorios.ContainsKey(x))
                    .Where(x => !Setup.Controladores.ContainsKey(x))
                    .Where(x => !Setup.Validacoes.ContainsKey(x))
                    .Where(x => !x.Is(typeof(Controller)))
                    .Where(x => !x.Is(typeof(ControllerBase)))
                    .Where(x => x.FullName?.Contains("+") == false)
                    .Where(x => x.FullName?.StartsWith("System") == false)
                    .Where(x => x.FullName?.StartsWith("Windows") == false)
                    .Where(x => x.FullName?.StartsWith("Microsoft") == false)
                    .Where(x => x.FullName?.StartsWith("Internal") == false)
                    .Where(x => x.FullName?.StartsWith("FxResources") == false)
                    .Where(x => x.GetCustomAttribute<DnDocAttribute>()?.Apresentacao == EnumApresentar.Mostrar)
                    .Select(x => new EntityModelAndName
                    {
                        Description = (x.GetCustomAttribute<DescriptionAttribute>(true)?.Description ?? x.GetCustomAttribute<DnFormularioJsonAtributo>(true)?.Descricao ?? x.GetFriendlyName()).G(),
                        FriendlyName = x.GetCustomAttribute<DnFormularioJsonAtributo>(true)?.Nome ?? x.GetFriendlyName(),
                        Name = x.Name, //.ToDnJsonStringNormalized(),
                        FullName = x.FullName
                    })
                    .OrderBy(x => x.Name)
                    .ToList();
            }
        }

        #endregion
    }
}