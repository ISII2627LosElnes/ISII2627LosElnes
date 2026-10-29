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
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> CreateReposicion([FromBody] ReposicionForCreateDTO reposicionDTO)
    {
        //Aquí comienzo haciendo las primeras validaciones.
        if (!ModelState.IsValid)
        {
            _logger.LogError($"{DateTime.Now} Error de validación: faltan datos obligatorios según el Flujo Alternativo 4.");
            return BadRequest(new ValidationProblemDetails(ModelState));
        }
        if (reposicionDTO.ReposicionItems == null || !reposicionDTO.ReposicionItems.Any())
        {
            return BadRequest("El carrito de reposición no puede estar vacío.");
        }

        //Aquí compruebo que el usuario existe y lo guardo para usarlo posteriormente.
        var usuario = await _context.Users.FirstOrDefaultAsync(u => 
                u.Name == reposicionDTO.Nombre && 
                u.Surname == reposicionDTO.Apellido);

        if (usuario == null)
        {
            return BadRequest($"No existe el usuario con el nombre {reposicionDTO.Nombre} y apellido {reposicionDTO.Apellido} introducido en la base de datos");
        }

        //Aquí compruebo que el método de pago es el correcto y lo asigno al método de pago final.
        var metodoPagoEncontrar = await _context.MetodoPagos.FirstOrDefaultAsync(u => 
                u.Id == reposicionDTO.MetodoPago);
        
        if(metodoPagoEncontrar == null)
        {
            return BadRequest("El método de pago no se encuentra en la base de datos");
        }

        string metodoPagoString = metodoPagoEncontrar is Visa ? "Visa" :
                                  metodoPagoEncontrar is PayPal ? "PayPal" :
                                  metodoPagoEncontrar is GooglePay ? "GooglePay" : "Desconocido";
        
        //Aquí creo la reposición
        var nuevaReposicion = new Reposicion
        {
            FechaReposicion = DateTime.Now,
            PrecioTotal = 0, // Se calcula posteriormente
            Comentario = reposicionDTO.Comentario, 
            Usuario = usuario,
            ReposicionItems = new List<ReposicionItem>(),
            MetodoPago = metodoPagoEncontrar
        };

        //Aquí compruebo los libros y cambio el precio total.
        double precioTotalCalculado = 0;
        var detallesItemsParaDetails = new List<ReposicionItemDTO>();
        // Extraemos los títulos de los libros del carrito en lugar de los IDs
        var titulosLibros = reposicionDTO.ReposicionItems.Select(ri => ri.Titulo).ToList();
        
        // Buscamos en la base de datos comparando por el Título
        var libros = await _context.Libros.Where(l => titulosLibros.Contains(l.Titulo)).ToListAsync();

        foreach (var itemDTO in reposicionDTO.ReposicionItems)
        {
            // Enlazamos usando itemDTO.Titulo
            var libro = libros.FirstOrDefault(l => l.Titulo == itemDTO.Titulo);
            
            if (libro == null)
            {
                ModelState.AddModelError("ReposicionItems", $"El libro con título '{itemDTO.Titulo}' no existe en el catálogo.");
                continue;
            }
            // Generamos el item usando itemDTO.Cantidad (como se llama en tu DTO)
            var reposicionItem = new ReposicionItem(itemDTO.Cantidad, libro, nuevaReposicion);
            nuevaReposicion.ReposicionItems.Add(reposicionItem);
            // Cálculo seguro con datos de BDD usando itemDTO.Cantidad
            precioTotalCalculado += (double)libro.PrecioReposicion * itemDTO.Cantidad;
            nuevaReposicion.PrecioTotal = precioTotalCalculado;
            
            // Actualizamos Stock usando itemDTO.Cantidad
            libro.Stock += itemDTO.Cantidad;
            // Preparamos DTO de salida usando itemDTO.Cantidad
            detallesItemsParaDetails.Add(new ReposicionItemDTO(libro.Titulo, libro.Autor, itemDTO.Cantidad));
        }
        if (ModelState.ErrorCount > 0) 
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        
        // 6. Insertar en la Base de Datos
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
        // 7. Devolver el DetailsFor (ReposicionDetailDTO)
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