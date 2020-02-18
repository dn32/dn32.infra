using dn32.infra.atributos;

namespace dn32.infra.dados
{
    [FluenteDocAtributo]
    public class ResultadoPadraoPaginadoComTermo<T> : ResultadoPadraoPaginado<T>
    {
        public string Termo { get; }

        public ResultadoPadraoPaginadoComTermo() { }

        public ResultadoPadraoPaginadoComTermo(T dados, FluentePaginacao paginacao, string termo) :
            base(dados, paginacao)
        {
            Termo = termo;
        }
    }
}