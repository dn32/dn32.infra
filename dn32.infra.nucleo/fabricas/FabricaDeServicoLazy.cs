using System;



namespace dn32.infra
{
    internal class FabricaDeServicoLazy
    {
        internal static DnServicoTransacionalBase Criar(Type serviceType, Guid sessionId) =>
            ServiceLazyClassBuilder.CreateObject(serviceType, sessionId) as DnServicoTransacionalBase;
    }
}