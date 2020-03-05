﻿using dn32.infra.nucleo.atributos;
using dn32.infra.Nucleo.Util;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using dn32.infra.extensoes;

namespace dn32.infra.Nucleo.Extensoes
{
    public static class PropertyInfoExtension
    {
        public static object GetExampleValue(this PropertyInfo property, int max_ = 0)
        {
            if (property.PropertyType.GetNonNullableType().EhNumerico())
            {
                var min = property.GetCustomAttribute<DnPropriedadeJsonAtributo>()?.Minimo ?? property.GetCustomAttribute<RangeAttribute>()?.Minimum;
                var max = property.GetCustomAttribute<DnPropriedadeJsonAtributo>()?.Maximo ?? property.GetCustomAttribute<RangeAttribute>()?.Maximum;
                if (min == null || max == null) { return UtilitarioDeRandomico.NextRandom(int.MaxValue); }

                if (max.ToString() == "0" && property.PropertyType.EhNumerico())
                {
                    max = property.PropertyType.GetMaxValueOfNumber().DnCast<double>();
                }

                return UtilitarioDeRandomico.NextRandom((int)min, (double)max);
            }

            if (property.PropertyType.GetNonNullableType() == typeof(string) && property.PropertyType.GetNonNullableType() == typeof(String))
            {
                var min = property.GetCustomAttribute<DnPropriedadeJsonAtributo>()?.Minimo ?? property.GetCustomAttribute<MinLengthAttribute>()?.Length;
                var max = max_ == 0 ? (property.GetCustomAttribute<DnPropriedadeJsonAtributo>()?.Maximo ?? property.GetCustomAttribute<MaxLengthAttribute>()?.Length) : max_;
                max ??= 64;
                min ??= 0;
                if (min == null) { return UtilitarioDeRandomico.ObterString(max.Value); }
                if (max == null) { return UtilitarioDeRandomico.ObterString(min.Value); }

                return UtilitarioDeRandomico.ObterString(max.Value);
            }

            return property.PropertyType.GetDnDefaultValue();
        }
    }
}
