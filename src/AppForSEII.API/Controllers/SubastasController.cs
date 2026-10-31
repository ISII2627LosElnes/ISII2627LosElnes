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
            if (subastaForCreate.SubastaItems == null || subastaForCreate.SubastaItems.Count == 0)
                ModelState.AddModelError("SubastaItems", "Error! Debes seleccionar al menos un libro para inciar la puja");

            if (subastaForCreate.PrecioTotal < 0)
                ModelState.AddModelError("SubastaItems", "Error! Debes seleccionar al menos un libro para inciar la puja");

            if (subastaForCreate.Nombre.IsNullOrEmpty())
                ModelState.AddModelError("Nombre", "Error! Debes rellenar tu nombre");

            if (subastaForCreate.Apellidos.IsNullOrEmpty())
                ModelState.AddModelError("Apellidos", "Error! Debes rellenar tus apellidos");

            if (subastaForCreate.Telefono.IsNullOrEmpty())
                ModelState.AddModelError("Telefono", "Error! Debes rellenar tu teléfono");

            var metodoPago = await _context.MetodoPagos.FindAsync(subastaForCreate.MetodoPago);
            if (metodoPago == null)
                return BadRequest("El método de pago no es válido.");


            // comprobaciones necesarias
            var user = await _context.Users.FirstOrDefaultAsync(s => 
                s.Name == subastaForCreate.Nombre && 
                s.Surname == subastaForCreate.Apellidos);

            if (user == null)
                ModelState.AddModelError("SubastaApplicationUser", "Error! Usuario no registrado");

            if (ModelState.ErrorCount > 0)
                return BadRequest(new ValidationProblemDetails(ModelState));


            var tituloLibro = subastaForCreate.SubastaItems.Select(si => si.Titulo).ToList();

            /*var libros = _context.Libros.Include(s => s.SubastaItems)
                .ThenInclude(si => si.Subasta)
                .Where(s => tituloLibro.Contains(s.Titulo) &&

                    //debemos comprobar que haya stock en la base de datos
                    s.SubastaItems.Count < s.Stock)
                .ToList();*/

            var libros = await _context.Libros
            .Where(s => tituloLibro.Contains(s.Titulo) && s.Stock > 0)
            .ToListAsync();


            //crear Subasta + SubastaItems
            var subasta = new Subasta(0, subastaForCreate.PrecioTotal, DateTime.Now,
            metodoPago, user);


            //procesar item (bloque del else)
            foreach (var item in subastaForCreate.SubastaItems) {
                var libro = libros.FirstOrDefault(l => l.Titulo == item.Titulo);
                //1. comrpobar si el libro buscado por título no se encuentra en la base de datos
                if (libro == null || libro.Stock <= 0)  {
                    ModelState.AddModelError("SubastaItems", $"Error! Libro titulado '{item.Titulo}' no está disponible para ser subastado");
                }
                else {
                    // 2. Comprobar si hay suficiente stock disponible
                    if(item.Cantidad > libro.Stock){
                        ModelState.AddModelError("SubastaItems", $"Error! No hay suficiente stock para el libro '{item.Titulo}'. Solicitados: {item.Cantidad}, Disponibles: {libro.Stock}.");
                    }

                    // 3. Comprobar precio de puja
                    if(item.PrecioPuja <= 0)
                    {
                        ModelState.AddModelError("SubastaItems", $"Error! El precio de la puja para {item.Titulo} debe ser mayor de 0");
                    }

                    // 4. Solo procesar el ítem si cumple con el stock y el precio de puja válidos
                    if(item.Cantidad <= libro.Stock && item.PrecioPuja > 0)
                    {
                        var subastaItem = new SubastaItem(item.Descripcion, item.PrecioPuja, 
                        libro, subasta, item.Cantidad);
                        subasta.SubastaItems.Add(subastaItem);

                        // Actualizamos Stock usando itemDTO.Cantidad unicamente si la validación es correcta
                        libro.Stock -= item.Cantidad;
                    }
                } 
            }

            //recalcular el total acumulado de las pujas
            subasta.PrecioSubasta = subasta.SubastaItems.Sum(si => si.PrecioPuja);

            //if there is any problem because of the available quantity of movies or because the movie does not exist
            if (ModelState.ErrorCount > 0) {
                return BadRequest(new ValidationProblemDetails(ModelState));
            }

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