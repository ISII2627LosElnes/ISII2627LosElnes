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

    [HttpPost]
    [Route("[action]")]
    [ProducesResponseType(typeof(ResenaDetailDTO), (int)HttpStatusCode.Created)]//devuelve el OK 201 + el detalle porque al hacer el post te enseña lo que has hecho
    [ProducesResponseType(typeof(string),(int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(string),(int)HttpStatusCode.Conflict)]
    public async Task<ActionResult> CreateResena(ResenaForCreateDTO resenaForCreate)
    {
        //primer bloque: validaciones de los flujos alternativos del archivo de CU
            //flujo alternativo 1-4 (comprobar que se haya seleccionado al menos un libro)
        if (resenaForCreate.ResenaItems == null || resenaForCreate.ResenaItems.Count == 0)
        {
            ModelState.AddModelError("ResenaItems", "Debes seleccionar al menos un libro.");
        }
            //comprobar que el usuario esté registrado (precondición que sale abajo en el archivo)
            var user = _context.ApplicationUsers.FirstOrDefault(au => au.Name == resenaForCreate.Nombre && au.Surname == resenaForCreate.Apellidos);
            if (user == null)
                ModelState.AddModelError("ResenaApplicationUser", "¡Error! El usuario no está registrado");
            //devolver los errores todos juntos
        if (ModelState.ErrorCount > 0)
        {
            return BadRequest(new ValidationProblemDetails(ModelState));
        }

        //actualizar los datos del usuario 
        user.PhoneNumber = resenaForCreate.Telefono;
        user.Address = resenaForCreate.Direccion;
        //segundo bloque: comprobar disponibilidad de los libros (no hace falta en reseñas)
       
        //tercer bloque:crear el objeto reseña con todos lo que nos ha dado el parámetro de entrada, tambien hay que hacer los items
            //primero la resena
        Resena resena = new Resena(0, DateTime.Now, resenaForCreate.TituloResena);//pongo 0 porque es obligatorio ponerlo, pero luego se genera bien en la BDD
        resena.Usuario = user;
            //despues todos los items
            foreach (var item in resenaForCreate.ResenaItems)
            {
                //buscamos el libro por el titulo 
                var libro = _context.Libros
                    .FirstOrDefault(l => l.Titulo == item.Titulo);

                if (libro != null)
                {
                    libro.CalificacionMedia=(libro.CalificacionMedia + item.Calificacion)/2;
                    var resenaItem = new ResenaItem(libro, item.Descripcion, item.Calificacion,resena);

                    resena.ResenaItems.Add(resenaItem);
                }
            }   

        //cuarto bloque: inserción en labase de datos, a partir del context. 
        _context.Resenas.Add(resena);
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar la reseña");
            return Conflict("¡ERROR! No se ha podido guardar la reseña.");
        }
        //quinto bloque: crear el details y devolverlo con el OK
        //primero hacemos una lista con cada resenaitem creado
        var resenaDetail = new ResenaDetailDTO(
        user.Name,
        user.Surname,
        user.PhoneNumber,
        resena.FechaResena,
        resenaForCreate.ResenaItems
        );
        //devolver la reseña creada
        return CreatedAtAction("GetDetailsForResenas", new {id=resena.Id}, resenaDetail);


    }



    
    
}