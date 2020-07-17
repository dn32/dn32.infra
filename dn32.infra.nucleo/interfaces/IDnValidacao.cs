

namespace dn32.infra
{
    internal interface IDnValidacao
    {
        bool ChecagemDeParametroNuloOk { get; set; }

        bool ChecagemDeChavesOk { get; set; }

        void AdicionarInconsistencia(DnErroDeValidacao ex);
    }
}