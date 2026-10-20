using AppForSEII.API.Data;
using AppForSEII.API.ResenaDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AppForSEII.API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class ResenasController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ResenasController> _logger;

    public ResenasController(ApplicationDbContext context, ILogger<ResenasController> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(IList<ResenaDetailDTO>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetDetailsForResenas(int id)
    {
        var resena = await _context.Resenas
            .Include(r => r.Usuario)
            .Include (r => r.ResenaItems)
               .ThenInclude(ri => ri.Libro)
                    .ThenInclude(l => l.Editorial)
            .Where(r => r.Id == id)
            .Select(r => new ResenaDetailDTO(
                r.Usuario.Name,
                r.Usuario.Surname,
                r.Usuario.PhoneNumber,
                r.FechaResena,
                r.ResenaItems.Select(ri => new ResenaItemDTO(
                    ri.Libro.Titulo,
                    ri.Libro.Editorial.Nombre,
                    ri.Libro.TipoLibro.ToString(),
                    ri.Calificacion,
                    ri.Descripcion
                )).ToList()
            ))
            .FirstOrDefaultAsync();

            if (resena == null)
            {
                return NotFound("No se ha encontrado la reseña");
            }
            return Ok(resena);
    }
    
}