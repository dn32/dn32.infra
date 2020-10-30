using MongoDB.Bson.Serialization.Attributes;

namespace dn32.infra
{
    public abstract class DnMongoDBEntidadeBase : DnMongoDBEntidadeBaseSemId
    {
        public int Tenant { get; set; }

        public int CodUsuario { get; set; }

        //[BsonIgnore]
        //public (int CodUsuario, int CodUserAdmin) Chaves() => (CodUsuario, CodUserAdmin);
    }


    [DnTipoDeBancoDeDados(EnumTipoDeBancoDeDados.MONGO_DB)]
    [DnEntidadeSemChave]
    [BsonIgnoreExtraElements(Inherited = true)]
    public abstract class DnMongoDBEntidadeBaseSemId : DnEntidade
    {
    }

    public static class UtilDeId
    {
        public static (int tenant, int Codigo) Chaves(this string id)
        {
            var tenant = int.Parse(id.Split(':')[0]);
            var codigo = int.Parse(id.Split(':')[1]);
            return (tenant, codigo);
        }
    }
}
