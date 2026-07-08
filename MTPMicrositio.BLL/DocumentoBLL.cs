using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MTPMicrositio.DataAccess;
using MTPMicrositio.Entities;

namespace MTPMicrositio.BusinessLogic
{
    public class DocumentoBLL
    {
        private readonly DocumentoDAL _dal = new DocumentoDAL();

        public async Task<Guid> GuardarDocumentoAsync(DocumentoDTO documento)
        {
            try
            {
                if (string.IsNullOrEmpty(documento.Nombre))
                    throw new ArgumentException("El nombre del archivo o documento no es válido.");

                if (!string.IsNullOrEmpty(documento.ArchivoBase64) && documento.ArchivoBase64.Length > 14000000)
                    throw new Exception("El archivo adjunto excede el límite permitido de transferencia.");

                if (documento.DocumentoId == Guid.Empty)
                {
                    // Llama de forma asíncrona a la DAL
                    return await _dal.InsertarAsync(documento);
                }
                else
                {
                    // Llama de forma asíncrona a la DAL
                    await _dal.EditarAsync(documento);
                    return documento.DocumentoId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error en BLL al procesar de manera asíncrona la solicitud del documento.", ex);
            }
        }

        // Obtención Asíncrona por ID
        public async Task<DocumentoDTO> ObtenerPorIdAsync(Guid id)
        {
            try
            {
                if (id == Guid.Empty) throw new ArgumentException("ID de documento inválido.");

                return await _dal.ObtenerPorIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en BLL al buscar la entidad de manera asíncrona.", ex);
            }
        }

        // Listado de Documentos Asíncrono
        public async Task<List<DocumentoDTO>> ListarDocumentosAsync(Guid? clienteId, Guid? proyectoId)
        {
            try
            {
                return await _dal.ListarConFiltrosAsync(clienteId, proyectoId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en BLL al compilar el listado estructurado asíncrono.", ex);
            }
        }

        // Obtención de Tablas Maestras Asíncronas
        public async Task<List<CatalogoDto>> ObtenerCatalogoAsync(string tabla)
        {
            try
            {
                return await _dal.ObtenerCatalogoAsync(tabla);
            }
            catch (Exception ex)
            {
                throw new Exception("Error en BLL al leer tablas maestras de manera asíncrona.", ex);
            }
        }
    }
}