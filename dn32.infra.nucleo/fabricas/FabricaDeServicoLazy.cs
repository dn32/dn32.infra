using dn32.infra.Factory.Proxy;
using dn32.infra.servicos;
using System;

namespace dn32.infra.nucleo.fabricas
{
    internal class FabricaDeServicoLazy
    {
        internal static DnServicoTransacionalBase Criar(Type serviceType, Guid sessionId) =>
            ServiceLazyClassBuilder.CreateObject(serviceType, sessionId) as DnServicoTransacionalBase;
    }
}