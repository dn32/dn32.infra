using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Reflection;
using dn32.infra.extensoes;
using dn32.infra.nucleo.configuracoes;
using System.Security.Claims;

namespace dn32.infra.nucleo.filtros
{
    public class DnAutorizacaoFilter : IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var action = context?.ActionDescriptor as ControllerActionDescriptor;
            if (action?.ControllerTypeInfo.GetCustomAttribute<AllowAnonymousAttribute>() != null)
            {
                return;
            }

            if (action?.MethodInfo.GetCustomAttribute<AllowAnonymousAttribute>() != null)
            {
                return;
            }

            if (Setup.ConfiguracoesGlobais.InformacoesDoJWT != null)
            {
                ValidarAutenticacao(context);

                if (!ValideAutorizacao(context, context?.HttpContext.User))
                {
                    throw new UnauthorizedAccessException("Acesso negado");
                }
            }

            AutenticadoComSucesso(context);
        }

        protected virtual void ValidarAutenticacao(AuthorizationFilterContext context)
        {
            var tokenRequest = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer", "").Trim();
            tokenRequest = string.IsNullOrWhiteSpace(tokenRequest) ? context.HttpContext.Request.Query["Authorization"].ToString()?.Replace("Bearer", "")?.Trim() : tokenRequest;
            tokenRequest = string.IsNullOrWhiteSpace(tokenRequest) ? context.HttpContext.Request.Cookies["Authorization"]?.Replace("Bearer", "")?.Trim() : tokenRequest;

            if (string.IsNullOrWhiteSpace(tokenRequest) || tokenRequest == "undefined" && tokenRequest == "null")
            {
                throw new UnauthorizedAccessException("É necessário enviar um token de autenticação por meio do parâmetro 'Authorization' que pode ser por cookie, header, ou query string.");
            }
            else
            {
                var par = ObterDadosDoToken();
                var handler = new JwtSecurityTokenHandler();
                try
                {
                    context.HttpContext.User = handler.ValidateToken(tokenRequest, par, out SecurityToken tok);
                }
                catch (Exception ex)
                {
                    throw new UnauthorizedAccessException(ex.Message);
                }
            }
        }

        protected virtual bool ValideAutorizacao(AuthorizationFilterContext context, ClaimsPrincipal user) => true;

        protected virtual void AutenticadoComSucesso(AuthorizationFilterContext context) { }

        private void Forbidden(AuthorizationFilterContext context, string message)
        {
            ContentResult content = new ContentResult
            {
                ContentType = "application/json",
                Content = message.SerializarParaDnJson()
            };

            context.Result = content;
            context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return;
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