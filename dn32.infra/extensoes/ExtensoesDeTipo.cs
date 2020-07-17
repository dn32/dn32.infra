using System;

namespace dn32.infra {
    public static class ExtensoesDeTipo {
        public static T DnCast<T> (this object obj, bool dispararExcecaoCasoAConversaoFalhe = true) {
            if (obj == null) return default;

            if (obj is T valor)
                return valor;

            if (dispararExcecaoCasoAConversaoFalhe)
                throw new InvalidOperationException ($"O tipo '{obj.GetType().Name}' não é um '{typeof(T).Name}'.");

            return default;
        }
    }
}