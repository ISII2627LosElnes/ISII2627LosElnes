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
    public async Task<IActionResult> GetReciboReposicion(int id)
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
    //public async Task<IActionResult> CreateReposicion(ReposicionForCreateDTO reposicionCreateDTO)
    //{
    //    //Comienzo con la validación de los datos recibidos
    //    if (reposicionCreateDTO.ReposicionItems == null || reposicionCreateDTO.ReposicionItems.Count() < 1)
    //    {
    //        ModelState.AddModelError("Cantidad de reposición", "Error! Debes tener al menos un elemento seleccionado");
    //    }
//
    //    if (reposicionCreateDTO.MetodoPago.ToString().Equals("Visa") || reposicionCreateDTO.MetodoPago.ToString().Equals("GooglePay") || reposicionCreateDTO.MetodoPago.ToString().Equals("PayPal"))
    //    {
    //        ModelState.AddModelError("Cantidad de reposición", "Error! El método de pago no se encuentra entre los permitidos");
    //    }
//
    //    var user = _context.ApplicationUsers.FirstOrDefault(r => r.UserName == reposicionCreateDTO.Nombre);
    //        if (user == null)
    //            ModelState.AddModelError("Identidad del usuario", "Error! Usuario no registrado");
//
    //        if (ModelState.ErrorCount > 0)
    //            return BadRequest(new ValidationProblemDetails(ModelState));
    //    
    //    
    //}

    



    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(typeof(ReposicionDetailDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> CreateReposicion([FromBody] ReposicionForCreateDTO reposicionDTO)
    {
        // 1. Validación de los parámetros de entrada (Flujos alternativos)
        if (!ModelState.IsValid)
        {
            _logger.LogError($"{DateTime.Now} Error de validación al recibir el DTO de creación de reposición.");
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        if (reposicionDTO.ReposicionItems == null || !reposicionDTO.ReposicionItems.Any())
        {
            return BadRequest("El carrito de reposición no puede estar vacío.");
        }

        // 2. Extraer el usuario conectado obligatoriamente del Token
        var usuarioIdActual = User.FindFirstValue(ClaimTypes.NameIdentifier);[cite: 3]
        if (string.IsNullOrEmpty(usuarioIdActual))
        {
            return Unauthorized("Debe estar autenticado para realizar una reposición.");
        }
        var usuario = await _context.Users.FindAsync(usuarioIdActual);

        // 3. Crear la entidad principal Reposicion para la base de datos[cite: 3]
        var nuevaReposicion = new Reposicion
        {
            Id = $"REP-{Guid.NewGuid().ToString().Substring(0, 5).ToUpper()}",
            FechaReposicion = DateTime.Now,
            Comentario = reposicionDTO.Comentario,
            Usuario = usuario 
        };

        // 4. Instanciar la clase hija del Método de Pago basándonos en el string recibido[cite: 3]
        MetodoPago metodoPagoObj;
        string metodoPagoString = ""; 

        switch (reposicionDTO.MetodoPago.ToUpper())
        {
            case "VISA":
                metodoPagoObj = new Visa($"PAGO-{nuevaReposicion.Id}", "0000000000000000", DateTime.Now.AddYears(2));
                metodoPagoString = "Visa";
                break;
            case "GOOGLEPAY":
                metodoPagoObj = new GooglePay($"PAGO-{nuevaReposicion.Id}", usuario.Email ?? "email@test.com");
                metodoPagoString = "GooglePay";
                break;
            case "PAYPAL":
                metodoPagoObj = new PayPal($"PAGO-{nuevaReposicion.Id}", reposicionDTO.Telefono);
                metodoPagoString = "PayPal";
                break;
            default:
                return BadRequest("El método de pago proporcionado no es válido.");
        }
        nuevaReposicion.MetodoPago = metodoPagoObj;

        // 5. Comprobaciones de libros, cálculo de precio seguro y creación de Items
        double precioTotalCalculado = 0;
        var detallesItemsParaRecibo = new List<ReposicionItemDTO>();

        foreach (var itemDTO in reposicionDTO.ReposicionItems)
        {
            var libro = await _context.Libros.FindAsync(itemDTO.LibroId);
            
            if (libro == null)
            {
                ModelState.AddModelError("ReposicionItems", $"El libro con identificador {itemDTO.LibroId} no existe.");
                continue;
            }

            // Crear la relación ReposicionItem en la base de datos[cite: 3]
            var reposicionItem = new ReposicionItem(itemDTO.CantidadReposicion, libro, nuevaReposicion);
            nuevaReposicion.ReposicionItems.Add(reposicionItem);

            // Calcular el coste total multiplicando por el PrecioReposicion de la base de datos de forma segura[cite: 3]
            precioTotalCalculado += libro.PrecioReposicion * itemDTO.CantidadReposicion;
            
            // Actualizar el stock del libro sumando la cantidad repuesta[cite: 3]
            libro.Stock += itemDTO.CantidadReposicion;

            // Preparamos el DTO de salida para el recibo que devolveremos al final
            detallesItemsParaRecibo.Add(new ReposicionItemDTO(libro.Titulo, libro.Autor, itemDTO.CantidadReposicion));
        }

        // Si falló alguna búsqueda de libros, devolvemos error antes de guardar nada
        if (ModelState.ErrorCount > 0) 
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        // Asignamos el precio total definitivo a la entidad
        nuevaReposicion.PrecioTotal = precioTotalCalculado;

        // 6. Insertar en la base de datos[cite: 3]
        try
        {
            _context.Reposiciones.Add(nuevaReposicion);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            return Conflict("Error al procesar la reposición en la base de datos: " + ex.Message);
        }

        // 7. Devolver OK + details construyendo tu ReposicionDetailDTO
        // Nota: Como tu DTO de detalle pide un único string 'apellido', concatenamos el primer y segundo apellido recibidos.
        string apellidosConcatenados = $"{reposicionDTO.Apellido}".Trim();

        var reposicionDetail = new ReposicionDetailDTO(
            reposicionDTO.Nombre,
            apellidosConcatenados,
            reposicionDTO.Telefono,
            nuevaReposicion.FechaReposicion,
            nuevaReposicion.PrecioTotal,
            detallesItemsParaRecibo,
            metodoPagoString 
        );

        // Retorna un código 201 Created y el recibo en el cuerpo de la respuesta[cite: 3]
        return CreatedAtAction(nameof(GetReciboReposicion), new { id = nuevaReposicion.Id }, reposicionDetail);
    }
}