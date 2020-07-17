using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

using Microsoft.IdentityModel.Tokens;

namespace dn32.infra
{
    public static class UtilitarioDeAutenticacao
    {
        public static string GerarToken(List<KeyValuePair<string, string>> claims)
        {
            var claims_ = claims.Select(x => new Claim(x.Key, x.Value));
            var identity = new ClaimsIdentity(new GenericIdentity(Guid.NewGuid().ToString("N")), claims_);
            return GerarToken(identity, Setup.ConfiguracoesGlobais.InformacoesDoJWT.Expires ?? TimeSpan.FromDays(1));
        }

        public static string GerarToken(ClaimsIdentity identity, TimeSpan tempoParaExpiracao)
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

        public static ClaimsPrincipal ObterPrincipal(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new AccessViolationException("Token não informado");
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            if (!(tokenHandler.ReadToken(token) is JwtSecurityToken))
            {
                throw new AccessViolationException();
            }

            var validationParameters = ObterDadosDoToken();

            try
            {
                return tokenHandler.ValidateToken(token, validationParameters, out SecurityToken securityToken);
            }
            catch (SecurityTokenExpiredException)
            {
                throw new TimeoutException("token expirado");
            }
            catch (SecurityTokenInvalidSignatureException)
            {
                throw new AccessViolationException("Token inválido");
            }
        }

        private static TokenValidationParameters ObterDadosDoToken()
        {
            var Info = Setup.ConfiguracoesGlobais.InformacoesDoJWT;
            return new TokenValidationParameters
            {
                IssuerSigningKey = Info.SymmetricSecurityKey,
                ValidAudience = Info.Audience,
                ValidIssuer = Info.Issuer,
                ValidateIssuerSigningKey = Info.ValidateIssuerSigningKey,
                ValidateLifetime = Info.ValidateLifetime,
                ValidateIssuer = Info.ValidateIssuer,
                ValidateAudience = Info.ValidateAudience,
                ClockSkew = Info.ClockSkew
            };
        }
    }
}