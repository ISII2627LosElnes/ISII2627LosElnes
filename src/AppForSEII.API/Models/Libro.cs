namespace AppForSEII.API.Models;
public class Libro
{
    public Libro()
    {
    }

   public Libro(
    int id,
    string titulo,
    string tipoLibro,
    string autor,
    decimal precioTotal,
    int stock,
    decimal calificacionMedia,
    DateTime fechaLanzamiento)
{
    Id = id;
    Titulo = titulo;
    TipoLibro = tipoLibro;
    Autor = autor;
    PrecioTotal = precioTotal;
    Stock = stock;
    CalificacionMedia = calificacionMedia;
    FechaLanzamiento = fechaLanzamiento;
}

    public int Id { get; set; }

    public string Titulo { get; set; }

    public string Autor { get; set; }

    public decimal PrecioTotal { get; set; }
    
    [DataType(System.ComponentModel.DataAnnotations.DataType.Currency)]
    public decimal PrecioReposicion { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor que 0")]
    public int Stock { get; set; }

    public Editorial Editorial { get; set; }

    public Genero Genero { get; set; }

    public IList<CompraItem> CompraItems { get; set; } = new List<CompraItem>();
    public IList<ReposicionItem> ReposicionItems { get; set; } = new List<ReposicionItem>();


    [StringLength(50, MinimumLength = 10,
    ErrorMessage = "El tipo de libro debe tener entre 10 y 50 caracteres.")]
    public string TipoLibro { get; set; }

    public decimal CalificacionMedia { get; set; }

    [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
    public DateTime FechaLanzamiento { get; set; }

    public IList<ResenaItem> ResenaItems { get; set; } = new List<ResenaItem>();

    

    
    
}