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
            if (!Setup.ConfiguracoesGlobais.ConexoesForamInformadas()) { throw new DesenvolvimentoIncorretoException($"A arquitetura não foi iniciada"); }

            var DnTipoDeBancoDeDadosAttribute = GetTheEntityTipoDeBancoDeDadosAttribute(typeof(T));
            if (DnTipoDeBancoDeDadosAttribute == null)
            {
                if (Setup.ConfiguracoesGlobais.HaSomenteUmaConexao())
                {
                    DnTipoDeBancoDeDadosAttribute = Setup.ConfiguracoesGlobais.ObterUmaUnicaConexao().TipoDoContexto.GetCustomAttribute<DnTipoDeBancoDeDadosAttribute>() ??
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
                    var conn = Setup.ConfiguracoesGlobais.ObterConexoes(DnTipoDeBancoDeDadosAttribute);
                    if (conn.Count > 1)
                        throw new DesenvolvimentoIncorretoException($"Na inicialização da arquitetura, foram mencionados '{conn.Count}' conexões de banco de dados para o tipo '{DnTipoDeBancoDeDadosAttribute.ObterTipo()}'. Para que a arquitetura consiga distinguir as diversas conexões de mesmo tipo, deve-se informar ao mencionar as conexões, um identificador para cara uma delas.");

                    if (conn.Count == 0)
                        throw new DesenvolvimentoIncorretoException($"Não foi encontrado uma conexão de banco de dados para a entidade '{typeof(T).Name}' com tido especificado como '{DnTipoDeBancoDeDadosAttribute.ObterTipo()}'");
                 
                    connetion = conn.Single();
                }
                else
                {
                    if (!Setup.ConfiguracoesGlobais.ConexoesForamInformadas()) { throw new DesenvolvimentoIncorretoException($"Nenhuma conexão de banco de dados foi informada."); }
                    var conn = Setup.ConfiguracoesGlobais.ObterConexoes(DnTipoDeBancoDeDadosAttribute);
                    if (conn.Count > 1)
                    {
                        throw new DesenvolvimentoIncorretoException($"Na inicialização da arquitetura, foram mencionados '{conn.Count}' conexões de banco de dados com o identificador '{DnTipoDeBancoDeDadosAttribute.Identifier}'. Para que a arquitetura consiga distinguir as diversas conexões de mesmo tipo, deve-se identificar cada conexão com seu identificador único.");
                    }

                    if (conn.Count == 0)
                    {
                        throw new DesenvolvimentoIncorretoException($"Não foi encontrado uma conexão de banco de dados para o tido '{DnTipoDeBancoDeDadosAttribute.ObterTipo()}' e identificador '{DnTipoDeBancoDeDadosAttribute.Identifier}' na entidade '{typeof(T).Name}'");
                    }
                    connetion = conn.Single();
                }

                var transactionObjectsType = repository.TipoDeObjetosTransacionais;
                transactionObjects = FabricaDeObjetosTransacionais.Criar(transactionObjectsType, connetion, service.SessaoDaRequisicao);
                service.SessaoDaRequisicao.AdicionarObjetosDaTransacao(UtilitarioDeFabrica.ObterTipoDebancoDeDados(typeof(T)), transactionObjects);
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