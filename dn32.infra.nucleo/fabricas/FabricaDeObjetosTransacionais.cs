using System;
using dn32.infra;
using dn32.infra;
using dn32.infra;

namespace dn32.infra {
    public static class FabricaDeObjetosTransacionais {
        public static IDnObjetosTransacionais Criar (Type transactionObjectsType, Conexao connection, SessaoDeRequisicaoDoUsuario userSessionRequest) {
            return Activator.CreateInstance (transactionObjectsType, connection, userSessionRequest) as IDnObjetosTransacionais;
        }
    }
}