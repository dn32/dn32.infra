using dn32.infra.atributos;
using dn32.infra.dados;
using dn32.infra.enumeradores;

namespace dn32.infra.EntityFramework.MySQL
{
    [DnTipoDeBancoDeDadosAtributo(EnumTipoDeBancoDeDados.MYSQL)]
    public abstract class DnMySQLEntity : DnEntidade
    {
    }
}
