using dn32.infra.nucleo.erros_de_validacao;

namespace dn32.infra.nucleo.interfaces
{
    internal interface IDnValidacao
    {
        bool ChecagemDeParametroNuloOk { get; set; }

        bool ChecagemDeChavesOk { get; set; }

        void AdicionarInconsistencia(DnErroDeValidacao ex);
    }
}
