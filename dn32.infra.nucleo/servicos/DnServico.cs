﻿using ClosedXML.Excel;
using dn32.infra.dados;
using dn32.infra.Factory;
using dn32.infra.nucleo.interfaces;
using dn32.infra.nucleo.erros_de_validacao;
using dn32.infra.nucleo.validacoes;
using dn32.infra.Nucleo.Models;
using dn32.infra.Nucleo.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using dn32.infra.nucleo.configuracoes;
using dn32.infra.nucleo.fabricas;
using dn32.infra.excecoes;

namespace dn32.infra.servicos
{
    public class DnServico<T> : DnServicoTransacionalBase where T : EntidadeBase
    {
        #region PROPRIEDADES

        protected virtual async Task Salvo(T entidade) => await Task.Run(() => { });

        protected virtual void TransformarParaSalvar(T entidade, bool? ehAtualizacao) { }

        protected virtual void TransformarResultadoDaConsulta(T entidade) { }

        protected virtual void TransformarResultadoDaConsulta<TO>(TO entidade) { }

        protected internal new DnRepositorio<T> Repositorio
        {
            get => base.Repositorio as DnRepositorio<T>;
            set => base.Repositorio = value;
        }

        protected internal new DnValidacao<T> Validacao
        {
            get => base.Validacao as DnValidacao<T>;
            set => base.Validacao = value;
        }

        protected internal override void DefinirSessaoDoUsuario(SessaoDeRequisicaoDoUsuario sessaoDaRequisicao)
        {
            base.DefinirSessaoDoUsuario(sessaoDaRequisicao);

            ValidarInicializacaoDoServico();
            this.Repositorio = Setup.ConfiguracoesGlobais.FabricaDeRepositorio.Create(ObjetosDaTransacao, this);
            this.Validacao = FabricaDeValidacao.Criar<T>();
            this.Validacao.Inicializar(this);
        }

        #endregion

        #region PASSAGEM DIRETA PARA O REPOSITÓRIO

        public virtual async Task<List<TO>> ListarAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> especificacao, DnPaginacao paginacao = null)
        {
            var lista = await Repositorio.ListarAlternativoAsync(especificacao, paginacao);
            lista.ForEach(x => Repositorio.Desanexar(x));
            lista.ForEach(TransformarResultadoDaConsulta);
            return lista;
        }

        public virtual async Task<List<T>> ListarAsync(IDnEspecificacao especificacao, DnPaginacao pagination = null)
        {
            var lista = await Repositorio.ListarAsync(especificacao, pagination);
            lista.ForEach(x => Repositorio.Desanexar(x));
            lista.ForEach(TransformarResultadoDaConsulta);
            return lista;
        }

        public virtual async Task<TO> PrimeiroOuPadraoAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> especificacao)
        {
            var entidade = await Repositorio.PrimeiroOuPadraoAlternativoAsync(especificacao);
            entidade = Repositorio.Desanexar(entidade);
            TransformarResultadoDaConsulta(entidade);
            return entidade;
        }

        public virtual async Task<T> PrimeiroOuPadraoAsync(IDnEspecificacao especificacao)
        {
            var entidade = await Repositorio.PrimeiroOuPadraoAsync(especificacao);
            entidade = Repositorio.Desanexar(entidade);
            TransformarResultadoDaConsulta(entidade);
            return entidade;
        }

        public virtual async Task<T> UnicoOuPadraoAsync(IDnEspecificacao especificacao)
        {
            var entidade = await Repositorio.SingleOrDefaultAsync(especificacao);
            entidade = Repositorio.Desanexar(entidade);
            TransformarResultadoDaConsulta(entidade);
            return entidade;
        }

        public virtual async Task<TO> UnicoOuPadraoAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> especificacao)
        {
            var entidade = await Repositorio.UnicoOuPadraoAlternativoAsync(especificacao);
            entidade = Repositorio.Desanexar(entidade);
            TransformarResultadoDaConsulta(entidade);
            return entidade;
        }

        public virtual async Task<int> QuantidadeAlternativoAsync<TO>(IDnEspecificacaoAlternativaGenerica<TO> especificacao) => await Repositorio.QuantidadeAlternativoAsync(especificacao);

        public virtual async Task<int> QuantidadeAsync(IDnEspecificacao especificacao) => await Repositorio.QuantidadeAsync(especificacao);

        public virtual async Task<int> QuantidadeTotalAsync() => await Repositorio.QuantidadeTotalAsync();

        public virtual async Task<int> QuantidadeAsync(T entidade, bool includeExcludedLogically = false) => await Repositorio.QuantidadeAsync(entidade, includeExcludedLogically);

        public virtual void RemoverLista(IDnEspecificacao especificacao) => Repositorio.RemoverLista(especificacao);

        public virtual async Task EliminarTudoAsync(string APAGAR_TUDO)
        {
            Validacao.EliminarTudo(APAGAR_TUDO);
            await Repositorio.EliminarTudoAsync();
        }

        public virtual async Task<bool> HaSomenteUmAsync(T entidade, bool includeExcludedLogically = false) => await Repositorio.HaSomenteUmAsync(entidade, includeExcludedLogically).ConfigureAwait(false);

        public virtual async Task<bool> ExisteAsync(IDnEspecificacaoBase especificacao) => await Repositorio.ExisteAsync(especificacao);

        public virtual async Task<bool> ExisteAsync(T entidade, bool checkId = true, bool includeExcludedLogically = false) => await Repositorio.ExisteAsync(entidade, includeExcludedLogically);

        public virtual async Task<bool> ExisteAlternativoAsync<TO>(IDnEspecificacaoBase especificacao) => await Repositorio.ExisteAlternativoAsync<TO>(especificacao);

        public virtual async Task AdicionarListaAsync(params T[] entidades)
        {
            foreach (var item in entidades) { TransformarParaSalvar(item, false); }
            await Validacao.AdicionarListaAsync(entidades);
            await Repositorio.AdicionarListaAsync(entidades);

            foreach (var item in entidades)
                await Salvo(item);

            await SessaoDaRequisicao.SalvarTudoAsync();
        }

        public virtual async Task<T> AdicionarAsync(T entidade)
        {
            TransformarParaSalvar(entidade, false);
            T retorno;
            if (SessaoDaRequisicao.EnableLogicalDeletion && await ExisteAsync(entidade, true, true))
            {
                retorno = await AtualizarAsync(entidade); // Restore deleted
            }
            else
            {
                await Validacao.AdicionarAsync(entidade);
                retorno = await Repositorio.AdicionarAsync(entidade);
            }

            await Salvo(entidade);
            return retorno;
        }

        public virtual async Task<T> AdidionarOuAtualizarAsync(T entidade)
        {
            TransformarParaSalvar(entidade, null);

            var anotherServices = await Validacao.AdidionarOuAtualizarAsync(entidade);
            var exists = SessaoDaRequisicao.ContextoDeValidacao.Inconsistencies.RemoveAll(x => x.NomeDoErroDeValidacao == nameof(DnEntidadeExisteErroDeValidacao)) > 0;

            Validacao.ExecutarAsValidacoes(anotherServices);
            T retorno;

            if (exists)
            {
                retorno = await AtualizarAsync(entidade);
            }
            else
            {
                retorno = await AdicionarAsync(entidade);
            }

            await Salvo(entidade);
            return retorno;
        }

        public virtual async Task<T> BuscarAsync(T entidade, bool checarId = true, bool desanexar = true)
        {
            Validacao.Buscar(entidade, checarId);
            entidade = await Repositorio.BuscarAsync(entidade);
            entidade = desanexar ? Repositorio.Desanexar(entidade) : entidade;
            TransformarResultadoDaConsulta(entidade);
            return entidade;
        }

        public virtual async Task<T> AtualizarAsync(T entidade)
        {
            TransformarParaSalvar(entidade, true);
            await Validacao.AtualizarAsync(entidade);
            var retorno = await Repositorio.AtualizarAsync(entidade);
            await Salvo(entidade);
            return retorno;
        }

        public virtual async Task AtualizarListaAsync(params T[] entidades)
        {
            foreach (var item in entidades) { TransformarParaSalvar(item, true); }
            await Validacao.AtualizarListaAsync(entidades);
            await Repositorio.AtualizarListaAsync(entidades);
            foreach (var item in entidades)
                await Salvo(item);
        }

        public virtual async Task<T> RemoverAsync(T entidade)
        {
            await Validacao.RemoverAsync(entidade);
            return await Repositorio.RemoverAsync(entidade);
        }

        public virtual async Task RemoverListaAsync(params T[] entidades)
        {
            await Validacao.RemoverListaAsync(entidades);
            await Repositorio.RemoverListaAsync(entidades);
        }

        #endregion

        #region PRIVATE

        private static void ValidarInicializacaoDoServico()
        {
            var mth = new StackTrace()?.GetFrame(2)?.GetMethod() ?? null;
            var name = mth?.ReflectedType?.Name;
            if (name == nameof(FabricaDeServico))
            {
                return;
            }

            throw new DesenvolvimentoIncorretoException($"You can not initialize the {nameof(DnServico<T>)}");
        }

        //Todo - Em desenvolvimento a importação do XMLS
        internal virtual async Task<XLWorkbook> ImportFileStreamAsync(Stream stream)
        {
            var workbook = new XLWorkbook(stream);
            var lista = DataImportationUtil.ImportFileStream<T>(workbook);

            foreach (var item in lista)
            {
                try
                {
                    var entidade = item.Item2;
                    Validacao.LimparInconsistencias();

                    if (await ExisteAsync(entidade, true, true))
                    {
                        await Validacao.AtualizarAsync(entidade);
                        await Repositorio.AtualizarAsync(entidade);
                    }
                    else
                    {
                        await Validacao.AdicionarAsync(entidade);
                        await Repositorio.AdicionarAsync(entidade);
                    }

                    Validacao.ExecutarAsValidacoes();

                    item.Item1.Value = "Sucess!";
                    item.Item1.Style.Font.FontColor = XLColor.FromArgb(0x04AC15);
                }
                catch (Exception ex)
                {
                    item.Item1.Style.Font.FontColor = XLColor.FromArgb(0xDC4C3F);
                    item.Item1.Value = ex.Message.Replace("* ", "").Trim();
                }
            }

            Validacao.LimparInconsistencias();

            return workbook;
        }

        #endregion
    }
}