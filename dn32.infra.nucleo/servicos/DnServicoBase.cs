using dn32.infra.Factory;
using dn32.infra.nucleo.configuracoes;
using dn32.infra.nucleo.fabricas;
using dn32.infra.nucleo.interfaces;
using dn32.infra.nucleo.validacoes;
using dn32.infra.Nucleo.Models;
using dn32.infra.validacoes;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace dn32.infra.nucleo.servicos
{
    public abstract class DnServicoBase
    {
        protected virtual DnValidacaoBase Validacao { get; set; }

        protected virtual IDnRepositorioBase Repositorio { get; set; }

        protected internal SessaoDeRequisicaoDoUsuario SessaoDaRequisicao { get; private set; }

        public Guid IdentificadorDaSessaoDaRequisicao => SessaoDaRequisicao.IdentificadorDaSessao;

        protected internal HttpContext HttpContextLocal => SessaoDaRequisicao.LocalHttpContext;

        protected internal ClaimsPrincipal Usuario => SessaoDaRequisicao.LocalHttpContext.User as ClaimsPrincipal;

        private bool Disposed { get; set; }

        public virtual DnServicoBase ObterDependenciaDeServico<TS>(string identificadorDaSessao) where TS : DnServicoBase, new()
        {
            var identificadorDaSessaoGuid = Guid.Parse(identificadorDaSessao);
            SessaoDaRequisicao = Setup.ObterSessaoDeUmaRequisicao(identificadorDaSessaoGuid);
            return SessaoDaRequisicao.Servicos.GetOrAdd(typeof(TS), (FabricaDeServico.CriarServicoEmTempoReal(typeof(TS), identificadorDaSessaoGuid) as TS));
        }

        public void Dispose(bool servicoPrimario)
        {
            if (Disposed)
            {
                return;
            }

            Disposed = true;
            SessaoDaRequisicao.Dispose(servicoPrimario);
        }

        protected internal virtual void DefinirSessaoDoUsuario(SessaoDeRequisicaoDoUsuario sessaoDaRequisicao)
        {
            SessaoDaRequisicao = sessaoDaRequisicao;
        }
    }
}