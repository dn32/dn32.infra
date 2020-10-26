
namespace dn32.infra
{
    public interface IDnEspecificacaoBase
    {
        DnServicoTransacional Servico { get; set; }
        internal DnEspecificacao<TE> ObterSpec<TE>() where TE : DnEntidadeBase
        {
            if (this is DnEspecificacao<TE> spec) return spec;
            throw new DesenvolvimentoIncorretoException("A especificação é diferente do esperado");
        }

        internal DnEspecificacaoAlternativa<TE, TO> ObterSpecAlternativo<TE, TO>() where TE : DnEntidadeBase
        {
            if (this is IDnEspecificacaoAlternativaGenerica<TO> spec)
            {
                if (spec.TipoDeEntidade != typeof(TE))
                {
                    var serviceName = $"{spec.TipoDeEntidade.Name}Servico";
                    throw new DesenvolvimentoIncorretoException($"The type of input reported in the {spec} specification is not the same as that requested in the repository request.\r\nSpecification type: {spec.TipoDeEntidade}.\r\nRequisition Tipo: {typeof(TE)}\r\nThis usually occurs when you make use of the wrong service. Make sure that when invoking the method that is causing this error you are making use of the service: {serviceName}");
                }

                if (spec.TipoDeRetorno != typeof(TO))
                {
                    var serviceName = $"{typeof(TE).Name}Servico";
                    throw new DesenvolvimentoIncorretoException($"The type of output reported in the {spec} specification is not the same as that requested in the repository request.\r\nSpecification type: {spec.TipoDeEntidade}.\r\nRequisition Tipo: {typeof(TO)}\r\nThis usually occurs when you make use of the wrong service. Make sure that when invoking the method that is causing this error you are making use of the service: {serviceName}");
                }

                return spec as DnEspecificacaoAlternativa<TE, TO>;
            }

            throw new DesenvolvimentoIncorretoException("The specification is of a different type than expected");
        }
    }
}