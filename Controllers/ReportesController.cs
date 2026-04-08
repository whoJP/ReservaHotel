using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReservaHotel.Data;
using ReservaHotel.Models;

namespace ReservaHotel.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConverter _converter;

        public ReportesController(ApplicationDbContext context, IConverter converter)
        {
            _context = context;
            _converter = converter;
        }

        // GET: Reportes — Menú principal
        public IActionResult Index()
        {
            return View();
        }

        // PDF 1: Lista de todas las reservas
        public async Task<IActionResult> ListaReservas()
        {
            var reservas = await _context.Reservas
                .Include(r => r.Hotel)
                .Include(r => r.Usuario)
                .OrderByDescending(r => r.FechaInicio)
                .ToListAsync();

            var html = GenerarHtmlListaReservas(reservas);
            return GenerarPdf(html, "ListaReservas.pdf");
        }

        // PDF 2: Reservas agrupadas por usuario
        public async Task<IActionResult> ReservasPorUsuario()
        {
            var reservas = await _context.Reservas
                .Include(r => r.Hotel)
                .Include(r => r.Usuario)
                .OrderBy(r => r.Usuario!.NombreCompleto)
                .ThenByDescending(r => r.FechaInicio)
                .ToListAsync();

            var html = GenerarHtmlReservasPorUsuario(reservas);
            return GenerarPdf(html, "ReservasPorUsuario.pdf");
        }

        // PDF 3: Hoteles más reservados
        public async Task<IActionResult> HotelesMasReservados()
        {
            var datos = await _context.Reservas
                .Include(r => r.Hotel)
                .GroupBy(r => r.Hotel!.Nombre)
                .Select(g => new HotelMasReservado
                {
                    NombreHotel = g.Key,
                    CantidadReservas = g.Count()
                })
                .OrderByDescending(x => x.CantidadReservas)
                .ToListAsync();

            var html = GenerarHtmlHotelesMasReservados(datos);
            return GenerarPdf(html, "HotelesMasReservados.pdf");
        }

        // ── Métodos privados para construir el HTML de cada PDF ──

        private string GenerarHtmlListaReservas(List<Reserva> reservas)
        {
            var filas = string.Join("", reservas.Select(r =>
            {
                int noches = (r.FechaFin - r.FechaInicio).Days;
                decimal total = noches * (r.Hotel?.PrecioNoche ?? 0);
                return $@"<tr>
                    <td>{r.Id}</td>
                    <td>{r.Usuario?.NombreCompleto}</td>
                    <td>{r.Hotel?.Nombre}</td>
                    <td>{r.FechaInicio:dd/MM/yyyy}</td>
                    <td>{r.FechaFin:dd/MM/yyyy}</td>
                    <td style='text-align:center'>{noches}</td>
                    <td style='text-align:right'>${total:N2}</td>
                </tr>";
            }));

            return $@"<html><head><style>
                body {{ font-family: Arial; font-size: 12px; margin: 30px; }}
                h1 {{ color: #0d6efd; text-align: center; margin-bottom: 5px; }}
                .subtitulo {{ text-align:center; color:#666; margin-bottom:20px; }}
                table {{ width:100%; border-collapse:collapse; }}
                th {{ background:#0d6efd; color:white; padding:8px; text-align:left; }}
                td {{ padding:6px 8px; border-bottom:1px solid #ddd; }}
                tr:nth-child(even) {{ background:#f8f9fa; }}
                .total {{ margin-top:10px; text-align:right; font-weight:bold; }}
            </style></head><body>
                <h1>🏨 ReservaHotel — Lista de Reservas</h1>
                <p class='subtitulo'>Generado el {DateTime.Now:dd/MM/yyyy HH:mm} | Total: {reservas.Count} reservas</p>
                <table>
                    <thead><tr>
                        <th>#</th><th>Cliente</th><th>Hotel</th>
                        <th>Inicio</th><th>Fin</th><th>Noches</th><th>Total</th>
                    </tr></thead>
                    <tbody>{filas}</tbody>
                </table>
            </body></html>";
        }

        private string GenerarHtmlReservasPorUsuario(List<Reserva> reservas)
        {
            var grupos = reservas.GroupBy(r => r.Usuario?.NombreCompleto ?? "Sin nombre");
            var contenido = new System.Text.StringBuilder();

            foreach (var grupo in grupos)
            {
                contenido.Append($@"
                    <h3 style='color:#198754; margin-top:20px; border-bottom:2px solid #198754; padding-bottom:4px;'>
                        👤 {grupo.Key} ({grupo.Count()} reservas)
                    </h3>
                    <table>
                        <thead><tr>
                            <th>Hotel</th><th>Fecha Inicio</th><th>Fecha Fin</th><th>Noches</th><th>Total</th>
                        </tr></thead>
                        <tbody>");

                foreach (var r in grupo)
                {
                    int noches = (r.FechaFin - r.FechaInicio).Days;
                    decimal total = noches * (r.Hotel?.PrecioNoche ?? 0);
                    contenido.Append($@"<tr>
                        <td>{r.Hotel?.Nombre}</td>
                        <td>{r.FechaInicio:dd/MM/yyyy}</td>
                        <td>{r.FechaFin:dd/MM/yyyy}</td>
                        <td style='text-align:center'>{noches}</td>
                        <td style='text-align:right'>${total:N2}</td>
                    </tr>");
                }

                contenido.Append("</tbody></table>");
            }

            return $@"<html><head><style>
                body {{ font-family: Arial; font-size: 12px; margin: 30px; }}
                h1 {{ color: #198754; text-align: center; }}
                .subtitulo {{ text-align:center; color:#666; margin-bottom:20px; }}
                table {{ width:100%; border-collapse:collapse; margin-bottom:10px; }}
                th {{ background:#198754; color:white; padding:7px; text-align:left; }}
                td {{ padding:6px 8px; border-bottom:1px solid #ddd; }}
                tr:nth-child(even) {{ background:#f8f9fa; }}
            </style></head><body>
                <h1>🏨 ReservaHotel — Reservas por Usuario</h1>
                <p class='subtitulo'>Generado el {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                {contenido}
            </body></html>";
        }

        private string GenerarHtmlHotelesMasReservados(List<HotelMasReservado> datos)
        {
            var filas = string.Join("", datos.Select((h, i) => $@"<tr>
                <td style='text-align:center'><strong>{i + 1}</strong></td>
                <td>{h.NombreHotel}</td>
                <td style='text-align:center'><strong>{h.CantidadReservas}</strong></td>
            </tr>"));

            return $@"<html><head><style>
                body {{ font-family: Arial; font-size: 13px; margin: 40px; }}
                h1 {{ color: #dc3545; text-align: center; }}
                .subtitulo {{ text-align:center; color:#666; margin-bottom:25px; }}
                table {{ width:60%; margin:0 auto; border-collapse:collapse; }}
                th {{ background:#dc3545; color:white; padding:10px; text-align:center; }}
                td {{ padding:9px 12px; border-bottom:1px solid #ddd; }}
                tr:nth-child(even) {{ background:#f8f9fa; }}
                tr:first-child td {{ font-size:15px; }}
            </style></head><body>
                <h1>🏨 Hoteles más Reservados</h1>
                <p class='subtitulo'>Generado el {DateTime.Now:dd/MM/yyyy HH:mm}</p>
                <table>
                    <thead><tr><th>#</th><th>Hotel</th><th>Reservas</th></tr></thead>
                    <tbody>{filas}</tbody>
                </table>
            </body></html>";
        }

        private FileResult GenerarPdf(string html, string nombreArchivo)
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4
                },
                Objects = {
                    new ObjectSettings {
                        HtmlContent = html,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };

            var pdf = _converter.Convert(doc);
            return File(pdf, "application/pdf", nombreArchivo);
        }
    }
}