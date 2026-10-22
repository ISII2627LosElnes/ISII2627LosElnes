using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AppForSEII.API.Data; 
using AppForSEII.API.Models;
using AppForSEII.API.ReposicionDTOs; 

namespace AppForSEII.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReposicionesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ReposicionesController> _logger;

    public ReposicionesController(ApplicationDbContext context, ILogger<ReposicionesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [Route("[action]")]
    [ProducesResponseType(typeof(IList<ReposicionDetailDTO>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetReciboReposicion(string id)
    {
        var reposicion = await _context.Reposiciones
            .Include(r => r.Usuario)
            .Include(r => r.ReposicionItems)
                .ThenInclude(ri => ri.Libro)
            
            .Where(r => r.Id == id)
            .Select(r => new ReposicionDetailDTO(
                r.Usuario.Name,
                r.Usuario.Surname,
                r.Usuario.PhoneNumber,
                r.FechaReposicion,
                r.PrecioTotal,
                r.ReposicionItems.Select(ri => new ReposicionItemDTO(
                    ri.Libro.Titulo,
                    ri.Libro.Autor,
                    ri.CantidadReposicion
                )).ToList(),
                r.MetodoPago is Visa ? "Visa" :
                r.MetodoPago is PayPal ? "PayPal" :
                r.MetodoPago is GooglePay ? "GooglePay" : "Desconocido"
            ))
            .FirstOrDefaultAsync();

        if (reposicion == null)
        {
            return NotFound("No se ha encontrado ninguna reposición asociada al ID proporcionado.");
        }
        
        return Ok(reposicion);
    }
}