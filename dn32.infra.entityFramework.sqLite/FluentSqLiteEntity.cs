using dn32.infra.dados;
using dn32.infra.atributos;
using dn32.infra.enumeradores;

namespace dn32.infra.EntityFramework.SqLite
{
    [DnTipoDeBancoDeDadosAtributo(EnumTipoDeBancoDeDados.SQLITE)]
    public abstract class DnSqLiteEntity : DnEntidade
    {
    }
}
