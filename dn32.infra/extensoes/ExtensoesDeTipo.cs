using System;

namespace dn32.infra.extensoes
{
    public static class ExtensoesDeTipo
    {
        public static T FluenteCast<T>(this object obj, bool dispararExcecaoCasoAConversaoFalhe = true)
        {
            if (obj == null) return default;

            if (obj is T valor)
                return valor;
            
            if (dispararExcecaoCasoAConversaoFalhe)
                throw new InvalidOperationException($"O tipo '{obj.GetType().Name}' não é um '{typeof(T).Name}'.");

            return default;
        }
    }
}