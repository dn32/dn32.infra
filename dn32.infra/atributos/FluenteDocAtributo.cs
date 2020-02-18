using System;
using dn32.infra.enumeradores;

namespace dn32.infra.atributos
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Property)]
    public class FluenteDocAtributo : Attribute
    {
        public EnumMostrar Apresentacao { get; }

        public FluenteDocAtributo(EnumMostrar apresentar = EnumMostrar.Mostrar)
        {
            Apresentacao = apresentar;
        }
    }
}
