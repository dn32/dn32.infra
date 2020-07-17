using System;

namespace dn32.infra {
    [AttributeUsage (AttributeTargets.Enum)]
    public class DnValorNuloParaEnumeradorAtributo : Attribute {
        public int Valor { get; }

        public DnValorNuloParaEnumeradorAtributo (int valor) {
            this.Valor = valor;
        }
    }
}