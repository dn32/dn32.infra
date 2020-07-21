using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace dn32.infra
{
    public class FabricaDeController : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var baseController = (Setup.ConfiguracoesGlobais.TipoGenericoDeController) ?? typeof(DnApiController<>);
            var entities = Setup.ObterEntidades();

            foreach (var entity in entities)
            {
                if (entity.GetCustomAttribute<DnControllerApiAttribute>(true)?.GerarAutomaticamente == false) { continue; }
                if (Setup.Controllers.ContainsKey(entity)) { continue; }

                if (entity.GetCustomAttribute<DnFormularioJsonAttribute>(true)?.EhSomenteLeitura == true)
                {
                    baseController = typeof(DnApiSomenteLeituraController<>);
                }

                var typeName = entity.Name + "Controller";
                if (feature.Controllers.Any(t => t.Name == typeName))
                {
                    throw new DesenvolvimentoIncorretoException($"TO controlador '{typeName}' Possui um nome que internamente é reservado. Por favor, altere o nome desse controlador.");
                }

                var parentClass = baseController.MakeGenericType(entity);
                var moduleName = $"DnDynamicModule{entity.Name}";

                var dynamicClass = BuilderClassUtil.CreateClass(parentClass, typeName, moduleName);
                BuilderClassUtil.CreateConstructor(dynamicClass);
                var type = dynamicClass.CreateType() ??
                    throw new InvalidOperationException($"ControllerFactory not create {typeName}");

                var controllerType = type.GetTypeInfo();
                feature.Controllers.Add(controllerType);
                Setup.Controllers.Add(entity, controllerType);
            }
        }
    }
}