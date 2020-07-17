﻿using System;
using System.Collections.Concurrent;
using System.Linq;








namespace dn32.infra
{
    public static class FabricaDeServico
    {
        private static ConcurrentDictionary<Type, DnServicoTransacionalBase> ServicosSingleton { get; set; } = new ConcurrentDictionary<Type, DnServicoTransacionalBase>();

        internal static TS Criar<TS>(object httpContext, bool escopoSingleton = false) where TS : DnServicoTransacionalBase, new()
        {
            return Criar(typeof(TS), httpContext, null, escopoSingleton) as TS;
        }

        internal static DnServicoTransacionalBase Criar(Type tipoDeServico, SessaoDeRequisicaoDoUsuario sessaoDeRequisicao, bool escopoSingleton = false)
        {
            return Criar(tipoDeServico, sessaoDeRequisicao.HttpContext, sessaoDeRequisicao, escopoSingleton);
        }

        internal static DnServicoTransacionalBase Criar(Type tipoDeServico, object httpContext, SessaoDeRequisicaoDoUsuario sessaoDeRequisicao = null, bool escopoSingleton = false)
        {
            lock (ServicosSingleton)
            {
                if (escopoSingleton && ServicosSingleton.TryGetValue(tipoDeServico, out var servicoOut))
                    return servicoOut;

                var sessionId = Guid.NewGuid();
                tipoDeServico = ObterServicoEspecializado(tipoDeServico);
                var service = FabricaDeServicoLazy.Criar(tipoDeServico, sessionId);
                var userSession = sessaoDeRequisicao ?? CreateUserSession(httpContext, sessionId, service);
                service.DefinirSessaoDoUsuario(userSession);
                service.EscopoSingleton = escopoSingleton;

                if (escopoSingleton)
                    ServicosSingleton.TryAdd(tipoDeServico, service);

                return service;
            }
        }

        /// <summary>
        /// MUITO CUIDADO!!!! Esse método só deve ser utilizado se você estiver muito certo do que está fazendo.
        /// </summary>
        /// <typeparam Nome="TS">
        /// O tipo de serviço a ser criado.
        /// </typeparam>
        /// <param Nome="httpContext">
        /// O contexto do controller.
        /// </param>
        /// <param Nome="justification">
        /// Explique por que você está fazendo uso desse método.
        /// </param>
        /// <returns></returns>
        public static TS Criar<TS>(object httpContext, string justificativa) where TS : DnServicoTransacionalBase, new()
        {
            if (string.IsNullOrWhiteSpace(justificativa))
            {
                throw new DesenvolvimentoIncorretoException("Informe uma justividativa.");
            }

            return Criar(typeof(TS), httpContext).DnCast<TS>();
        }

        /// <summary>
        /// Específico para criação de serviço com tempo de vida manipulado manualmente.
        /// Salve as alterações por conta própria e destrua o serviço quando acabar de usar.
        /// MUITO CUIDADO!!!! Esse método só deve ser utilizado se você estiver muito certo do que está fazendo.
        /// </summary>
        /// <typeparam Nome="TS">
        /// O tipo de serviço a ser criado.
        /// </typeparam>
        /// <param Nome="httpContext">
        /// O contexto do controller.
        /// </param>
        /// <returns></returns>
        public static TS CriarServicoInterno<TS>(bool escopoSingleton = false) where TS : DnServicoTransacionalBase, new()
        {
            return Criar<TS>(null, escopoSingleton).DnCast<TS>();
        }

        public static DnServicoTransacionalBase Criar(Type tipoDeServico, object httpContext, string justificativa)
        {
            if (string.IsNullOrWhiteSpace(justificativa))
            {
                throw new DesenvolvimentoIncorretoException("Informe uma justividativa.");
            }

            if (tipoDeServico.IsDnEntity())
            {
                tipoDeServico = typeof(DnServico<>).MakeGenericType(tipoDeServico);
            }

            return Criar(tipoDeServico, httpContext).DnCast<DnServicoTransacionalBase>();
        }

        /// <summary>
        /// Cria um serviço em tempo de execução por meio de um processo de lazy-loading, à partir de um serviço original criado pelo <see cref="DnControllerController{T}"/>.
        /// </summary>
        /// <param Nome="serviceType">
        /// O tipo de serviço a ser criado.
        /// </param>
        /// <param Nome="sessionId">
        /// O identificador de sessão do usuário durante a requisição ao controller.
        /// </param>
        /// <returns>
        /// O serviço criado.
        /// </returns>
        internal static object CriarServicoEmTempoReal(Type tipoDeServico, Guid identificadorDaRequisicao)
        {
            var service = FabricaDeServicoLazy.Criar(tipoDeServico, identificadorDaRequisicao);
            service.DefinirSessaoDoUsuario(Setup.ObterSessaoDeUmaRequisicao(identificadorDaRequisicao));
            return service;
        }

        #region PRIVATE

        private static Type ObterServicoEspecializado(Type tipoDeServico)
        {
            var args = tipoDeServico.GetGenericArguments();

            if (args.Any())
            {
                var entityType = args.First();
                if (!Setup.Servicos.TryGetValue(entityType, out tipoDeServico))
                {
                    var type = (Setup.ConfiguracoesGlobais.TipoGenericoDeServico) ?? typeof(DnServico<>);
                    tipoDeServico = type.MakeGenericType(entityType);
                }
            }

            return tipoDeServico;
        }

        private static SessaoDeRequisicaoDoUsuario CreateUserSession(object httpContext, Guid identificadorDaRequisicao, DnServicoBase servico)
        {
            //Todo arrumar
            IDnObjetosTransacionais transactionObjects = null; // ITransactionObjects.Create();

            var tipoDeServico = ObterServicoEspecializado(servico.GetType());
            var tipo = Setup.ConfiguracoesGlobais.TipoDeSessaoDeRequisicaoDeUsuario ?? typeof(SessaoDeRequisicaoDoUsuario);
            var sessaoDaRequisicao = Activator.CreateInstance(tipo).DnCast<SessaoDeRequisicaoDoUsuario>();
            sessaoDaRequisicao.ObjetosDaTransacao = transactionObjects;
            sessaoDaRequisicao.IdentificadorDaSessao = identificadorDaRequisicao;
            sessaoDaRequisicao.Servicos = new ConcurrentDictionary<Type, DnServicoBase>();
            sessaoDaRequisicao.HttpContext = httpContext;
            sessaoDaRequisicao.SessaoSemContexto = httpContext == null;

            sessaoDaRequisicao.Servicos.TryAdd(tipoDeServico, servico);
            Setup.AdicionarSessaoDeRequisicao(sessaoDaRequisicao);

            return sessaoDaRequisicao;
        }

        #endregion
    }
}