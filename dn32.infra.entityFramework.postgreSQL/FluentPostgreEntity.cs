using dn32.infra.atributos;
using dn32.infra.dados;
using dn32.infra.enumeradores;

namespace dn32.infra.EntityFramework.PostgreSQL
{
    [DnTipoDeBancoDeDadosAtributo(EnumTipoDeBancoDeDados.POSTGREE_SQL)]
    public abstract class DnPostgreEntity : DnEntidade
    {
    }
}
