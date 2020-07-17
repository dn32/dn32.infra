﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;






[assembly: InternalsVisibleTo(@"dn32.infra.EntityFramework, PublicKey=002400000480000094000000060200000024000052534131000400000100010001e5fbcd7e6f1d70524fc7b787a6ba4d8f332e822c5506e1831f4e59ab41e930c56bbf8cc29fa91f1270f4e873c036335c5aa4ccfc76ab13bfa7372de9d4e17de6c2d188fae9e6842d7d90d51e123836fd9f5d6be5580a32d1a12e59489519c6b93cdcf7ecd782042db1f31190350fbf937bbd6a5ae61d648773b46b9a706ccf")]
namespace dn32.infra
{
    public class DnConfiguracoesGlobais
    {
        public List<Conexao> Conexoes { get; internal set; } = new List<Conexao>();
        public Type TipoDeSessaoDeRequisicaoDeUsuario { get; internal set; }
        public Type TipoGenericoDeServico { get; internal set; }
        public Type TipoGenericoDeRepositorio { get; internal set; }
        public Type TipoGenericoDeValidacao { get; internal set; }
        public Type TipoGenericoDeControlador { get; internal set; }
        internal FrabricaDeRepositorioBase FabricaDeRepositorio { get; set; }
        public InformacoesDoJWT InformacoesDoJWT { get; set; }
        public string StringDeConexaoDoRedis { get; set; }
        public Type TipoDeServicoDoRedis { get; set; }
        public Dictionary<string, string> Valores { get; set; } = new Dictionary<string, string>();
        public bool MostrarLogsDoBDEmDebug { get; set; } = true;
    }
}