using System;


namespace dn32.infra
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Property)]
    public class DnDocAttribute : Attribute
    {
        public EnumApresentar Apresentacao { get; }

        public DnDocAttribute(EnumApresentar apresentar = EnumApresentar.Mostrar)
        {
            Apresentacao = apresentar;
        }
    }
}