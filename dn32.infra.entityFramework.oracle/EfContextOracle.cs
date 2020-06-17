// // ReSharper disable CommentTypo
// using Microsoft.EntityFrameworkCore;

// namespace dn32.infra.EntityFramework.Oracle
// {
//     /// <inheritdoc />
//     /// <summary>
//     /// Contexto do EF no net Core
//     /// </summary>
//     [DnTipoDeBancoDeDadosAtributo(EnumTipoDeBancoDeDados.ORACLE)]
//     public class EfContextOracle : EfContext
//     {

//         public EfContextOracle(string connectionString) : base(connectionString)
//         {
//         }

//         protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//         {
//             optionsBuilder.UseOracle(ConnectionString);
//             base.OnConfiguring(optionsBuilder);
//         }
//     }
// }
