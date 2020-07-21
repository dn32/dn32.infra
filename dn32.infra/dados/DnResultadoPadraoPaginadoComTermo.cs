

namespace dn32.infra
{
    [DnDocAttribute]
    public class DnResultadoPadraoPaginadoComTermo<T> : DnResultadoPadraoPaginado<T>
    {
        public string Termo { get; }

        public DnResultadoPadraoPaginadoComTermo() { }

        public DnResultadoPadraoPaginadoComTermo(T dados, DnPaginacao paginacao, string termo) : base(dados, paginacao)
        {
            Termo = termo;
        }
    }
}