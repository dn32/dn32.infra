using System;
using dn32.infra.enumeradores;

namespace dn32.infra.atributos
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Property)]
    public class DnDocAtributo : Attribute
    {
        public EnumApresentar Apresentacao { get; }

        public DnDocAtributo(EnumApresentar apresentar = EnumApresentar.Mostrar)
        {
            Apresentacao = apresentar;
        }
    }
}
