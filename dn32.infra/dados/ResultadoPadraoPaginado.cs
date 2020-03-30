using dn32.infra.atributos;

namespace dn32.infra.dados
{
    [DnDocAttribute]
    public class ResultadoPadraoPaginado<T> : ResultadoPadrao<T>
    {
        public DnPaginacao Paginacao { get; set; }

        public ResultadoPadraoPaginado() : base() { }

        public ResultadoPadraoPaginado(T dados, DnPaginacao paginacao) : base(dados)
        {
            Paginacao = paginacao;
        }
    }
}