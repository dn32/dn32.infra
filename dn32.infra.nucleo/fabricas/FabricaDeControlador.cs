using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace dn32.infra {
    public class FabricaDeControlador : IApplicationFeatureProvider<ControllerFeature> {
        public void PopulateFeature (IEnumerable<ApplicationPart> parts, ControllerFeature feature) {
            var baseController = (Setup.ConfiguracoesGlobais.TipoGenericoDeControlador) ?? typeof (DnApiControlador<>);
            var entities = Setup.ObterEntidades ();

            foreach (var entity in entities) {
                if (entity.GetCustomAttribute<DnControladorApiAtributo> (true)?.GerarAutomaticamente == false) { continue; }
                if (Setup.Controladores.ContainsKey (entity)) { continue; }

                if (entity.GetCustomAttribute<DnFormularioJsonAtributo> (true)?.EhSomenteLeitura == true) {
                    baseController = typeof (DnApiSomenteLeituraControlador<>);
                }

                var typeName = entity.Name + "Controller";
                if (feature.Controllers.Any (t => t.Name == typeName)) {
                    throw new DesenvolvimentoIncorretoException ($"TO controlador '{typeName}' Possui um nome que internamente é reservado. Por favor, altere o nome desse controlador.");
                }

                var parentClass = baseController.MakeGenericType (entity);
                var moduleName = $"DnDynamicModule{entity.Name}";

                var dynamicClass = BuilderClassUtil.CreateClass (parentClass, typeName, moduleName);
                BuilderClassUtil.CreateConstructor (dynamicClass);
                var type = dynamicClass.CreateType () ??
                    throw new InvalidOperationException ($"ControllerFactory not create {typeName}");

                var controllerType = type.GetTypeInfo ();
                feature.Controllers.Add (controllerType);
                Setup.Controladores.Add (entity, controllerType);
            }
        }
    }
}