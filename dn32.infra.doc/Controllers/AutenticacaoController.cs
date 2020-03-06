using dn32.infra.Factory;
using dn32.infra.Nucleo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using dn32.infra.extensoes;
using dn32.infra.nucleo.servicos;
using dn32.infra.nucleo.configuracoes;
using dn32.infra.nucleo.fabricas;
using dn32.infra.nucleo.modelos;

namespace dn32.infra.Nucleo.Doc.Controllers
{
    [AllowAnonymous]
    public class AutenticacaoController : Controller
    {
        [HttpGet]
        [Route("DnDoc/Autenticacao")]
        public IActionResult Index()
        {
            if (Setup.ConfiguracoesGlobais.InformacoesDoJWT == null)
            {
                throw new InvalidOperationException("Use UseJwt at Arquitetura startup to set autenticacao parameters");
            }

            return View("/Views/DnDoc/Autenticacao.cshtml");
        }

        [HttpGet]
        [Route("DnDoc/Token")]
        public IActionResult Token()
        {
            var token = Request.Cookies["Authorization"];
            if (string.IsNullOrWhiteSpace(token)) { return RedirectToAction(nameof(Index)); }
            ViewBag.Token = token;
            return View("/Views/DnDoc/Token.cshtml");
        }

        [HttpPost]
        [HttpGet]
        [Route("DnDoc/Sair")]
        public IActionResult Sair()
        {
            Response.Cookies.Delete("Authorization");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("DnDoc/Autenticacao")]
        public async Task<IActionResult> Autenticar(string email, string senha)
        {
            if (Setup.ConfiguracoesGlobais.InformacoesDoJWT == null)
                throw new InvalidOperationException("Use UseJwt at Arquitetura startup to set autenticacao parameters");

            var autenticacaoUser = new DnUsuarioParaAutenticacao { Email = email, Senha = senha };

            var service = FabricaDeServico.Criar(Setup.ConfiguracoesGlobais.InformacoesDoJWT.DnAuthenticationServiceType, HttpContext, "DocAutenticacaoServiceType for DnDoc").DnCast<DnServicoDeAutenticacao>();
            var token = await service.EntrarAsync(autenticacaoUser);
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new InvalidOperationException("Houve um erro na tentativa de autenticação");
            }

            Response.Cookies.Append("Authorization", token, new CookieOptions() { Path = "/", HttpOnly = false, Secure = false });
            return RedirectToAction(nameof(Token));
        }
    }
}
