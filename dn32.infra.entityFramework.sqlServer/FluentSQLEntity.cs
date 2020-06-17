using dn32.infra.dados;
using dn32.infra.enumeradores;
using dn32.infra.atributos;

namespace dn32.infra.EntityFramework.SqlServer
{
    [DnTipoDeBancoDeDadosAtributo(EnumTipoDeBancoDeDados.SQL_SERVER)]
    public abstract class DnSQLEntity : DnEntidade
    {
    }
}
