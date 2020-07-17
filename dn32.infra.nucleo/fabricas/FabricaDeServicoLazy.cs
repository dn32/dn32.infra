using System;
using dn32.infra;
using dn32.infra;

namespace dn32.infra {
    internal class FabricaDeServicoLazy {
        internal static DnServicoTransacionalBase Criar (Type serviceType, Guid sessionId) =>
            ServiceLazyClassBuilder.CreateObject (serviceType, sessionId) as DnServicoTransacionalBase;
    }
}