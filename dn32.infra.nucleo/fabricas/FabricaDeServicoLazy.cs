using System;



namespace dn32.infra
{
    internal class FabricaDeServicoLazy
    {
        internal static DnServicoTransacional Criar(Type serviceType, Guid sessionId) =>
            ServiceLazyClassBuilder.CreateObject(serviceType, sessionId) as DnServicoTransacional;
    }
}