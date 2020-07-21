

namespace dn32.infra
{
    [DnDocAttribute]
    public class DnResultadoPadraoPaginado<T> : DnResultadoPadrao<T>
    {
        public DnPaginacao Paginacao { get; set; }

        public DnResultadoPadraoPaginado() : base() { }

        public DnResultadoPadraoPaginado(T dados, DnPaginacao paginacao) : base(dados)
        {
            Paginacao = paginacao;
        }
    }
}