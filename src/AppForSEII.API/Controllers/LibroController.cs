using AppForSEII.API.LibroDTOs;
namespace AppForSEII.API.Controllers

{
    [Route("api/[controller]")]
    [ApiController]
    public class LibroController : ControllerBase 
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LibroController> _logger;

        public LibroController(ApplicationDbContext context, ILogger<LibroController> logger)
        {
            _context = context;
            _logger=logger;
        }

         [HttpGet]
[Route("[action]")]
[ProducesResponseType(typeof(IList<LibroParaResenarDTO>), (int)HttpStatusCode.OK)]
[ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
public async Task<IActionResult> GetLibrosParaResenar(string? tipoLibro, string? genero)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    var libros = await _context.Libros
    .Include(libro => libro.Genero)
    .Include(libro => libro.Editorial)
    .Where(libro => (tipoLibro == null || libro.TipoLibro.Equals(tipoLibro)) &&
                    (genero == null || libro.Genero.Nombre.Equals(genero)))
    .Select(libro => new LibroParaResenarDTO(
        libro.Id,
        libro.Titulo,
        libro.TipoLibro,
        libro.Genero.Nombre,
        libro.Editorial.Nombre,
        libro.Autor,
        libro.CalificacionMedia,
        libro.FechaLanzamiento))
    .ToListAsync();

    return Ok(libros);
}

[HttpGet]
[Route("[action]")]
[ProducesResponseType(typeof(IList<LibroParaComprarDTO>), (int)HttpStatusCode.OK)]
[ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
public async Task<IActionResult> GetLibrosParaComprar(string? genero, decimal? precio)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(ModelState);
    }

    var libros = await _context.Libros
        .Include(libro => libro.Genero)
        .Include(libro => libro.Editorial)
        .Where(libro => libro.Stock > 0 && 
                        (genero == null || libro.Genero.Nombre.Equals(genero)) &&
                        (precio == null || libro.PrecioCompra <= precio))
        .Select(libro => new LibroParaComprarDTO(
            libro.Id,
            libro.Titulo,
            libro.PrecioCompra,
            libro.FechaLanzamiento,
            libro.Genero.Nombre,
            libro.Editorial.Nombre))
        .ToListAsync();

    return Ok(libros);
}
    
    }


    






}