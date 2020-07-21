using System;


namespace dn32.infra
{
    public class DnLogAttribute : Attribute
    {
        public EnumApresentar Apresentar { get; set; }

        public DnLogAttribute(EnumApresentar apresentar = EnumApresentar.Mostrar)
        {
            this.Apresentar = apresentar;
        }
    }
}