namespace dn32.infra {
    public enum EnumTipoDeFiltro {
        //Numérico
        MaiorQue = 1,
        MenorQue = 2,

        //String
        IniciaCom = 4,
        TerminaCom = 8,
        Contem = 16,

        //All
        Igual = 32,
        Nulo = 64,

        //Bool
        Verdadeiro = 128,
        Falso = 256
    }
}