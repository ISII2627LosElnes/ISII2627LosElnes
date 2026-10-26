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

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(typeof(ComprasDetailDTO), (int)HttpStatusCode.Created)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> CreateCompra([FromBody] CompraForCreateDTO compraForCreate)
    {
        // Validar los datos recibidos 
        if (!ModelState.IsValid)
            return BadRequest(ModelState); 

        if (compraForCreate.LibrosSeleccionados == null || compraForCreate.LibrosSeleccionados.Count == 0)
            return BadRequest("Debe seleccionar al menos un libro.");

        // Comprobaciones previas: validar método de pago y localizar al cliente
        var metodoPago = await _context.MetodoPagos.FindAsync(compraForCreate.MetodoPagoId);
        if (metodoPago == null)
            return BadRequest("El método de pago no es válido.");

        var cliente = await _userManager.FindByEmailAsync("peter@uclm.es");
        if (cliente == null) 
            return NotFound("Usuario no encontrado.");

        // Actualizar el perfil del cliente con los datos del formulario
        cliente.Name = compraForCreate.Nombre;
        cliente.Surname = compraForCreate.Apellidos;
        cliente.Address = compraForCreate.Direccion; 
        cliente.PhoneNumber = compraForCreate.Telefono;
        await _userManager.UpdateAsync(cliente);

        // Crear la Compra 
        var nuevaCompra = new Compra
        {
            FechaCompra = DateTime.Now,
            CodigoDescuento = compraForCreate.CodigoDescuento,
            Usuario = cliente,       
            MetodoPago = metodoPago, 
            PrecioTotal = 0m
        };

        var itemsDTO = new List<CompraItemDTO>();

        // Crear los CompraItems y actualizar el stock 
        foreach (var item in compraForCreate.LibrosSeleccionados)
        {
            var libro = await _context.Libros
                .Include(l => l.Genero)
                .Include(l => l.Editorial)
                .FirstOrDefaultAsync(l => l.Id == item.LibroId);

            if (libro == null) 
                return NotFound($"El libro {item.LibroId} no existe.");

            if (libro.Stock < item.Cantidad)
                return BadRequest($"No hay stock suficiente para '{libro.Titulo}'.");

            // Descontar el stock y acumular el precio total
            libro.Stock -= item.Cantidad;
            nuevaCompra.PrecioTotal += (libro.PrecioCompra * item.Cantidad);

            nuevaCompra.CompraItems.Add(new CompraItem
            {
                Cantidad = item.Cantidad,
                LibroId = libro.Id,
                Compra = nuevaCompra
            });

            // Preparar el DTO de respuesta visual sobre la marcha
            itemsDTO.Add(new CompraItemDTO(
                libro.Titulo,
                libro.Genero.Nombre,
                libro.Editorial.Nombre,
                item.Cantidad
            ));
        }

        // Insertar en la Base de Datos
        _context.Compras.Add(nuevaCompra);
        await _context.SaveChangesAsync();

        // Devolver OK + Details 
        var compraDetail = new ComprasDetailDTO(
            cliente.Name,
            cliente.Surname,
            cliente.Address, 
            cliente.PhoneNumber,
            nuevaCompra.FechaCompra,
            nuevaCompra.PrecioTotal,
            nuevaCompra.CodigoDescuento, 
            itemsDTO,                    
            metodoPago.GetType().Name    
        );

        return CreatedAtAction(nameof(GetDetailsForCompras), new { id = nuevaCompra.Id }, compraDetail);
    }

   
       
}