using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dn32.infra
{
    /// <summary>
    /// Entidade organizadora da injeção de dependência e do contexto da requisição do usuário.
    /// </summary>
    public class SessaoDeRequisicaoDoUsuario
    {
        internal ConcurrentDictionary<Type, DnServicoBase> Servicos { get; set; }
        internal ConcurrentDictionary<EnumTipoDeBancoDeDados, IDnObjetosTransacionais> DicionarioDeObtetosDeTransacao { get; set; }
        internal List<IDnObjetosTransacionais> DicionarioDeObtetosDeTransacaoValores => DicionarioDeObtetosDeTransacao.Values.Where(x => x != null).ToList();
        internal Guid IdentificadorDaSessao { get; set; }
        public DnContextoDeValidacaoException ContextoDeValidacao { get; set; }
        public DnPaginacao Paginacao { get; set; }

        internal object HttpContext;

        public bool SessaoSemContexto { get; set; }

        public HttpContext HttpContextLocal => this.HttpContext as HttpContext;

        public SessaoDeRequisicaoDoUsuario()
        {
            ContextoDeValidacao = new DnContextoDeValidacaoException();
            DicionarioDeObtetosDeTransacao = new ConcurrentDictionary<EnumTipoDeBancoDeDados, IDnObjetosTransacionais>();
        }

        internal IDnObjetosTransacionais ObterObjetosDaTransacao(EnumTipoDeBancoDeDados tipoDeBancoDeDados)
        {
            if (DicionarioDeObtetosDeTransacao.TryGetValue(tipoDeBancoDeDados, out var valor))
                return valor;
            else
                return null;
        }

        internal void AdicionarObjetosDaTransacao(EnumTipoDeBancoDeDados tipoDeBancoDeDados, IDnObjetosTransacionais objetoDaTransacao)
        {
            DicionarioDeObtetosDeTransacao.TryRemove(tipoDeBancoDeDados, out _);
            DicionarioDeObtetosDeTransacao.TryAdd(tipoDeBancoDeDados, objetoDaTransacao);
        }

        public void Dispose(bool primaryService)
        {
            Setup.RemoverSessaoDeRequisicao(this.IdentificadorDaSessao);
            DicionarioDeObtetosDeTransacaoValores.ForEach(x => x.Dispose());
            DicionarioDeObtetosDeTransacao.Clear();

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

        //Todo - arrumar
        public bool EnableLogicalDeletion => false;// ObjetosDaTransacao.contexto.EnableLogicalDeletion;

        public async Task<int> SalvarTudoAsync()
        {
            ContextoDeValidacao.Validate();
            var lista = DicionarioDeObtetosDeTransacaoValores;
            var registros = 0;
            foreach (var item in lista)
                registros += await item.contexto.SaveChangesAsync();
            return registros;
        }
    }
}