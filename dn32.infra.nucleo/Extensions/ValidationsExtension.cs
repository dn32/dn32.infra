﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;








namespace dn32.infra
{
    internal static class ValidationsExtension
    {
        /*
         === PADRÃO DE NOMECLATURA ===
         O que deve ser verdadeiro
         ParameterMustBeInformed 
         (O parâmetro deve ser informado. Se não for informado, teremos uma inconsistência)
         Evite escrever negação, mas quando não for possível evitar, escreva assim: EntityShouldNotExistInDatabase.
         A entida não pode existir. Se existir, teremos uma inconsistência.
         =============================         
        */

        internal static void DnValidateAttribute<T>(this IDnValidacao validation, T entity, string compositionProperty, string compositionFieldName) where T : DnEntidadeBase
        {
            if (!validation.ChecagemDeParametroNuloOk)
            {
                return;
            }

            var properties = entity.GetType().GetProperties();
            foreach (var property in properties)
            {
                var DnValidateAttribute = property.GetCustomAttribute<DnValidacaoAttribute>(true)?.DnCast<DnValidacaoAttribute>();
                if (DnValidateAttribute == null) { continue; }

                DnValidateAttribute.Entidade = entity;
                var value = property.GetValue(entity);
                if (!DnValidateAttribute.EhValidoQuando(value))
                {
                    validation.AdicionarInconsistencia(new DnGenericoErroDeValidacao(property, false, DnValidateAttribute.MensagemDeInvalido, compositionProperty, compositionFieldName));
                }
            }
        }

        internal static void ParameterMustBeInformed(this IDnValidacao validation, object obj, string compositionProperty)
        {
            if (obj == null)
            {
                validation.AdicionarInconsistencia(new DnParametroNuloErroDeValidacao(compositionProperty ?? nameof(obj)));
                validation.ChecagemDeParametroNuloOk = false;
                return;
            }

            validation.ChecagemDeParametroNuloOk = true;
        }

        internal static void MaxMinLenghtPropertyMustBeInformed<T>(this IDnValidacao validation, T entity, string compositionProperty, string compositionFieldName) where T : DnEntidadeBase
        {
            if (!validation.ChecagemDeParametroNuloOk)
            {
                return;
            }

            var properties = entity.GetType().GetProperties().ToList();
            foreach (var property in properties)
            {
                if (property.IsDefined(typeof(DnCriarValorRandomicoAoAdicionarEntidadeAttribute), true)) { continue; }
                if (!string.IsNullOrWhiteSpace(compositionProperty))
                {
                    if (property.GetCustomAttribute<DnPropriedadeJsonAttribute>(true)?.Formulario == EnumTipoDeComponenteDeFormularioDeTela.Hidden)
                    {
                        continue;
                    }
                }
                var value = property.GetValue(entity);
                if (value == null) { continue; }
                if (property.PropertyType.EhNumerico())
                {
                    var min = property.GetCustomAttribute<DnPropriedadeJsonAttribute>()?.Minimo ?? property.GetCustomAttribute<RangeAttribute>()?.Minimum;
                    var max = property.GetCustomAttribute<DnPropriedadeJsonAttribute>()?.Maximo ?? property.GetCustomAttribute<RangeAttribute>()?.Maximum;
                    var mindouble = min == null ? double.MinValue : double.Parse(min?.ToString() ?? "", CultureInfo.InvariantCulture);
                    var maxdouble = max == null ? double.MaxValue : double.Parse(max?.ToString() ?? "", CultureInfo.InvariantCulture);
                    var stringValue = value?.ToString() ?? "";
                    if (stringValue.Contains(".")) { stringValue = stringValue.Split(".")[0]; }
                    if (stringValue.Contains(",")) { stringValue = stringValue.Split(",")[0]; }
                    var valuedoble = double.Parse(stringValue, CultureInfo.InvariantCulture);
                    if (valuedoble < mindouble || valuedoble > maxdouble)
                    {
                        validation.AdicionarInconsistencia(new DnCampoDeTelaComTamanhoIncorretoErroDeValidacao(property, compositionProperty, compositionFieldName));
                    }
                }

                if (property.PropertyType == typeof(string) && property.PropertyType == typeof(String))
                {
                    var requ = property.GetCustomAttribute<RequiredAttribute>() != null || property.GetCustomAttribute<DnRequeridoAttribute>() != null;
                    if (requ && property.GetValue(entity).IsDnNull())
                    { //Nesse caso já há uma inconsistência de requerido adicionada
                        return;
                    }

                    var min = property.GetCustomAttribute<DnPropriedadeJsonAttribute>()?.Minimo ?? property.GetCustomAttribute<MinLengthAttribute>()?.Length;
                    var max = property.GetCustomAttribute<DnPropriedadeJsonAttribute>()?.Maximo ?? property.GetCustomAttribute<MaxLengthAttribute>()?.Length;
                    if (min == null || max == null) { continue; }

                    if (!new MinLengthAttribute(min.Value).IsValid(value))
                    {
                        validation.AdicionarInconsistencia(new DnCampoDeTelaComTamanhoIncorretoErroDeValidacao(property, compositionProperty, compositionFieldName));
                    }

                    var maxint = Convert.ChangeType(max, typeof(int), CultureInfo.InvariantCulture) as int?;
                    maxint ??= 0;
                    maxint = maxint == 0 ? int.MaxValue : maxint;
                    if (!new MaxLengthAttribute(maxint.Value).IsValid(value))
                    {
                        validation.AdicionarInconsistencia(new DnCampoDeTelaComTamanhoIncorretoErroDeValidacao(property, compositionProperty, compositionFieldName));
                    }
                }
            }
        }

        internal static void RequiredPropertyMustBeInformed<T>(this IDnValidacao validation, T entity, string compositionProperty, string compositionFieldName) where T : DnEntidadeBase
        {
            if (!validation.ChecagemDeParametroNuloOk)
            {
                return;
            }

            var properties = typeof(T).GetPropertiesByAttribute<RequiredAttribute>();
            var properties2 = typeof(T).GetPropertiesByAttribute<DnRequeridoAttribute>();

            properties2.ForEach(x =>
            {
                if (!properties.Contains(x))
                {
                    properties.Add(x);
                }
            });

            foreach (var property in properties)
            {
                if (property.IsDefined(typeof(DnCriarValorRandomicoAoAdicionarEntidadeAttribute), true)) { continue; }
                if (!string.IsNullOrWhiteSpace(compositionProperty))
                {
                    if (property.GetCustomAttribute<DnPropriedadeJsonAttribute>(true)?.Formulario == EnumTipoDeComponenteDeFormularioDeTela.Hidden)
                    {
                        continue;
                    }
                }

                if (property.GetValue(entity).IsDnNull())
                {
                    validation.AdicionarInconsistencia(new DnCampoDeTelaRequeridoErroDeValidacao(property, compositionProperty, compositionFieldName));
                }
            }
        }

        internal static void AllKeysMustBeInformed<T>(this IDnValidacao validation, T entity, string compositionProperty, string compositionFieldName) where T : DnEntidadeBase
        {
            validation.ChecagemDeChavesOk = true;

            var properties = entity.GetType().GetKeyProperties();
            foreach (var property in properties)
            {
                if (property.IsDefined(typeof(DnCriarValorRandomicoAoAdicionarEntidadeAttribute), true)) { continue; }
                if (!string.IsNullOrWhiteSpace(compositionProperty))
                {
                    if (property.GetCustomAttribute<DnPropriedadeJsonAttribute>(true)?.Formulario == EnumTipoDeComponenteDeFormularioDeTela.Hidden)
                    {
                        continue;
                    }
                }

                if (property.GetValue(entity).IsDnNull())
                {
                    validation.AdicionarInconsistencia(new DnCampoDeTelaRequeridoErroDeValidacao(property, compositionProperty, compositionFieldName));
                    validation.ChecagemDeChavesOk = false;
                }
            }
        }

        internal static void AllKeysShouldBeInformedWhenThereAreMoreThanOne<T>(this IDnValidacao validation, T entity, string compositionProperty, string compositionFieldName, bool isUpdate = false) where T : DnEntidadeBase
        {
            if (!validation.ChecagemDeParametroNuloOk || !validation.ChecagemDeChavesOk)
            {
                return;
            }

            var entityType = typeof(T);
            var properties = entityType.GetKeyProperties();
            if (properties.Count > 1)
            {
                validation.AllKeysMustBeInformed(entity, compositionProperty, compositionFieldName);
            }
            else
            {
                if (entityType.GetCustomAttribute<DnEntidadeSemChaveAttribute>() != null) return;

                var property = properties.FirstOrDefault();
                if (property == null) throw new DesenvolvimentoIncorretoException($"A entidade {entityType.Name} deve possuir uma chave primária decorata com o atributo [Key]");
                if (property.GetValue(entity).IsDnNull())
                {
                    if (!isUpdate)
                    {
                        return;
                    }

                    if (property.IsDefined(typeof(DnCriarValorRandomicoAoAdicionarEntidadeAttribute), true)) { return; }
                    if (!string.IsNullOrWhiteSpace(compositionProperty))
                    {
                        if (property.GetCustomAttribute<DnPropriedadeJsonAttribute>(true)?.Formulario == EnumTipoDeComponenteDeFormularioDeTela.Hidden)
                        {
                            return;
                        }
                    }

                    validation.AdicionarInconsistencia(new DnCampoDeTelaRequeridoErroDeValidacao(property, compositionProperty, compositionFieldName));
                    validation.ChecagemDeChavesOk = false;
                }
                else
                {
                    if (isUpdate)
                    {
                        return;
                    }

                    //Todo - Exigir que não seja informado somente quando o campo for de auto incremento.
                    //AddInconsistency(new DbFieldNotRequiredDnValidationException(property));
                    //KeyValuesOk = false;
                }
            }
        }

        internal static async Task EntityMustExistInDatabaseAsync<T>(this IDnValidacao validation, T entity, bool incluirExcluidosLogicamente = false) where T : DnEntidadeBase
        {
            if (!validation.ChecagemDeParametroNuloOk)
            {
                return;
            }

            if (!await validation.DnCast<DnValidacao<T>>().Servico.ExisteAsync(entity, incluirExcluidosLogicamente))
            {
                var keys = entity.GetKeyValues().Select(x => $"{{{x.Propriedade.Name}:{x.Valor}}}").ToArray();
                var keyValues = string.Join(", ", keys);
                validation.AdicionarInconsistencia(new DnEntidadeNaoEncontradaErroDeValidacao(keyValues));
            }
        }

        internal static async Task ThereIsOnlyOneEntityAsync<T>(this IDnValidacao validation, T entity, bool incluirExcluidosLogicamente = false) where T : DnEntidadeBase
        {
            if (!validation.ChecagemDeParametroNuloOk)
            {
                return;
            }

            if (await validation.DnCast<DnValidacao<T>>().Servico.QuantidadeAsync(entity, incluirExcluidosLogicamente) > 1)
            {
                var keys = entity.GetKeyValues().Select(x => $"-{{{x.Propriedade.Name}:{x.Valor}}}").ToArray();
                var keyValues = string.Join(", ", keys);
                validation.AdicionarInconsistencia(new DnEntidadeExisteErroDeValidacao(keyValues));
            }
        }

        internal static async Task EntityShouldNotExistInDatabaseBasedOnKeysAsync<T>(this IDnValidacao validation, T entity) where T : DnEntidadeBase
        {
            if (!validation.ChecagemDeParametroNuloOk || !validation.ChecagemDeChavesOk)
            {
                return;
            }

            if (await validation.DnCast<DnValidacao<T>>().Servico.ExisteAsync(entity))
            {
                var keys = entity.GetKeyAndDnUniqueKeyValues().Select(x => $"{{{x.Propriedade.Name}:{x.Valor}}}").ToArray();
                var keyValues = string.Join(", ", keys);
                validation.AdicionarInconsistencia(new DnEntidadeExisteErroDeValidacao(keyValues));
            }
        }
    }
}