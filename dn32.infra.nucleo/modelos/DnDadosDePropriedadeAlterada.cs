namespace dn32.infra {
    public class DnDadosDePropriedadeAlterada {
        public string NomeDaPropriedade { get; set; }
        public object ValorOriginal { get; set; }
        public object NovoValor { get; set; }
        public bool HaAlteracao => ValorOriginal?.ToString () != NovoValor?.ToString ();
    }
}