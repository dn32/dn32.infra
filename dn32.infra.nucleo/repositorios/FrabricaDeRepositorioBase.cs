using dn32.infra.atributos;
using dn32.infra.dados;
using dn32.infra.excecoes;
using dn32.infra.nucleo.configuracoes;
using dn32.infra.nucleo.fabricas;
using dn32.infra.nucleo.interfaces;
using dn32.infra.servicos;
using System;
using System.Linq;
using System.Reflection;

namespace dn32.infra.nucleo.repositorios
{
    internal abstract class FrabricaDeRepositorioBase
    {
        protected abstract Type ObterTipoDeRepositorioPadrao<T>() where T : EntidadeBase;
       
        public DnRepositorio<T> Create<T>(IDnObjetosTransacionais transactionObjects, DnServico<T> service) where T : EntidadeBase
        {
            if (Setup.ConfiguracoesGlobais.Conexoes == null) { throw new DesenvolvimentoIncorretoException($"A arquitetura não foi iniciada"); }

            var TipoDeBancoDeDadosAtributo = GetTheEntityTipoDeBancoDeDadosAtributo(typeof(T));
            if (TipoDeBancoDeDadosAtributo == null)
            {
                if (Setup.ConfiguracoesGlobais.Conexoes.Count == 1)
                {
                    TipoDeBancoDeDadosAtributo = Setup.ConfiguracoesGlobais.Conexoes.Single().TipoDoContexto.GetCustomAttribute<DnTipoDeBancoDeDadosAtributo>() ?? throw new DesenvolvimentoIncorretoException($"The entity {typeof(T).Name} needs a database type specification. Example: [TipoDeBancoDeDadosAtributo (DnTipoDeBancoDeDadosAtributo.ORACLE)]"); ;
                }
            }

            if (TipoDeBancoDeDadosAtributo == null)
            {
                throw new DesenvolvimentoIncorretoException($"The entity {typeof(T).Name} needs a database type specification. Example: [TipoDeBancoDeDadosAtributo (DnTipoDeBancoDeDadosAtributo.ORACLE)]"); ;
            }

            var localType = Setup.ConfiguracoesGlobais.TipoGenericoDeRepositorio?.MakeGenericType(typeof(T)) ?? ObterTipoDeRepositorioPadrao<T>();// typeof(DnEFRepository<T>);

            if (Setup.Repositorios.TryGetValue(typeof(T), out var repositoryType))
            {
                localType = repositoryType;
            }

            var repository = Create<T>(localType);

            if (transactionObjects == null)
            {
                Conexao connetion;

                if (string.IsNullOrWhiteSpace(TipoDeBancoDeDadosAtributo.Identifier))
                {
                    var conn = Setup.ConfiguracoesGlobais.Conexoes.Where(x => x.TipoDoContexto.GetCustomAttribute<DnTipoDeBancoDeDadosAtributo>()?.TipoDeBancoDeDados == TipoDeBancoDeDadosAtributo.TipoDeBancoDeDados);
                    if (conn.Count() > 1)
                    {
                        throw new DesenvolvimentoIncorretoException($"More than one connection of the same type was found with the same type \"{TipoDeBancoDeDadosAtributo.TipoDeBancoDeDados}\". Adicionar identifiers for them.");
                    }

                    if (conn.Count() == 0)
                    {
                        throw new DesenvolvimentoIncorretoException($"Could not find connection of requested \"{TipoDeBancoDeDadosAtributo.TipoDeBancoDeDados}\" type in entity \"{typeof(T).Name}\"");
                    }
                    connetion = conn.Single();
                }
                else
                {
                    if (Setup.ConfiguracoesGlobais.Conexoes == null) { throw new DesenvolvimentoIncorretoException($"Arquitetura was not initialized properly"); }
                    var conn = Setup.ConfiguracoesGlobais.Conexoes.Where(x =>
                                    x.TipoDoContexto.GetCustomAttribute<DnTipoDeBancoDeDadosAtributo>()?.TipoDeBancoDeDados == TipoDeBancoDeDadosAtributo.TipoDeBancoDeDados &&
                                    x.IdentificadorDaConexao.Equals(TipoDeBancoDeDadosAtributo.Identifier, StringComparison.InvariantCultureIgnoreCase));
                    if (conn.Count() > 1)
                    {
                        throw new DesenvolvimentoIncorretoException($"More than one connection of the same type was found with the same identifier \"{TipoDeBancoDeDadosAtributo.Identifier}\"");
                    }

                    if (conn.Count() == 0)
                    {
                        throw new DesenvolvimentoIncorretoException($"Could not find connection of requested \"{TipoDeBancoDeDadosAtributo.TipoDeBancoDeDados}\" type and identifier \"{TipoDeBancoDeDadosAtributo.Identifier}\" in entity \"{typeof(T).Name}\"");
                    }
                    connetion = conn.Single();
                }

                var transactionObjectsType = repository.TipoDeObjetosTransacionais;
                transactionObjects = FabricaDeObjetosTransacionais.Criar(transactionObjectsType, connetion, service.SessaoDaRequisicao);
                service.SessaoDaRequisicao.ObjetosDaTransacao = transactionObjects;
            }

            repository.ObjetosTransacionais = transactionObjects;
            repository.Servico = service;

            return repository;
        }

        internal DnRepositorio<T> Create<T>(Type repositoryType) where T : EntidadeBase
        {
            return Activator.CreateInstance(repositoryType) as DnRepositorio<T>;
        }

        //Todo - validar no boot se todas as entidades tem tipo de BD,ou se só tem um tipo de bd instanciado na aplicação
        private DnTipoDeBancoDeDadosAtributo GetTheEntityTipoDeBancoDeDadosAtributo(Type type)
        {
            return type.GetCustomAttribute<DnTipoDeBancoDeDadosAtributo>();
        }
    }
}
