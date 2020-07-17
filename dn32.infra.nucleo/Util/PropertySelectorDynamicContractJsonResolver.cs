using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace dn32.infra
{
    public class PropertySelectorDynamicContractJsonResolver : DefaultContractResolver
    {
        private readonly string[] PropertyToIgnore;
        private readonly string[] PropriedadesDesejadas;

        public PropertySelectorDynamicContractJsonResolver(string[] propertyToIgnore, string[] propriedadesDesejadas)
        {
            PropertyToIgnore = propertyToIgnore;
            PropriedadesDesejadas = propriedadesDesejadas;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);
            IList<JsonProperty> propertiesResult = null;

            if (PropertyToIgnore != null && PropertyToIgnore.Length > 0)
            {
                propertiesResult = properties.Where(x => !PropertyToIgnore.Contains(x.PropertyName, StringComparer.InvariantCultureIgnoreCase)).ToList();
            }

            if (PropriedadesDesejadas != null && PropriedadesDesejadas.Length > 0)
            {
                propertiesResult = properties.Where(x => PropriedadesDesejadas.Contains(x.PropertyName, StringComparer.InvariantCultureIgnoreCase)).ToList();
            }

            if (propertiesResult == null || propertiesResult.Count == 0)
            {
                return properties;
            }
            else
            {
                return propertiesResult;
            }
        }
    }
}