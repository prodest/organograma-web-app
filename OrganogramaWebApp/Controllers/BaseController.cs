﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApp.Autorizacao;
using WebApp.Models;

namespace WebApp.Controllers
{
    [Authorize]
    public class BaseController: Controller
    {
        public UsuarioLogado usuario = new UsuarioLogado();
        public BaseController()
        {            
                                       
        }

        [AllowAnonymous]
        public ActionResult Signout()
        {
            Request.GetOwinContext().Authentication.SignOut();
            return Redirect("/");
        }

        [AllowAnonymous]
        public void SignoutCleanup(string sid)
        {
            var cp = (ClaimsPrincipal)User;
            var sidClaim = cp.FindFirst("sid");
            if (sidClaim != null && sidClaim.Value == sid)
            {
                Request.GetOwinContext().Authentication.SignOut("Cookies");
            }
        }

        [NonAction]
        public string ConvertAnexoBase64(string anexo)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(anexo);
            return Convert.ToBase64String(plainTextBytes);           
        }

        [NonAction]
        public string ConvertAnexoBase64_2(string anexo)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(anexo);            
            MemoryStream stream = new MemoryStream(byteArray);
            Byte[] Content = new BinaryReader(stream).ReadBytes(anexo.Length);

            var conteudo = Convert.ToBase64String(Content);
            return conteudo;
        }

        [NonAction]
        public string ConvertAnexoBase64_3(string anexo)
        {
            byte[] byteArray = new byte[(int)anexo.Length + 1];
            Stream stream = new MemoryStream(byteArray);
            Byte[] Content = new BinaryReader(stream).ReadBytes(anexo.Length);
            
            var conteudo = Convert.ToBase64String(Content);

            return conteudo;

        }

        [NonAction]
        protected void AdicionarMensagem(TipoMensagem tipo, string texto, string titulo = null)
        {
            ListaMensagens.AdicionarMensagem(tipo, texto, titulo);
        }

        public ActionResult MensagensAjax()
        {
            DesabilitarCache();
            return PartialView("MensagensAjax");
        }

        [NonAction]
        protected void DesabilitarCache()
        {
            //Desabilitando cache deste response para evitar duplicação de mensagens pelo cache do navegador
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.Now);
        }

        [HttpGet]
        public ActionResult Municipios(string uf)
        {
            WorkServiceBase base_ws = new WorkServiceBase();

            var municipios = JsonConvert.DeserializeObject<List<Municipio>>(base_ws.GetMunicipios(uf, usuario.Token).content);

            return Json(municipios, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UnidadesPorOrganizacao(string guidOrganizacao)
        {
            WorkServiceBase base_ws = new WorkServiceBase();

            var municipios = JsonConvert.DeserializeObject<List<Municipio>>(base_ws.GetUnidadesPorOrganizacao(guidOrganizacao, usuario.Token).content);

            return Json(municipios, JsonRequestBehavior.AllowGet);
        }
    }
}