using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace dn32.infra
{
    public abstract class DnServicoBase : IDisposable
    {
        internal bool EscopoSingleton { get; set; }

        protected virtual DnValidacaoBase Validacao { get; set; }

        protected virtual IDnRepositorioBase Repositorio { get; set; }

        protected internal SessaoDeRequisicaoDoUsuario SessaoDaRequisicao { get; private set; }

        public Guid IdentificadorDaSessaoDaRequisicao => SessaoDaRequisicao.IdentificadorDaSessao;

        protected internal HttpContext HttpContextLocal => SessaoDaRequisicao.HttpContextLocal;

        protected internal ClaimsPrincipal Usuario => SessaoDaRequisicao.HttpContextLocal?.User as ClaimsPrincipal;

        private bool Disposed { get; set; }

        public virtual DnServicoBase ObterDependenciaDeServico<TS>(string identificadorDaSessao) where TS : DnServicoBase, new()
        {
            var identificadorDaSessaoGuid = Guid.Parse(identificadorDaSessao);
            SessaoDaRequisicao = Setup.ObterSessaoDeUmaRequisicao(identificadorDaSessaoGuid);
            return SessaoDaRequisicao.Servicos.GetOrAdd(typeof(TS), (FabricaDeServico.CriarServicoEmTempoReal(typeof(TS), identificadorDaSessaoGuid) as TS));
        }

        public void Dispose(bool servicoPrimario)
        {
            if (this.EscopoSingleton) return;

            if (Disposed)
            {
                return;
            }

            Disposed = true;
            SessaoDaRequisicao.Dispose(servicoPrimario);
        }

        protected internal virtual void DefinirSessaoDoUsuario(SessaoDeRequisicaoDoUsuario sessaoDaRequisicao, Type tipoDeEntidade)
        {
            SessaoDaRequisicao = sessaoDaRequisicao;
        }

        public virtual void Dispose() => Dispose(true);
    }
}