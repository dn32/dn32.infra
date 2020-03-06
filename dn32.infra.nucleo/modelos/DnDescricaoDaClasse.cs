using dn32.infra.Nucleo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace dn32.infra.nucleo.modelos
{
    internal class DnDescricaoDaClasse
    {
        public List<DnDescricaoDePropriedade> Propriedades { get; set; }

        public DnDescricaoDaClasse(Type tipoPrincipal, string[] campos)
        {
            Propriedades = new List<DnDescricaoDePropriedade>();

            var composicoes = campos.OrderBy(x => x).Where(x => x.Contains(".")).ToList();
            var outrasPropriedades = campos.OrderBy(x => x).Where(x => !x.Contains(".")).ToList();

            foreach (var propriedade in outrasPropriedades)
            {
                var propriedadeComplexa = ObterPropriedadesSimples(tipoPrincipal, propriedade);
                Propriedades.Add(propriedadeComplexa);
            }

            foreach (var property in composicoes)
            {
                AdicionarPropriedadeDeComposicao(Propriedades, tipoPrincipal, property);
            }
        }

        private static void AdicionarPropriedadeDeComposicao(List<DnDescricaoDePropriedade> propriedades, Type tipoPrincipal, string propriedade)
        {
            var indice = propriedade.IndexOf(".");
            var nomeDaClasse = propriedade.Substring(0, indice);
            var nomeDaPropriedade = propriedade.Substring(indice + 1);
            var propertyInfo = tipoPrincipal.GetProperty(nomeDaClasse) ?? throw new InvalidOperationException($"A entidade '{tipoPrincipal.Name}' não possui uma propriedade com nome '{nomeDaClasse}'");
            var propriedadeComplexaEncontrada = propriedades.SingleOrDefault(x => x.Nome == nomeDaClasse);

            if (propriedadeComplexaEncontrada == null)
            {
                var propriedadeComplexa = new DnDescricaoDePropriedade { Nome = nomeDaClasse, Tipo = propertyInfo.PropertyType, DescricaoDaClasse = new DnDescricaoDaClasse(propertyInfo.PropertyType, new[] { nomeDaPropriedade }) };
                propriedades.Add(propriedadeComplexa);
            }
            else
            {
                if (nomeDaPropriedade.Contains("."))
                {
                    AdicionarPropriedadeDeComposicao(propriedadeComplexaEncontrada.DescricaoDaClasse.Propriedades, propertyInfo.PropertyType, nomeDaPropriedade);
                }
                else
                {
                    var propriedadeSimples = ObterPropriedadesSimples(propertyInfo.PropertyType, nomeDaPropriedade);
                    propriedadeComplexaEncontrada.DescricaoDaClasse.Propriedades.Add(propriedadeSimples);
                }
            }
        }

        private static DnDescricaoDePropriedade ObterPropriedadesSimples(Type tipoPrincipal, string nomeDaPropriedade)
        {
            var propertyInfo = tipoPrincipal.GetProperties().FirstOrDefault(x => x.Name.Equals(nomeDaPropriedade, StringComparison.InvariantCultureIgnoreCase)) ?? throw new InvalidOperationException($"A entidade '{tipoPrincipal.Name}' não possui uma propriedade com nome '{nomeDaPropriedade}'");
            return new DnDescricaoDePropriedade { Nome = nomeDaPropriedade, Tipo = propertyInfo.PropertyType };
        }
    }
}
