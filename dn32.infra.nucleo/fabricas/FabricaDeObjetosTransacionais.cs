using dn32.infra.nucleo.configuracoes;
using dn32.infra.nucleo.interfaces;
using dn32.infra.Nucleo.Models;
using System;

namespace dn32.infra.nucleo.fabricas
{
    public static class FabricaDeObjetosTransacionais
    {
        public static IDnObjetosTransacionais Criar(Type transactionObjectsType, Conexao connection, SessaoDeRequisicaoDoUsuario userSessionRequest)
        {
            return Activator.CreateInstance(transactionObjectsType, connection, userSessionRequest) as IDnObjetosTransacionais;
        }
    }
}
