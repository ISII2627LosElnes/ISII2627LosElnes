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
using System.Security.Claims;

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
    public async Task<IActionResult> GetDetailsForReposiciones(int id)
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

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(typeof(ReposicionDetailDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> CreateReposicion([FromBody] ReposicionForCreateDTO reposicionDTO)
    {
        if (reposicionDTO.ReposicionItems == null || !reposicionDTO.ReposicionItems.Any())
        {
            ModelState.AddModelError("ReposicionItems", "El carrito de reposición no puede estar vacío.");
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        var usuario = await _context.Users.FirstOrDefaultAsync(u =>
                u.Name == reposicionDTO.Nombre &&
                u.Surname == reposicionDTO.Apellido);

        if (usuario == null)
        {
            ModelState.AddModelError("Usuario", $"No existe el usuario con el nombre {reposicionDTO.Nombre} y apellido {reposicionDTO.Apellido} introducido en la base de datos");
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        var metodoPagoEncontrar = await _context.MetodoPagos.FirstOrDefaultAsync(u =>
                u.Id == reposicionDTO.MetodoPago);

        if (metodoPagoEncontrar == null)
        {
            ModelState.AddModelError("MetodoPago", "El método de pago no se encuentra en la base de datos");
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        //Cojo el método de pago para que el details no muestre el id sino el tipo de método de pago 
        string metodoPagoString = metodoPagoEncontrar.GetType().Name;
        Console.WriteLine(metodoPagoString);

        // Usamos DateTime.Today para que coincida con la fecha del test sin fallar por milisegundos
        Reposicion nuevaReposicion = new Reposicion(0, DateTime.Today, 0, reposicionDTO.Comentario, new List<ReposicionItem>(), metodoPagoEncontrar, usuario);

        double precioTotalCalculado = 0;
        var detallesItemsParaDetails = new List<ReposicionItemDTO>();
        var titulosLibros = reposicionDTO.ReposicionItems.Select(ri => ri.Titulo).ToList();
        var libros = await _context.Libros.Where(l => titulosLibros.Contains(l.Titulo)).ToListAsync();

        foreach (var itemDTO in reposicionDTO.ReposicionItems)
        {
            var libro = libros.FirstOrDefault(l => l.Titulo == itemDTO.Titulo);

            if (libro == null)
            {
                ModelState.AddModelError("ReposicionItems", $"El libro con título '{itemDTO.Titulo}' no existe en el catálogo.");
                continue;
            }

            if (libro.Stock > 0)
            {
                ModelState.AddModelError("ReposicionItems", $"No se puede reponer el libro '{libro.Titulo}' porque actualmente hay stock disponible ({libro.Stock} unidades). Solo se pueden reponer libros con stock = 0.");
                continue;
            }

            var reposicionItem = new ReposicionItem(itemDTO.Cantidad, libro, nuevaReposicion);
            nuevaReposicion.ReposicionItems.Add(reposicionItem);
            precioTotalCalculado += (double)libro.PrecioReposicion * itemDTO.Cantidad;
            nuevaReposicion.PrecioTotal = precioTotalCalculado;
            libro.Stock += itemDTO.Cantidad;
            detallesItemsParaDetails.Add(new ReposicionItemDTO(libro.Titulo, libro.Autor, itemDTO.Cantidad));
        }

        if (ModelState.ErrorCount > 0)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        try
        {
            _context.Reposiciones.Add(nuevaReposicion);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar la reposición en la base de datos.");
            return Conflict("Error al procesar la reposición en la base de datos: " + ex.Message);
        }

        var reposicionDetail = new ReposicionDetailDTO(
            reposicionDTO.Nombre,
            reposicionDTO.Apellido,
            reposicionDTO.Telefono,
            nuevaReposicion.FechaReposicion,
            nuevaReposicion.PrecioTotal,
            detallesItemsParaDetails,
            metodoPagoString
        );

        return CreatedAtAction(nameof(GetDetailsForReposiciones), new { id = nuevaReposicion.Id }, reposicionDetail);
    }
}