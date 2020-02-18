using dn32.infra.atributos;

namespace dn32.infra.dados
{
    [FluenteDocAtributo]
    public class ResultadoPadraoPaginado<T> : ResultadoPadrao<T>
    {
        public FluentePaginacao Paginacao { get; set; }

        public ResultadoPadraoPaginado() : base() { }

        public ResultadoPadraoPaginado(T dados, FluentePaginacao paginacao) : base(dados)
        {
            Paginacao = paginacao;
        }
    }
}