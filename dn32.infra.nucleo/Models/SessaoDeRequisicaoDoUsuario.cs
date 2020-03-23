using dn32.infra.extensoes;
using dn32.infra.servicos;
using dn32.infra.validacoes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using dn32.infra.dados;
using dn32.infra.nucleo.servicos;
using dn32.infra.nucleo.interfaces;
using dn32.infra.nucleo.configuracoes;
using System.Threading.Tasks;

namespace dn32.infra.Nucleo.Models
{
    /// <summary>
    /// Entidade organizadora da injeção de dependência e do contexto da requisição do usuário.
    /// </summary>
    public class SessaoDeRequisicaoDoUsuario
    {
        internal Dictionary<Type, DnServicoBase> Servicos { get; set; }
        internal IDnObjetosTransacionais ObjetosDaTransacao { get; set; }
        internal Guid IdentificadorDaSessao { get; set; }
        public DnContextoDeValidacaoException ContextoDeValidacao { get; set; }
        public DnPaginacao Paginacao { get; set; }

        internal object HttpContext;

        public HttpContext LocalHttpContext => this.HttpContext as HttpContext;

        public SessaoDeRequisicaoDoUsuario()
        {
            ContextoDeValidacao = new DnContextoDeValidacaoException();
        }

        public void Dispose(bool primaryService)
        {
            Setup.RemoverSessaoDeRequisicao(this.IdentificadorDaSessao);
            ObjetosDaTransacao?.Dispose();

            foreach (var service in this.Servicos.Values)
            {
                service.Dispose(false);
            }

            if (primaryService)
            {
                Servicos.Clear();
            }
        }

        public Dictionary<string, List<object>> CodeAvailableForEntity { get; set; } = new Dictionary<string, List<object>>();

        internal object GetCodeAvailableForEntity(string key)
        {
            lock (CodeAvailableForEntity)
            {
                if (CodeAvailableForEntity.TryGetValue(key, out List<object> list))
                {
                    return list.Next();
                }
                else
                {
                    return null;
                }
            }
        }

        internal void SetCodeAvailableForEntity(string key, List<object> entitiCodes)
        {
            lock (CodeAvailableForEntity)
            {
                CodeAvailableForEntity.Remove(key);
                CodeAvailableForEntity.Add(key, entitiCodes);
            }
        }

        public bool EnableLogicalDeletion => ObjetosDaTransacao.Sessao.EnableLogicalDeletion;

        public async Task SalvarTudoAsync()
        {
            ContextoDeValidacao.Validate();
            await ObjetosDaTransacao.Sessao.SaveChangesAsync();
        }
    }
}