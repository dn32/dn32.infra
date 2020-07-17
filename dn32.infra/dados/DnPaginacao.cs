using System;
using dn32.infra;
using dn32.infra;
using Newtonsoft.Json;

namespace dn32.infra {
    public class Parametros {
        public const string NomePropriedadesDesejadas = "propriedades_desejadas";
        public const string NomePropriedadesDeOrdenacao = "propriedades_de_ordenacao";
        public const string NomePaginaAtual = "pagina_atual";
        public const string NomeItensPorPagina = "itens_por_pagina";
        public const string NomeIniciarNaPaginaZero = "iniciar_na_pagina_zero";
        public const string NomeQuantidadeTotalDeItens = "quantidade_total_de_itens";
        public const string NomeSalto = "salto";
        public const string NomeNumeroTotalDePaginas = "numero_total_de_paginas";
        public const string DuracaoDaConsultaNoBDEmMs = "duracao_da_consulta_no_bd_em_ms";

    }

    //Todo - 001 Testar
    [DnDocAttribute]
    public class DnPaginacao {
        #region CAMPOS

        private const int ITENS_POR_PAGINA_PADRAO = 10;
        private int _paginaAtual;
        private int _itensPorPagina;
        private bool _iniciaEmZero;

        #endregion

        #region PROPRIEDADES

        [JsonProperty (Parametros.NomeQuantidadeTotalDeItens)]
        public virtual int QuantidadeTotalDeItens { get; set; }

        [JsonProperty (Parametros.NomeIniciarNaPaginaZero)]
        public virtual bool IniciaComZero => _iniciaEmZero;

        [JsonProperty (Parametros.NomeSalto)]
        public virtual int Salto => ObterOTamanhoDoSalto ();

        [JsonProperty (Parametros.NomeItensPorPagina)]
        public virtual int ItensPorPagina => ObterItensPorPagina ();

        [JsonProperty (Parametros.NomePaginaAtual)]
        public virtual int PaginaAtual {
            get => ObterPaginaAtual ();
            set => _paginaAtual = value;
        }

        [JsonProperty (Parametros.NomeNumeroTotalDePaginas)]
        public virtual int NumeroDePaginas => ObterNumeroDePaginas ();

        [JsonProperty (Parametros.DuracaoDaConsultaNoBDEmMs)]
        public long DuracaoDaConsultaNoBDEmMs { get; set; }

        #endregion

        #region CONSTRUTORES

        public DnPaginacao () { }

        public static DnPaginacao Criar (int paginaAtual) {
            return new DnPaginacao {
                _paginaAtual = paginaAtual
            };
        }

        public static DnPaginacao Criar (int paginaAtual, bool iniciaComZero) {
            return new DnPaginacao {
                _paginaAtual = paginaAtual,
                    _iniciaEmZero = iniciaComZero
            };
        }

        public static DnPaginacao Criar (int paginaAtual, bool iniciaComZero, int itensPorPagina, bool liberarMaisDe100Itens = false) {
            if (liberarMaisDe100Itens == false && itensPorPagina > 100 && itensPorPagina != int.MaxValue) {
                throw new DesenvolvimentoIncorretoException ($"Você tentou fazer uma paginação com {itensPorPagina} itens em uma página. Isso causaria um problema de performance. O máximo que pode solicitar é 100 itens por página.");
            }

            return new DnPaginacao {
                _paginaAtual = paginaAtual,
                    _itensPorPagina = itensPorPagina,
                    _iniciaEmZero = iniciaComZero
            };
        }

        #endregion

        #region INTERNOS

        protected int ObterItensPorPagina () => _itensPorPagina == 0 ? ITENS_POR_PAGINA_PADRAO : _itensPorPagina;

        protected int ObterPaginaAtual () => !_iniciaEmZero && _paginaAtual == 0 ? 1 : _paginaAtual;

        protected int ObterNumeroDePaginas () => IncrementarAQuantidadeDePaginasSeSobraremItens (QuantidadeTotalDeItens / ItensPorPagina);

        protected int IncrementarAQuantidadeDePaginasSeSobraremItens (int produtoDaDivisao) => QuantidadeTotalDeItens % ItensPorPagina > 0 ? produtoDaDivisao + 1 : produtoDaDivisao;

        protected int ObterOTamanhoDoSalto () => ItensPorPagina * (IniciaComZero ? PaginaAtual : PaginaAtual - 1);

        #endregion
    }
}