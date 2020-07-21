using System;

namespace dn32.infra
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class DnValorNuloParaEnumeradorAttribute : Attribute
    {
        public int Valor { get; }

        public DnValorNuloParaEnumeradorAttribute(int valor)
        {
            this.Valor = valor;
        }
    }
}