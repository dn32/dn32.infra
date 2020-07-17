using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using dn32.infra;
using dn32.infra;
using dn32.infra;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace dn32.infra {
    public class DnAutorizacaoFilter : IAuthorizationFilter {
        public void OnAuthorization (AuthorizationFilterContext context) {
            var action = context?.ActionDescriptor as ControllerActionDescriptor;
            if (action?.ControllerTypeInfo.GetCustomAttribute<AllowAnonymousAttribute> () != null) {
                return;
            }

            if (action?.MethodInfo.GetCustomAttribute<AllowAnonymousAttribute> () != null) {
                return;
            }

            if (Setup.ConfiguracoesGlobais.InformacoesDoJWT != null) {
                ValidarAutenticacao (context);

                if (!ValideAutorizacao (context, context?.HttpContext.User)) {
                    throw new UnauthorizedAccessException ("Acesso negado");
                }
            }

            AutenticadoComSucesso (context);
        }

        protected virtual void ValidarAutenticacao (AuthorizationFilterContext context) {
            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault ()?.Replace ("Bearer", "").Trim ();
            token = string.IsNullOrWhiteSpace (token) ? context.HttpContext.Request.Query["Authorization"].ToString ()?.Replace ("Bearer", "")?.Trim () : token;
            token = string.IsNullOrWhiteSpace (token) ? context.HttpContext.Request.Cookies["Authorization"]?.Replace ("Bearer", "")?.Trim () : token;

            if (string.IsNullOrWhiteSpace (token) || token == "undefined" && token == "null") {
                throw new UnauthorizedAccessException ("É necessário enviar um token de autenticação por meio do parâmetro 'Authorization' que pode ser por cookie, header, ou query string.");
            } else {
                context.HttpContext.User = UtilitarioDeAutenticacao.ObterPrincipal (token);
            }
        }

        protected virtual bool ValideAutorizacao (AuthorizationFilterContext context, ClaimsPrincipal user) => true;

        protected virtual void AutenticadoComSucesso (AuthorizationFilterContext context) { }

        private void Forbidden (AuthorizationFilterContext context, string message) {
            ContentResult content = new ContentResult {
                ContentType = "application/json",
                Content = message.SerializarParaDnJson ()
            };

            context.Result = content;
            context.HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;
            return;
        }
    }
}