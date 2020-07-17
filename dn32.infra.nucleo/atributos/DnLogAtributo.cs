using System;


namespace dn32.infra
{
    public class DnLogAtributo : Attribute
    {
        public EnumApresentar Apresentar { get; set; }

        public DnLogAtributo(EnumApresentar apresentar = EnumApresentar.Mostrar)
        {
            this.Apresentar = apresentar;
        }
    }
}