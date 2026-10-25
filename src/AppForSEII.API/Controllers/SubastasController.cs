using AppForSEII.API.Data;
using AppForSEII.API.DTOs.SubastaDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AppForSEII.API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class SubastasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<SubastasController> _logger;

    public SubastasController(ApplicationDbContext context, ILogger<SubastasController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(IList<SubastaDetailDTO>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetDetailsForSubastas(int id)
    {
        var subasta = await _context.Subastas
            .Include(s => s.Usuario)
            .Include (s => s.SubastaItems)
               .ThenInclude(si => si.Libro)
            .Where(s => s.Id == id)
            .Select(s => new SubastaDetailDTO(
                s.Usuario.Name,
                s.Usuario.Surname,
                s.Usuario.PhoneNumber,
                s.FechaSubasta,
                s.PrecioSubasta,
                s.SubastaItems.Select(si => new SubastaItemDTO(
                    si.Libro.Titulo,
                    si.Libro.Autor,
                    si.Descripcion
                )).ToList(),
                s.MetodoPago is Visa ? "Visa" :
                s.MetodoPago is PayPal ? "PayPal" :
                s.MetodoPago is GooglePay ? "GooglePay" : "Desconocido"
            ))
            .FirstOrDefaultAsync();

            if (subasta == null)
            {
                return NotFound("No se ha encontrado la subasta");
            }
            return Ok(subasta);
    }
    
}