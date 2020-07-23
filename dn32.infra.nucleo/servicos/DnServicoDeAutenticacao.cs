using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;





namespace dn32.infra
{
    public abstract class DnServicoDeAutenticacao : DnServicoTransacional
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
                return UtilitarioDeAutenticacao.GerarToken(identity, Setup.ConfiguracoesGlobais.InformacoesDoJWT.Expires ?? TimeSpan.FromDays(1));
            }
            else
            {
                return "";
            }
        }
    }
}