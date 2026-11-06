using AppForSEII.API.Data;
using AppForSEII.API.DTOs.SubastaDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
                    si.Descripcion,
                    si.PrecioPuja,
                    si.CantidadSubasta
                )).ToList()
            ))
            .FirstOrDefaultAsync();

            if (subasta == null)
            {
                return NotFound("No se ha encontrado la subasta");
            }
            return Ok(subasta);
    }



    [HttpPost]
        [Route("[action]")]
        [ProducesResponseType(typeof(SubastaDetailDTO), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Conflict)]
        public async Task<ActionResult> CreateSubasta(SubastaForCreateDTO subastaForCreate) {
            //validación de datos recibidos (flujos alternativas)
            //comprobación de datos obligatorios de inicio sesión6y
            //comprobación de libros seleccionados, si existen o si se han seleccionado
            if (subastaForCreate.SubastaItems.Count == 0)
                ModelState.AddModelError("SubastaItems", "Error! Debes seleccionar al menos un libro para inciar la puja");

            //comprobación de precio puja válido
            /*if (subastaForCreate.PrecioTotal < 0)
                ModelState.AddModelError("SubastaItems", "Error! Debes poner una puja mayor a 0");*/

            var metodoPago = await _context.MetodoPagos.FindAsync(subastaForCreate.MetodoPago);
            if (metodoPago == null)
                ModelState.AddModelError("MetodoPago", "Error! El método de pago no es válido.");


            // comprobaciones necesarias
            var user = await _context.Users.FirstOrDefaultAsync(s => 
                s.Name == subastaForCreate.Nombre && 
                s.Surname == subastaForCreate.Apellidos);

            //usuario no registrado
            if (user == null)
                ModelState.AddModelError("SubastaApplicationUser", "Error! Usuario no registrado");

            //devolver todos los errores juntos
            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));


            var tituloLibro = subastaForCreate.SubastaItems.Select(si => si.Titulo).ToList();

            var libros = await _context.Libros
            .Where(s => tituloLibro.Contains(s.Titulo)/* && s.Stock > 0*/)
            .ToListAsync();


            //crear Subasta + SubastaItems
            var subasta = new Subasta(0, subastaForCreate.PrecioTotal, DateTime.Today,
            metodoPago, user);


            //procesar item (bloque del else)
            foreach (var item in subastaForCreate.SubastaItems) {
                var libro = libros.FirstOrDefault(l => l.Titulo == item.Titulo);
                //1. comrpobar si el libro buscado por título no se encuentra en la base de datos
                if (libro == null)  {
                    ModelState.AddModelError("SubastaItems", $"Error! Libro titulado '{item.Titulo}' no está disponible para ser subastado");
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }

                /*if (item.Cantidad <= 0) {
                    ModelState.AddModelError("SubastaItems", $"Error! Debes seleccionar al menos 1 unidad.");
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }*/

                // 2. Comprobar si hay stock disponible
                if(libro.Stock <= 0){
                    ModelState.AddModelError("SubastaItem", $"Error! Libro titulado '{item.Titulo}' no está disponible para ser subastado");
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }

                // 3. Comprobar precio de puja
                if(item.PrecioPuja <= 0)
                {
                    ModelState.AddModelError("SubastaItems", $"Error! El precio de la puja para '{item.Titulo}' debe ser mayor de 0");
                    return BadRequest(new ValidationProblemDetails(ModelState));
                }

                // 4. Solo procesar el ítem si cumple con el stock y el precio de puja válidos
                //gracias a la comprobaciones anteriores, lo hacemos con seguridad
                    var subastaItem = new SubastaItem(item.Descripcion, item.PrecioPuja, 
                    libro, subasta, item.Cantidad);
                    subasta.SubastaItems.Add(subastaItem);

                    //recalcular el total acumulado de las pujas
                    subasta.PrecioSubasta = subasta.SubastaItems.Sum(si => si.PrecioPuja);

                    // Actualizamos Stock usando itemDTO.Cantidad unicamente si la validación es correcta
                    libro.Stock -= 1;
            }


            //guardamos en la variable _context para luego meterla a la base de datos
            _context.Subastas.Add(subasta);

            try {
                //insertar en la base de datos
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                _logger.LogError(ex.Message);
                ModelState.AddModelError("Subasta", $"Error! Ha habido un error guardando tu subasta, por favor, intentalo de nuevo");
                return Conflict("Error" + ex.Message);

            }

            //devolver ok + details
            var subastaDetail = new SubastaDetailDTO(subastaForCreate.Nombre,
                subastaForCreate.Apellidos, subastaForCreate.Telefono,
                subasta.FechaSubasta, subasta.PrecioSubasta,
                subastaForCreate.SubastaItems);

            return CreatedAtAction("GetDetailsForSubastas", new { id = subasta.Id }, subastaDetail);
        }
    
}