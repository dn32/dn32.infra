using dn32.infra.atributos;

namespace dn32.infra.dados
{
    [FluenteDocAtributo]
    public class ResultadoPadraoComTermo<T> : ResultadoPadrao<T>
    {
        public string Termo { get; }

        public ResultadoPadraoComTermo() { }

        public ResultadoPadraoComTermo(T dados, string termo) : base(dados)
        {
            Termo = termo;
        }
    }
}