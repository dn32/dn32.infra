using dn32.infra.atributos;
using dn32.infra.dados;
using dn32.infra.enumeradores;

namespace dn32.infra.EntityFramework.MemoryDatabase
{
    [DnTipoDeBancoDeDadosAtributo(EnumTipoDeBancoDeDados.MEMORY)]
    public abstract class DnMySQLEntity : DnEntidade
    {
    }
}
