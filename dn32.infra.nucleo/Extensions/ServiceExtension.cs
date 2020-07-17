using System;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using dn32.infra;

namespace dn32.infra
{
    public static class ServiceExtension
    {
        public static DnServicoTransacionalBase GetServiceInstanceByServiceType(this Type serviceType, SessaoDeRequisicaoDoUsuario SessionRequest)
        {
            if (serviceType == null) { throw new ArgumentNullException(nameof(serviceType)); }
            if (SessionRequest == null) { throw new ArgumentNullException(nameof(SessionRequest)); }
            if (serviceType.Name == "DnDynamicProxy") { serviceType = serviceType.BaseType; }
            if (serviceType == null) { throw new ArgumentNullException(nameof(serviceType)); }

            if (!serviceType.IsSubclassOf(typeof(DnServicoTransacionalBase)))
            {
                throw new DesenvolvimentoIncorretoException($"The service instance attempt using the {nameof(GetServiceInstanceByServiceType)} method failed because the passed type is not a {nameof(DnServicoTransacionalBase)}");
            }

            return SessionRequest.Servicos.GetOrAdd(serviceType, FabricaDeServico.Criar(serviceType, SessionRequest.LocalHttpContext, SessionRequest)) as DnServicoTransacionalBase;
        }

        public static DnServicoTransacionalBase GetServiceInstanceByEntity(this Type entityType, SessaoDeRequisicaoDoUsuario SessionRequest)
        {
            if (entityType?.IsSubclassOf(typeof(DnEntidade)) != true)
            {
                throw new DesenvolvimentoIncorretoException($"The service instance attempt using the {nameof(GetServiceInstanceByEntity)} method failed because the passed type is not a {nameof(DnEntidade)}");
            }

            var type = (Setup.ConfiguracoesGlobais.TipoGenericoDeServico) ?? typeof(DnServico<>);
            var serviceType = type.MakeGenericType(entityType).GetSpecializedService();
            return serviceType.GetServiceInstanceByServiceType(SessionRequest);
        }
    }
}