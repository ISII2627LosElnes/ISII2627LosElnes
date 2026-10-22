using AppForSEII.API.Data;
using AppForSEII.API.CompraDTOs;
using AppForSEII.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AppForSEII.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ComprasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ComprasController> _logger;
    private readonly UserManager<ApplicationUser> _userManager;

    public ComprasController(ApplicationDbContext context, ILogger<ComprasController> logger, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _logger = logger;
        _userManager = userManager;
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(ComprasDetailDTO), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetDetailsForCompras(int id)
    {
        var compra = await _context.Compras
            .Include(c => c.Usuario)
            .Include(c => c.CompraItems)
                .ThenInclude(ci => ci.Libro)
                    .ThenInclude(l => l.Genero)
            .Include(c => c.CompraItems)
                .ThenInclude(ci => ci.Libro)
                    .ThenInclude(l => l.Editorial)
            .Where(c => c.Id == id)
            .Select(c => new ComprasDetailDTO(
                c.Usuario.Name, 
                c.Usuario.Surname,
                c.Usuario.Address,
                c.Usuario.PhoneNumber,
                c.FechaCompra,
                c.PrecioTotal,
                c.CodigoDescuento,
                c.CompraItems.Select(ci => new CompraItemDTO(
                    ci.Libro.Titulo,
                    ci.Libro.Genero.Nombre,
                    ci.Libro.Editorial.Nombre,
                    ci.Cantidad
                )).ToList(),
                c.MetodoPago is Visa? "Visa" : 
                c.MetodoPago is GooglePay? "Google Pay" : 
                c.MetodoPago is PayPal? "PayPal" : "Desconocido"

            ))
            .FirstOrDefaultAsync();

        if (compra == null)
        {
            return NotFound("No se ha encontrado la compra");
        }

        return Ok(compra);
    }
}