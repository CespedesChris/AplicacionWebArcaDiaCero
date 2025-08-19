using Arca.Data.Models;
using Arca.Data.Repositories;
using Arca.MVC.Models;
//------------------*
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;



namespace Arca.MVC.Controllers
{

    public class ReporteController : Controller
    {
        private readonly SemillaRepository _semillaRepository;
        private readonly ReporteProgramadoRepository _reporteProgRepository; // ESTO ES NUEVO 18/8/25
        private readonly EmailService _emailService;

        // Inyección del repositorio en el constructor
        public ReporteController(SemillaRepository semillaRepository,
                        ReporteProgramadoRepository reporteProgRepository, EmailService emailService) // NUEVO 18/8/25
        {
            _semillaRepository = semillaRepository;
            _reporteProgRepository = reporteProgRepository; // <--- NUEVO 18/8/25
            _emailService = emailService; // <--- NUEVO 18/8/25
        }

        //*--------PARA ENVIO MANUAL *-------------------
        [HttpGet]
        public IActionResult EnviarReporte(int id)
        {
            var reporte = _reporteProgRepository.ObtenerPorId(id);
            if (reporte == null)
                return NotFound();

            // Deserializar parámetros
            var parametros = JsonSerializer.Deserialize<ParametrosDto>(reporte.Parametros);

            // Generar lista de semillas usando los parámetros
            var lista = _semillaRepository.ObtenerSemillasConNombres();

            if (parametros.FechaInicio.HasValue)
                lista = lista.Where(s => s.FechaAlmacenamiento >= parametros.FechaInicio.Value).ToList();
            if (parametros.FechaFin.HasValue)
                lista = lista.Where(s => s.FechaAlmacenamiento <= parametros.FechaFin.Value).ToList();
            if (parametros.EspecieId.HasValue)
                lista = lista.Where(s => s.IdEspecie == parametros.EspecieId.Value).ToList();
            if (parametros.UbicacionId.HasValue)
                lista = lista.Where(s => s.IdUbicacion == parametros.UbicacionId.Value).ToList();

            // Generar archivo según formato
            byte[] archivoBytes;
            string nombreArchivo;
            switch (reporte.Formato.ToLower())
            {
                case "pdf":
                    archivoBytes = ExportarPdfBytes(lista); // Método similar a ExportarPdf pero devuelve byte[]
                    nombreArchivo = "ReporteSemillas.pdf";
                    break;
                case "xlsx":
                    archivoBytes = ExportarExcelBytes(lista); // Método similar a ExportarExcel pero devuelve byte[]
                    nombreArchivo = "ReporteSemillas.xlsx";
                    break;
                case "csv":
                    archivoBytes = ExportarCsvBytes(lista); // Método similar a ExportarCsv pero devuelve byte[]
                    nombreArchivo = "ReporteSemillas.csv";
                    break;
                default:
                    return BadRequest("Formato no soportado");
            }

            // Enviar correo
            _emailService.EnviarCorreo(reporte.Destinatarios, $"Reporte: {reporte.NombreReporte}", "Adjunto el reporte solicitado.", archivoBytes, nombreArchivo);
            TempData["Mensaje"] = "El reporte se envió correctamente al correo.";

            return RedirectToAction("Programados");
        }







        //*------------Para envio de correos -------------------

        public IActionResult Index()
        {
            var vm = new ReporteFiltroViewModel
            {
                Especies = _semillaRepository.ObtenerTodasEspecies()
                            .Select(e => new SelectListItem
                            {
                                Value = e.IdEspecie.ToString(),
                                Text = e.NombreComun
                            })
                            .ToList(),

                Ubicaciones = _semillaRepository.ObtenerTodasUbicaciones()
                                .Select(u => new SelectListItem
                                {
                                    Value = u.IdUbicacion.ToString(),
                                    Text = u.Nombre
                                })
                                .ToList()
            };

            return View(vm);
        }

        [HttpGet]
        public IActionResult Resultado()
        {
            return View();
        }
        //-------------------------------------------------------------------
        //-----------* POST RESULTADO DE INVENTARIO
        //-------------------------------------------------------------------
        [HttpPost]
        public IActionResult Resultado(ReporteFiltroViewModel filtros)
        {
            // Obtener todas las semillas
            //var lista = _semillaRepository.ObtenerTodas();
            var lista = _semillaRepository.ObtenerSemillasConNombres();
            // Filtrar por rango de fechas
            if (filtros.FechaInicio.HasValue)
                lista = lista.Where(s => s.FechaAlmacenamiento >= filtros.FechaInicio.Value).ToList();

            if (filtros.FechaFin.HasValue)
                lista = lista.Where(s => s.FechaAlmacenamiento <= filtros.FechaFin.Value).ToList();

            // Filtrar por especie
            if (filtros.EspecieId.HasValue)
                lista = lista.Where(s => s.IdEspecie == filtros.EspecieId.Value).ToList();

            // Filtrar por ubicación
            if (filtros.UbicacionId.HasValue)
                lista = lista.Where(s => s.IdUbicacion == filtros.UbicacionId.Value).ToList();


            // Guardar filtros en ViewBag para los botones
            ViewBag.FechaInicio = filtros.FechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = filtros.FechaFin?.ToString("yyyy-MM-dd");
            ViewBag.EspecieId = filtros.EspecieId;
            ViewBag.UbicacionId = filtros.UbicacionId;

            // Retornar la vista Resultado con la lista filtrada
            return View("Resultado", lista);
        }
        //-------------------------------------------------------------------
        //-----------* FIN POST RESULTADO DE INVENTARIO
        //-------------------------------------------------------------------


        //-------------------------------------------------------------------
        //-----------* EXPORTAR INVENTARIO
        //-------------------------------------------------------------------

        public IActionResult Exportar(string formato, DateTime? fechaInicio, DateTime? fechaFin, int? especieId, int? ubicacionId)
        {
            // Traer datos usando tu método existente
            var lista = _semillaRepository.ObtenerSemillasConNombres();

            // Aplicar filtros como en Resultado
            if (fechaInicio.HasValue)
                lista = lista.Where(s => s.FechaAlmacenamiento >= fechaInicio.Value).ToList();
            if (fechaFin.HasValue)
                lista = lista.Where(s => s.FechaAlmacenamiento <= fechaFin.Value).ToList();
            if (especieId.HasValue)
                lista = lista.Where(s => s.IdEspecie == especieId.Value).ToList();
            if (ubicacionId.HasValue)
                lista = lista.Where(s => s.IdUbicacion == ubicacionId.Value).ToList();

            // Seleccionar formato
            switch (formato?.ToLower())
            {
                case "pdf":
                    return ExportarPdf(lista);
                case "xlsx":
                    return ExportarExcel(lista);
                case "csv":
                    return ExportarCsv(lista);
                default:
                    return BadRequest("Formato no soportado");
            }
        }
        //-------------------------------------------------------------------
        //-----------* FIN EXPORTAR INVENTARIO
        //-------------------------------------------------------------------

        //-------------------------------------------------------------------
        //-----------* METODOS PARA EXPORTAR Y ENVIAR POR CORREO
        //---------------------EXPORTAR PDF CSV y EXCEL
        //-------------------------------------------------------------------
        private byte[] ExportarPdfBytes(List<Data.Models.SemillaViewModel> lista)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);
                    page.Content().Column(col =>
                    {
                        col.Item().Text("Reporte de Semillas").FontSize(20).Bold();
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50);
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.ConstantColumn(50);
                                columns.RelativeColumn();
                            });
                            table.Header(header =>
                            {
                                header.Cell().Text("ID");
                                header.Cell().Text("Nombre");
                                header.Cell().Text("Especie");
                                header.Cell().Text("Ubicación");
                                header.Cell().Text("Cantidad");
                                header.Cell().Text("Fecha");
                            });
                            foreach (var s in lista)
                            {
                                table.Cell().Text(s.IdSemilla.ToString());
                                table.Cell().Text(s.Nombre);
                                table.Cell().Text(s.NombreEspecie);
                                table.Cell().Text(s.NombreUbicacion);
                                table.Cell().Text(s.Cantidad.ToString());
                                table.Cell().Text(s.FechaAlmacenamiento.ToShortDateString());
                            }
                        });
                    });
                });
            }).GeneratePdf();
        }
        private byte[] ExportarExcelBytes(List<Data.Models.SemillaViewModel> lista)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte");

            // Encabezados
            worksheet.Cell(1, 1).Value = "Nombre";
            worksheet.Cell(1, 2).Value = "Especie";
            worksheet.Cell(1, 3).Value = "Ubicación";
            worksheet.Cell(1, 4).Value = "Cantidad";
            worksheet.Cell(1, 5).Value = "FechaAlmacenamiento";

            // Datos
            for (int i = 0; i < lista.Count; i++)
            {
                var s = lista[i];
                worksheet.Cell(i + 2, 1).Value = s.Nombre;
                worksheet.Cell(i + 2, 2).Value = s.NombreEspecie;
                worksheet.Cell(i + 2, 3).Value = s.NombreUbicacion;
                worksheet.Cell(i + 2, 4).Value = s.Cantidad;
                worksheet.Cell(i + 2, 5).Value = s.FechaAlmacenamiento.ToShortDateString();
            }

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            return ms.ToArray();
        }

        private byte[] ExportarCsvBytes(List<Data.Models.SemillaViewModel> lista)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Nombre,Especie,Ubicacion,Cantidad,FechaAlmacenamiento");

            foreach (var s in lista)
                sb.AppendLine($"{s.Nombre},{s.NombreEspecie},{s.NombreUbicacion},{s.Cantidad},{s.FechaAlmacenamiento:yyyy-MM-dd}");

            return Encoding.UTF8.GetBytes(sb.ToString());
        }





        //-------------------------------------------------------------------
        //-----------* EXPORTAR CSV
        //-------------------------------------------------------------------
        private IActionResult ExportarCsv(List<Data.Models.SemillaViewModel> lista)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Nombre,Especie,Ubicacion,Cantidad,FechaAlmacenamiento");

            foreach (var s in lista)
                sb.AppendLine($"{s.Nombre},{s.NombreEspecie},{s.NombreUbicacion},{s.Cantidad},{s.FechaAlmacenamiento:yyyy-MM-dd}");

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", "reporte.csv");
        }
        //-------------------------------------------------------------------
        //-------------------------------------------------------------------
        //-----------* EXPORTAR EXCEL
        //-------------------------------------------------------------------
        private IActionResult ExportarExcel(List<Data.Models.SemillaViewModel> lista)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Reporte");

            // Encabezados
            worksheet.Cell(1, 1).Value = "Nombre";
            worksheet.Cell(1, 2).Value = "Especie";
            worksheet.Cell(1, 3).Value = "Ubicacion";
            worksheet.Cell(1, 4).Value = "Cantidad";
            worksheet.Cell(1, 5).Value = "FechaAlmacenamiento";

            // Datos
            for (int i = 0; i < lista.Count; i++)
            {
                var s = lista[i];
                worksheet.Cell(i + 2, 1).Value = s.Nombre;
                worksheet.Cell(i + 2, 2).Value = s.NombreEspecie;
                worksheet.Cell(i + 2, 3).Value = s.NombreUbicacion;
                worksheet.Cell(i + 2, 4).Value = s.Cantidad;
                worksheet.Cell(i + 2, 5).Value = s.FechaAlmacenamiento.ToShortDateString();
            }

            using var ms = new MemoryStream();
            workbook.SaveAs(ms);
            ms.Position = 0;

            return File(ms.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "reporte.xlsx");
        }
        //-------------------------------------------------------------------
        //-------------------------------------------------------------------
        //-----------* EXPORTAR PDF
        //-------------------------------------------------------------------
        private IActionResult ExportarPdf(List<Data.Models.SemillaViewModel> lista)
        {
            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(20);
                    page.Size(PageSizes.A4);

                    page.Content().Column(col =>
                    {
                        col.Item().Text("Reporte de Semillas").FontSize(20).Bold();

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.ConstantColumn(50); // Id
                                columns.RelativeColumn();   // Nombre
                                columns.RelativeColumn();   // Especie
                                columns.RelativeColumn();   // Ubicación
                                columns.ConstantColumn(50); // Cantidad
                                columns.RelativeColumn();   // Fecha
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell().Text("ID");
                                header.Cell().Text("Nombre");
                                header.Cell().Text("Especie");
                                header.Cell().Text("Ubicación");
                                header.Cell().Text("Cantidad");
                                header.Cell().Text("Fecha");
                            });

                            // Data rows
                            foreach (var s in lista)
                            {
                                table.Cell().Text(s.IdSemilla.ToString());
                                table.Cell().Text(s.Nombre);
                                table.Cell().Text(s.NombreEspecie);
                                table.Cell().Text(s.NombreUbicacion);
                                table.Cell().Text(s.Cantidad.ToString());
                                table.Cell().Text(s.FechaAlmacenamiento.ToShortDateString());
                            }
                        });
                    });
                });
            }).GeneratePdf();

            return File(pdfBytes, "application/pdf", "ReporteSemillas.pdf");
        }
        //-------------------------------------------------------------------


        //-----------------------------------------------------------------------------------------------------
        // *----------------------------------* A PARTIR DE ACA TODO NUEVO PARA PROGRAMAR REPORTE -------------
        //-----------------------------------------------------------------------------------------------------
        // POST: /Reporte/Programar
        [HttpPost]
        public IActionResult Programar(ReporteFiltroViewModel filtros)
        {
            // Empaquetar los filtros en JSON para guardarlos
            var parametros = new
            {
                filtros.FechaInicio,
                filtros.FechaFin,
                filtros.EspecieId,
                filtros.UbicacionId
            };
            var json = JsonSerializer.Serialize(parametros);

            var entidad = new ReporteProgramado
            {
                NombreReporte = "Inventario de Semillas",
                Formato = string.IsNullOrWhiteSpace(filtros.Formato) ? "pdf" : filtros.Formato.ToLower(),
                Frecuencia = string.IsNullOrWhiteSpace(filtros.Frecuencia) ? "unico" : filtros.Frecuencia.ToLower(),
                Destinatarios = filtros.Destinatarios ?? string.Empty,
                ProximoEnvio = filtros.ProximoEnvio,
                Parametros = json,
                FechaCreacion = DateTime.Now
            };

            _reporteProgRepository.Insertar(entidad);
            TempData["Ok"] = "Programación guardada correctamente.";
            return RedirectToAction("Programados");
        }

        // GET: /Reporte/Programados
        [HttpGet]
        public IActionResult Programados()
        {
            var lista = _reporteProgRepository.Listar();
            return View(lista);
        }

        // GET: /Reporte/EditarProgramado/5
        [HttpGet]
        public IActionResult EditarProgramado(int id)
        {
            var r = _reporteProgRepository.ObtenerPorId(id);
            if (r == null) return NotFound();
            return View(r);
        }

        // POST: /Reporte/EditarProgramado
        [HttpPost]
        public IActionResult EditarProgramado(ReporteProgramado r)
        {
            if (!ModelState.IsValid) return View(r);
            _reporteProgRepository.Actualizar(r);
            TempData["Ok"] = "Programación actualizada.";
            return RedirectToAction("Programados");
        }

        // GET: /Reporte/EliminarProgramado/5
        [HttpGet]
        public IActionResult EliminarProgramado(int id)
        {
            var r = _reporteProgRepository.ObtenerPorId(id);
            if (r == null) return NotFound();
            return View(r);
        }

        // POST: /Reporte/EliminarProgramadoConfirmado/5
        [HttpPost]
        public IActionResult EliminarProgramadoConfirmado(int id)
        {
            _reporteProgRepository.Eliminar(id);
            TempData["Ok"] = "Programación eliminada.";
            return RedirectToAction("Programados");
        }

        // (Opcional) GET: /Reporte/Enviar/5  -> genera archivo al vuelo con los parámetros guardados
        [HttpGet]
        public IActionResult Enviar(int id)
        {
            var r = _reporteProgRepository.ObtenerPorId(id);
            if (r == null) return NotFound();

            // Leer parámetros (los mismos que usás en Exportar)
            var p = JsonSerializer.Deserialize<ParametrosDto>(r.Parametros);
            // Reusar tu Exportar:
            return Exportar(r.Formato, p?.FechaInicio, p?.FechaFin, p?.EspecieId, p?.UbicacionId);
        }

        private class ParametrosDto
        {
            public DateTime? FechaInicio { get; set; }
            public DateTime? FechaFin { get; set; }
            public int? EspecieId { get; set; }
            public int? UbicacionId { get; set; }
        }
    }
//*-----------------------Fin de lo nuevo


}
