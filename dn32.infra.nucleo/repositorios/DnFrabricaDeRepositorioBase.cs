using System;
using System.Linq;
using System.Reflection;

namespace dn32.infra
{
    internal abstract class DnFrabricaDeRepositorioBase
    {
        protected abstract Type ObterTipoDeRepositorioPadrao<T>() where T : DnEntidadeBase;

        public DnRepositorio<T> Create<T>(IDnObjetosTransacionais transactionObjects, DnServico<T> service) where T : DnEntidadeBase
        {
            if (Setup.ConfiguracoesGlobais.Conexoes == null) { throw new DesenvolvimentoIncorretoException($"A arquitetura não foi iniciada"); }

            var DnTipoDeBancoDeDadosAttribute = GetTheEntityTipoDeBancoDeDadosAttribute(typeof(T));
            if (DnTipoDeBancoDeDadosAttribute == null)
            {
                if (Setup.ConfiguracoesGlobais.Conexoes.Count == 1)
                {
                    DnTipoDeBancoDeDadosAttribute = Setup.ConfiguracoesGlobais.Conexoes.Single().TipoDoContexto.GetCustomAttribute<DnTipoDeBancoDeDadosAttribute>() ??
                        throw new DesenvolvimentoIncorretoException($"The entity {typeof(T).Name} needs a database type specification. Example: [DnTipoDeBancoDeDadosAttribute (DnTipoDeBancoDeDadosAttribute.ORACLE)]"); ;
                }
            }

            if (DnTipoDeBancoDeDadosAttribute == null)
            {
                throw new DesenvolvimentoIncorretoException($"The entity {typeof(T).Name} needs a database type specification. Example: [DnTipoDeBancoDeDadosAttribute (DnTipoDeBancoDeDadosAttribute.ORACLE)]"); ;
            }

            var localType = Setup.ConfiguracoesGlobais.TipoGenericoDeRepositorio?.MakeGenericType(typeof(T)) ?? ObterTipoDeRepositorioPadrao<T>(); // typeof(DnEFRepository<T>);

            if (Setup.Repositorios.TryGetValue(typeof(T), out var repositoryType))
            {
                localType = repositoryType;
            }

            var repository = Create<T>(localType);

            if (transactionObjects == null)
            {
                Conexao connetion;

                if (string.IsNullOrWhiteSpace(DnTipoDeBancoDeDadosAttribute.Identifier))
                {
                    var conn = Setup.ConfiguracoesGlobais.Conexoes.Where(x => x.TipoDoContexto.GetCustomAttribute<DnTipoDeBancoDeDadosAttribute>()?.TipoDeBancoDeDados == DnTipoDeBancoDeDadosAttribute.TipoDeBancoDeDados);
                    if (conn.Count() > 1)
                    {
                        throw new DesenvolvimentoIncorretoException($"More than one connection of the same type was found with the same type \"{DnTipoDeBancoDeDadosAttribute.TipoDeBancoDeDados}\". Adicionar identifiers for them.");
                    }

                    if (conn.Count() == 0)
                    {
                        throw new DesenvolvimentoIncorretoException($"Could not find connection of requested \"{DnTipoDeBancoDeDadosAttribute.TipoDeBancoDeDados}\" type in entity \"{typeof(T).Name}\"");
                    }
                    connetion = conn.Single();
                }
                else
                {
                    if (Setup.ConfiguracoesGlobais.Conexoes == null) { throw new DesenvolvimentoIncorretoException($"Arquitetura was not initialized properly"); }
                    var conn = Setup.ConfiguracoesGlobais.Conexoes.Where(x =>
                       x.TipoDoContexto.GetCustomAttribute<DnTipoDeBancoDeDadosAttribute>()?.TipoDeBancoDeDados == DnTipoDeBancoDeDadosAttribute.TipoDeBancoDeDados &&
                       x.IdentificadorDaConexao.Equals(DnTipoDeBancoDeDadosAttribute.Identifier, StringComparison.InvariantCultureIgnoreCase));
                    if (conn.Count() > 1)
                    {
                        throw new DesenvolvimentoIncorretoException($"More than one connection of the same type was found with the same identifier \"{DnTipoDeBancoDeDadosAttribute.Identifier}\"");
                    }

                    if (conn.Count() == 0)
                    {
                        throw new DesenvolvimentoIncorretoException($"Could not find connection of requested \"{DnTipoDeBancoDeDadosAttribute.TipoDeBancoDeDados}\" type and identifier \"{DnTipoDeBancoDeDadosAttribute.Identifier}\" in entity \"{typeof(T).Name}\"");
                    }
                    connetion = conn.Single();
                }

                var transactionObjectsType = repository.TipoDeObjetosTransacionais;
                transactionObjects = FabricaDeObjetosTransacionais.Criar(transactionObjectsType, connetion, service.SessaoDaRequisicao);
                service.SessaoDaRequisicao.ObjetosDaTransacao = transactionObjects;
            }

            repository.ObjetosTransacionais = transactionObjects;
            repository.Servico = service;

            repository.Inicializar(); //Todo - Melhorar isso no futuro

            return repository;
        }

        internal DnRepositorio<T> Create<T>(Type repositoryType) where T : DnEntidadeBase
        {
            return Activator.CreateInstance(repositoryType) as DnRepositorio<T>;
        }

        //Todo - validar no boot se todas as entidades tem tipo de BD,ou se só tem um tipo de bd instanciado na aplicação
        private DnTipoDeBancoDeDadosAttribute GetTheEntityTipoDeBancoDeDadosAttribute(Type type)
        {
            return type.GetCustomAttribute<DnTipoDeBancoDeDadosAttribute>();
        }
    }
}