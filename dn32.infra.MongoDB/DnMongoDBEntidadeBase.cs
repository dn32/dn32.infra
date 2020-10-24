namespace dn32.infra
{
    [DnTipoDeBancoDeDados(EnumTipoDeBancoDeDados.MONGO_DB)]
    public class DnMongoDBEntidadeBase : DnEntidade
    {
        public string Id { get; set; }
    }
}
