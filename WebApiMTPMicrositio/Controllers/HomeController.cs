using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MTPMicrositio.BLL;
using Newtonsoft.Json;
using WebApiMTPMicrositio.Models.Web;

namespace WebApiMTPMicrositio.Controllers
{
    public class HomeController : Controller
    {

        // Reutilizar HttpClient para evitar el agotamiento de sockets (Socket Exhaustion)
        private static readonly HttpClient _httpClient;

        static HomeController()
        {
            _httpClient = new HttpClient();
            // Configura aquí la URL base de tu Web API (puedes leerla desde el Web.config)
            // Ejemplo: _httpClient.BaseAddress = new Uri(ConfigurationManager.AppSettings["WebApiBaseUrl"]);
            _httpClient.BaseAddress = new Uri("http://localhost:59726/"); // Ajustar al puerto local correspondiente
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }

        public ActionResult QuienesSomos()
        {
            ViewBag.Title = "Quienes somos";

            return View();
        }
        public ActionResult Certificaciones()
        {
            ViewBag.Title = "Certificaciones";

            return View();
        }
        public ActionResult Servicios()
        {
            ViewBag.Title = "Servicios";

            return View();
        }
        public ActionResult Comotrabajamos()
        {
            ViewBag.Title = "Cómo trabajamos";

            return View();
        }
        public ActionResult CasosExito()
        {
            ViewBag.Title = "Casos de Éxito";

            return View();
        }
        public ActionResult Contacto()
        {
            ViewBag.Title = "Contacto";

            return View(new ContactoViewModel());
        }

        public ActionResult Sectores()
        {
            ViewBag.Title = "Contacto";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Validación de seguridad estricta para el formulario de la Vista
        public async Task<ActionResult> Contacto(ContactoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // 1. Serializar el ViewModel a formato JSON
                string jsonPayload = JsonConvert.SerializeObject(model);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // 2. Consumir el controlador asíncrono de la API (api/v1/contacto/enviar)
                HttpResponseMessage response = await _httpClient.PostAsync("api/v1/contacto/enviar", content);

                // 3. Procesar la respuesta del API Controller
                if (response.IsSuccessStatusCode)
                {
                    ViewBag.MensajeExito = "¡Gracias por contactarnos! Tu información ha sido procesada exitosamente y un consultor de MTP se comunicará contigo.";
                    return View(new ContactoViewModel()); // Limpia el formulario tras el éxito
                }
                else
                {
                    // Leer el error detallado enviado por la Web API si es necesario
                    string errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", "Se rechazó la solicitud. Detalles: " + response.ReasonPhrase);
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Error de comunicación de red con la capa de servicios o API caída
                ModelState.AddModelError("", "No se pudo establecer comunicación con el servicio de datos de MTP: " + httpEx.Message);
            }
            catch (Exception ex)
            {
                // Errores generales imprevistos
                ModelState.AddModelError("", "Ocurrió un error inesperado al procesar el formulario: " + ex.Message);
            }

            return View(model);
        }
    }
}
