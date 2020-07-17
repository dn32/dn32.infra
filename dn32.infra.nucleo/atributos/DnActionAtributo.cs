using System;

namespace dn32.infra
{
    public class DnActionAtributo : Attribute
    {
        public bool Paginacao { get; set; }
        public bool EspecificacaoDinamica { get; set; }
    }
}