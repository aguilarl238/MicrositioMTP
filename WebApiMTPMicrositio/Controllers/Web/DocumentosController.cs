using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MTPMicrositio.BLL;
using MTPMicrositio.Entities;
using MTPMicrositio.Models.Web;
using Newtonsoft.Json; // Certifique-se de ter o pacote Newtonsoft.Json instalado

namespace MTPMicrositio.Controllers
{
    public class DocumentosController : Controller
    {
        private readonly ComentarioBLL _bll = new ComentarioBLL();
        private readonly ProyectoBLL _bllProy = new ProyectoBLL();
        // Instância única ou gerenciada de HttpClient conforme as boas práticas do .NET
        private static readonly HttpClient _httpClient;

        static DocumentosController()
        {
            // Lê a URL base da API diretamente do Web.config
            string apiBaseUrl = "http://localhost:59726/";//ConfigurationManager.AppSettings["ApiBaseUrl"] ?? "http://localhost:xxxx/";

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(apiBaseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // GET: Documentos (Index com duplo filtro via API)
        public async Task<ActionResult> Index(Guid? filtroClienteId, Guid? filtroProyectoId)
        {
            try
            {
                // Construção da URL de listagem com parâmetros de filtro (Query String)
                string queryPath = $"api/v1/documentos/listar?clienteId={filtroClienteId}&proyectoId={filtroProyectoId}";

                HttpResponseMessage response = await _httpClient.GetAsync(queryPath);
                List<DocumentoDTO> documentos = new List<DocumentoDTO>();

                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    documentos = JsonConvert.DeserializeObject<List<DocumentoDTO>>(jsonResult);
                }
                else
                {
                    ViewBag.ErrorMessage = $"Erro operacional na API de dados (Código: {response.StatusCode}).";
                }

                // Carrega os combos chamando as rotas de catálogo da API
                var model = new DocumentoIndexViewModel
                {
                    FiltroClienteId = filtroClienteId,
                    FiltroProyectoId = filtroProyectoId,
                    Documentos = documentos,
                    Clientes = await _bllProy.GetSelectClientesAsync(),
                    Proyectos = (await _bll.GetProyectosCatAsync()).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Descripcion }),
                    
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Falha crítica de comunicação com o barramento de APIs: " + ex.Message;
                return View(new DocumentoIndexViewModel { Documentos = new List<DocumentoDTO>() });
            }
        }

        // GET: Documentos/Formulario (Insertar / Editar unificado consumindo API)
        public async Task<ActionResult> Formulario(Guid? id)
        {
            try
            {
                var model = new DocumentoViewModel();

                if (id.HasValue && id.Value != Guid.Empty)
                {
                    // Consome o endpoint GET de busca por ID da API Layer
                    HttpResponseMessage response = await _httpClient.GetAsync($"api/v1/documentos/obtener/{id.Value}");

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = await response.Content.ReadAsStringAsync();
                        var dto = JsonConvert.DeserializeObject<DocumentoDTO>(jsonResult);

                        if (dto != null)
                        {
                            model.DocumentoId = dto.DocumentoId;
                            model.ClienteId = dto.ClienteId;
                            model.TipoDocumentoId = dto.TipoDocumentoId;
                            model.Nombre = dto.Nombre;
                            model.Activo = dto.Activo;
                            model.EstatusId = dto.EstatusId;
                            model.FechaVigencia = dto.FechaVigencia;
                            model.ProyectoId = dto.ProyectoId;
                        }
                    }
                    else
                    {
                        TempData["Error"] = "O registro técnico solicitado não foi localizado na API de dados.";
                        return RedirectToAction("Index");
                    }
                }

                await CargarListasFormularioApiAsync(model);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Erro ao inicializar interface via API: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // POST: Documentos/Formulario (Persistência via POST na API Layer)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Formulario(DocumentoViewModel model)
        {
            try
            {
                if (model.ArchivoCargado != null && model.ArchivoCargado.ContentLength > 0)
                {
                    // Extraer extensión del archivo
                    model.ExtensionArchivo = Path.GetExtension(model.ArchivoCargado.FileName);

                    // Convertir el Stream binario a arreglo de bytes y luego a Base64
                    using (var binaryReader = new BinaryReader(model.ArchivoCargado.InputStream))
                    {
                        byte[] fileBytes = binaryReader.ReadBytes(model.ArchivoCargado.ContentLength);
                        model.ArchivoBase64 = Convert.ToBase64String(fileBytes);
                    }
                }

                if (!ModelState.IsValid)
                {
                    await CargarListasFormularioApiAsync(model);
                    return View(model);
                }

                var dto = new DocumentoDTO
                {
                    DocumentoId = model.DocumentoId,
                    ClienteId = model.ClienteId,
                    TipoDocumentoId = model.TipoDocumentoId,
                    Nombre = model.Nombre,
                    Activo = model.Activo,
                    EstatusId = model.EstatusId,
                    FechaVigencia = model.FechaVigencia,
                    ProyectoId = model.ProyectoId,
                    // Pasamos los nuevos campos mapeados
                    ArchivoBase64 = model.ArchivoBase64,
                    ExtensionArchivo = model.ExtensionArchivo,
                    UrlDoc= model.UrlDoc,
                    UsuarioId= (Guid)Session["UsuarioId"]
                };

                // Envia el DTO serializado con el Base64 incluido a la Api Layer
                string jsonContent = JsonConvert.SerializeObject(dto);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync("api/v1/documentos/guardar", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Parámetros y archivo digital guardados con éxito.";
                    return RedirectToAction("Index");
                }
                else
                {
                    string errorDetail = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = $"Error: {errorDetail}";
                    await CargarListasFormularioApiAsync(model);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Falha de conectividade no envio dos dados para a API: " + ex.Message;
                await CargarListasFormularioApiAsync(model);
                return View(model);
            }
        }

        public async Task<ActionResult> Descargar(Guid id)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"api/v1/documentos/obtener/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    var dto = JsonConvert.DeserializeObject<DocumentoDTO>(jsonResult);

                    if (dto != null && !string.IsNullOrEmpty(dto.ArchivoBase64))
                    {
                        // Convertir la cadena Base64 almacenada de vuelta a arreglo de bytes
                        byte[] fileBytes = Convert.FromBase64String(dto.ArchivoBase64);

                        // Determinar el Content-Type básico o genérico de descarga
                        string contentType = "application/octet-stream";
                        string nombreDescarga = $"MTP_Doc_{dto.Nombre.Replace(" ", "_")}{dto.ExtensionArchivo}";

                        // Retorna el archivo binario para forzar la descarga en el navegador cliente
                        return File(fileBytes, contentType, nombreDescarga);
                    }
                }
                TempData["Error"] = "El documento solicitado no cuenta con un archivo digital adjunto.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al procesar la descarga: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // Métodos Auxiliares para Alimentação dos DropDownLists via API
        private async Task CargarListasFormularioApiAsync(DocumentoViewModel model)
        {
            model.Clientes = await _bllProy.GetSelectClientesAsync();
            model.Proyectos = (await _bll.GetProyectosCatAsync()).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Descripcion });
            model.TiposDocumento = (await _bll.GetTipoDocumentoCatAsync()).Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Descripcion });
            model.Estatus = await CargarComboApiAsync("Estatus");
        }
        private class EstatusDtoSimulado { public Guid EstatusId { get; set; } public string Nombre { get; set; } }
        private async Task<List<SelectListItem>> CargarComboApiAsync(string tabla)
        {
            string uri = string.Empty;
            switch (tabla)
            {
                case "Clientes":
                    uri = "api/clientes";
                    break;
                case "Proyectos":
                    uri = "api/v1/proyectos/listar";
                    break;
                case "TiposDocumento":
                    uri = "api/estatus/Documento";
                    break;
                case "Estatus":
                    uri = "api/estatus/Seccion/Documento";
                    break;

            }
            try
            {
                // Consome a rota genérica de catálogos da API Layer
                HttpResponseMessage response = await _httpClient.GetAsync(uri);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResult = await response.Content.ReadAsStringAsync();
                    var catalogo = JsonConvert.DeserializeObject<List<EstatusDtoSimulado>>(jsonResult);

                    return catalogo
                        .Select(c => new SelectListItem { Value = c.EstatusId.ToString(), Text = c.Nombre })
                        .ToList();
                }
            }
            catch
            {
                // Tratamento preventivo em caso de queda parcial da API
            }

            return new List<SelectListItem>();
        }
    }
}