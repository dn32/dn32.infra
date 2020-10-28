using MongoDB.Bson.Serialization.Attributes;

namespace dn32.infra
{
    public abstract class DnMongoDBEntidadeBase : DnMongoDBEntidadeBaseSemId
    {
        public int CodUserAdmin { get; set; }

        public int CodUsuario { get; set; }

        public string Id => $"{CodUsuario}:{CodUserAdmin}";
    }


    [DnTipoDeBancoDeDados(EnumTipoDeBancoDeDados.MONGO_DB)]
    [DnEntidadeSemChave]
    [BsonIgnoreExtraElements(Inherited = true)]
    public abstract class DnMongoDBEntidadeBaseSemId : DnEntidade
    {
    }

    public static class UtilDeId
    {
        public static (int codUserAdmin, int codUsuario) ObterCodigosPorId(this string id)
        {
            var codUserAdmin = int.Parse(id.Split(':')[0]);
            var codUsuario = int.Parse(id.Split(':')[1]);
            return (codUserAdmin, codUsuario);
        }
    }
}
