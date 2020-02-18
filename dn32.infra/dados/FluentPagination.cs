using dn32.infra.atributos;

namespace dn32.infra.dados
{
    //Todo - 001 Testar
    [FluenteDocAtributo]
    public class FluentePaginacao
    {
        #region CAMPOS

        private const int ITENS_POR_PAGINA_PADRAO = 10;
        private int _paginaAtual;
        private int _itensPorPagina;
        private bool _iniciaEmZero;

        #endregion

        #region PROPRIEDADES

        public virtual int QuantidadeTotalDeItens { get; set; }

        public virtual bool IniciaComZero => _iniciaEmZero;

        public virtual int Salto => ObterOTamanhoDoSalto();

        public virtual int ItensPorPagina => ObterItensPorPagina();

        public virtual int PaginaAtual
        {
            get => ObterPaginaAtual();
            set => _paginaAtual = value;
        }

        public virtual int NumeroDePaginas => ObterNumeroDePaginas();

        #endregion

        #region CONSTRUTORES

        public FluentePaginacao() { }

        public static FluentePaginacao Criar(int paginaAtual)
        {
            return new FluentePaginacao
            {
                _paginaAtual = paginaAtual
            };
        }

        public static FluentePaginacao Criar(int paginaAtual, bool iniciaComZero)
        {
            return new FluentePaginacao
            {
                _paginaAtual = paginaAtual,
                _iniciaEmZero = iniciaComZero
            };
        }

        public static FluentePaginacao Criar(int paginaAtual, bool iniciaComZero, int itensPorPagina)
        {
            return new FluentePaginacao
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
