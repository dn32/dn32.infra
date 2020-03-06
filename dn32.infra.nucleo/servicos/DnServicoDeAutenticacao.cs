using dn32.infra.nucleo.configuracoes;
using dn32.infra.nucleo.modelos;
using dn32.infra.Nucleo.Models;
using dn32.infra.servicos;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace dn32.infra.nucleo.servicos
{
    public abstract class DnServicoDeAutenticacao : DnServicoTransacionalBase
    {
        public abstract Task<(bool sucess, List<Claim> claims)> AutenticarAsync(DnUsuarioParaAutenticacao usuario);

        public virtual void Registrar(DnUsuarioParaAutenticacao usuario) { }

        public virtual async Task<string> EntrarAsync(DnUsuarioParaAutenticacao user)
        {
            if (user is null) { throw new ArgumentNullException(nameof(user)); }
            if (string.IsNullOrWhiteSpace(user.Email)) { throw new ArgumentNullException(nameof(user.Email)); }

            var (sucesso, claims) = await AutenticarAsync(user);

            if (sucesso)
            {
                if (claims == null) { claims = new List<Claim>(); }
                claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")));
                var identity = new ClaimsIdentity(new GenericIdentity(user.Email), claims);
                return GerarToken(identity, Setup.ConfiguracoesGlobais.InformacoesDoJWT.Expires ?? TimeSpan.FromDays(1));
            }
            else
            {
                return "";
            }
        }

        protected virtual string GerarToken(ClaimsIdentity identity, TimeSpan tempoParaExpiracao)
        {
            var now = DateTime.Now;
            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = Setup.ConfiguracoesGlobais.InformacoesDoJWT.Issuer,
                Audience = Setup.ConfiguracoesGlobais.InformacoesDoJWT.Audience,
                SigningCredentials = Setup.ConfiguracoesGlobais.InformacoesDoJWT.SigningCredentials,
                Subject = identity,
                NotBefore = now,
                Expires = now.Add(tempoParaExpiracao)
            });

            return handler.WriteToken(securityToken);
        }
    }
}
