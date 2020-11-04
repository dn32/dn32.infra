using MongoDB.Bson.Serialization.Attributes;

namespace dn32.infra
{
    [DnTipoDeBancoDeDados(EnumTipoDeBancoDeDados.MONGO_DB)]
    [DnEntidadeSemChave]
    [BsonIgnoreExtraElements(Inherited = true)]
    public abstract class DnMongoDBEntidadeBase : DnEntidade
    {
    }
}
