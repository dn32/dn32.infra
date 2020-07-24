using System;









namespace dn32.infra
{
    public static class ServiceExtension
    {
        public static DnServicoTransacional GetServiceInstanceByServiceType(this Type serviceType, SessaoDeRequisicaoDoUsuario SessionRequest)
        {
            if (serviceType == null) { throw new ArgumentNullException(nameof(serviceType)); }
            if (SessionRequest == null) { throw new ArgumentNullException(nameof(SessionRequest)); }
            if (serviceType.Name == "DnDynamicProxy") { serviceType = serviceType.BaseType; }
            if (serviceType == null) { throw new ArgumentNullException(nameof(serviceType)); }

            if (!serviceType.IsSubclassOf(typeof(DnServicoTransacional)))
            {
                throw new DesenvolvimentoIncorretoException($"The service instance attempt using the {nameof(GetServiceInstanceByServiceType)} method failed because the passed type is not a {nameof(DnServicoTransacional)}");
            }

            return SessionRequest.Servicos.GetOrAdd(serviceType, FabricaDeServico.Criar(serviceType, SessionRequest.HttpContextLocal, SessionRequest)) as DnServicoTransacional;
        }

        public static DnServicoTransacional GetServiceInstanceByEntity(this Type entityType, SessaoDeRequisicaoDoUsuario SessionRequest)
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