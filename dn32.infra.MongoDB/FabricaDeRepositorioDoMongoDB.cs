using System;

namespace dn32.infra
{
    internal class FabricaDeRepositorioDoMongoDB : DnFrabricaDeRepositorioBase
    {
        protected override Type ObterTipoDeRepositorioPadrao<T>() => typeof(DnMongoDBRepositorio<DnMongoDBEntidadeBase>);
    }
}
