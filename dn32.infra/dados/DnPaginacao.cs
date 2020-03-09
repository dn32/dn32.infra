using dn32.infra.atributos;
using Newtonsoft.Json;

namespace dn32.infra.dados
{
    public class Parametros
    {
        public const string NomePropriedadesDesejadas = "propriedades_desejadas";
        public const string NomePropriedadesDeOrdenacao = "propriedades_de_ordenacao";
        public const string NomePaginaAtual = "pagina_atual";
        public const string NomeItensPorPagina = "itens_por_pagina";
        public const string NomeIniciarNaPaginaZero = "iniciar_na_pagina_zero";
        public const string NomeQuantidadeTotalDeItens = "quantidade_total_de_itens";
        public const string NomeSalto = "salto";
        public const string NomeNumeroTotalDePaginas = "numero_total_de_paginas";
        
    }

    //Todo - 001 Testar
    [DnDocAtributo]
    public class DnPaginacao
    {
        #region CAMPOS

        private const int ITENS_POR_PAGINA_PADRAO = 10;
        private int _paginaAtual;
        private int _itensPorPagina;
        private bool _iniciaEmZero;

        #endregion

        #region PROPRIEDADES

        [JsonProperty(Parametros.NomeQuantidadeTotalDeItens)]
        public virtual int QuantidadeTotalDeItens { get; set; }

        [JsonProperty(Parametros.NomeIniciarNaPaginaZero)]
        public virtual bool IniciaComZero => _iniciaEmZero;

        [JsonProperty(Parametros.NomeSalto)]
        public virtual int Salto => ObterOTamanhoDoSalto();

        [JsonProperty(Parametros.NomeItensPorPagina)]
        public virtual int ItensPorPagina => ObterItensPorPagina();

        [JsonProperty(Parametros.NomePaginaAtual)]
        public virtual int PaginaAtual
        {
            get => ObterPaginaAtual();
            set => _paginaAtual = value;
        }

        [JsonProperty(Parametros.NomeNumeroTotalDePaginas)]
        public virtual int NumeroDePaginas => ObterNumeroDePaginas();

        #endregion

        #region CONSTRUTORES

        public DnPaginacao() { }

        public static DnPaginacao Criar(int paginaAtual)
        {
            return new DnPaginacao
            {
                _paginaAtual = paginaAtual
            };
        }

        public static DnPaginacao Criar(int paginaAtual, bool iniciaComZero)
        {
            return new DnPaginacao
            {
                _paginaAtual = paginaAtual,
                _iniciaEmZero = iniciaComZero
            };
        }

        public static DnPaginacao Criar(int paginaAtual, bool iniciaComZero, int itensPorPagina)
        {
            return new DnPaginacao
            {
                _paginaAtual = paginaAtual,
                _itensPorPagina = itensPorPagina,
                _iniciaEmZero = iniciaComZero
            };
        }

        #endregion

        #region INTERNOS

        protected int ObterItensPorPagina() => _itensPorPagina == 0 ? ITENS_POR_PAGINA_PADRAO : _itensPorPagina;

        protected int ObterPaginaAtual() => !_iniciaEmZero && _paginaAtual == 0 ? 1 : _paginaAtual;

        protected int ObterNumeroDePaginas() => IncrementarAQuantidadeDePaginasSeSobraremItens(QuantidadeTotalDeItens / ItensPorPagina);

        protected int IncrementarAQuantidadeDePaginasSeSobraremItens(int produtoDaDivisao) => QuantidadeTotalDeItens % ItensPorPagina > 0 ? produtoDaDivisao + 1 : produtoDaDivisao;

        protected int ObterOTamanhoDoSalto() => ItensPorPagina * (IniciaComZero ? PaginaAtual : PaginaAtual - 1);

        #endregion
    }
}
