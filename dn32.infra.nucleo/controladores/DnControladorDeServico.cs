using System;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;




using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

[assembly: InternalsVisibleTo(@"dn32.infra.Controller.Test, PublicKey= 00240000048000009400000006020000002400005253413100040000010001006d1cca26da4daf8230bb524d15453c319d38c381589ab07912b8ab6afff8174aad961a74f171790b60e5ed604bc7bad410214a7d59ed6e101c03440e3b1cd055e2bdba377915b076aa15ac9cd6da1acf488a633cb9bc2bb34536b62593950249111ac7c572e02523978ac82d829fe8be29fba6cc4f4e5b668a6cd57d39eee2aa ")]
namespace dn32.infra
{
    public abstract class DnControladorDeServico<TS> : DnControladorBase where TS : DnServicoTransacionalBase, new()
    {
        protected internal TS Servico { get; set; }

        protected internal bool TransacaoEstaAberta { get; set; }

        public virtual DnPaginacao PaginacaoDaUltimaRequisicao => this.Servico.SessaoDaRequisicao.Paginacao;

        protected internal Guid IdentificadorDaSessaoDaRequisicao => this.Servico.IdentificadorDaSessaoDaRequisicao;

        protected internal ClaimsPrincipal UsuarioDoServico => this.Servico.Usuario;

        protected internal HttpContext HttpContextDoServico => this.Servico.HttpContextLocal;

        protected DnControladorDeServico() => this.Servico = null;

        [NonAction]
        protected async Task<ResultadoPadraoComTermo<T>> CrieResultadoAsync<T>(T dados, string termo)
        {
            await this.FecharTransacaoAsync();
            return new ResultadoPadraoComTermo<T>(dados, termo);
        }

        [NonAction]
        protected async Task<ResultadoPadrao<T>> CrieResultadoAsync<T>(T dados)
        {
            await this.FecharTransacaoAsync();
            return new ResultadoPadrao<T>(dados);
        }

        [NonAction]
        protected async Task<ResultadoPadraoPaginado<T>> CrieResultadoAsync<T>(T dados, DnPaginacao paginacao)
        {
            await this.FecharTransacaoAsync();
            return new ResultadoPadraoPaginado<T>(dados, paginacao);
        }

        [NonAction]
        protected async Task<ResultadoPadraoPaginadoComTermo<T>> CrieResultadoAsync<T>(T dados, DnPaginacao paginacao, string termo)
        {
            await this.FecharTransacaoAsync();
            return new ResultadoPadraoPaginadoComTermo<T>(dados, paginacao, termo);
        }

        [NonAction]
        public override void OnActionExecuting(ActionExecutingContext contexto)
        {
            this.AbrirTransacao();
            base.OnActionExecuting(contexto);
        }

        [NonAction]
        public override void OnActionExecuted(ActionExecutedContext contexto)
        {
            this.FecharTransacaoAsync().Wait();
            base.OnActionExecuted(contexto);
        }

        [NonAction]
        protected internal void AbrirTransacao()
        {
            this.Servico = FabricaDeServico.Criar<TS>(HttpContext);
            this.TransacaoEstaAberta = true;
        }

        [NonAction]
        protected internal async Task FecharTransacaoAsync()
        {
            if (!this.TransacaoEstaAberta) return;
            await this.SalvarTransacao();
            this.LimparMemoria();
        }

        private void LimparMemoria()
        {
            if (!this.Servico.EscopoSingleton)
                this.Servico.Dispose(true);

            this.TransacaoEstaAberta = false;
        }

        private async Task SalvarTransacao()
        {
            if (this.AsValidacoesApresentamSucesso() && this.HaObjetosNaTransacao())
            {
                await this.SalvarAlteracoes();
            }
        }

        private bool AsValidacoesApresentamSucesso() =>
            this.Servico.SessaoDaRequisicao.ContextoDeValidacao.EhValido;

        private bool HaObjetosNaTransacao() => this.Servico.ObjetosDaTransacao != null;

        private async Task SalvarAlteracoes()
        {
            if (Servico.ObjetosDaTransacao.contexto.HaAlteracao)
            {
                await Servico.ObjetosDaTransacao.contexto.SaveChangesAsync();
            }
        }
    }
}