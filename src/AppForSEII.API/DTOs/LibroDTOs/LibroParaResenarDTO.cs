using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.LibroDTOs;

public class LibroParaResenarDTO
{
     public LibroParaResenarDTO(int id, string titulo, string tipoLibro, string genero, string editorial, string autor, decimal calificacionMedia, DateTime fechaLanzamiento)
    {
        Id = id;
        Titulo = titulo;
        TipoLibro = tipoLibro;
        Genero = genero;
        Editorial = editorial;
        Autor = autor;
        CalificacionMedia = calificacionMedia;
        FechaLanzamiento = fechaLanzamiento;
    }
    public int Id { get; set; }

    public string Titulo { get; set; }

    [StringLength(50, MinimumLength = 10,
        ErrorMessage = "El tipo de libro debe tener entre 10 y 50 caracteres.")]
    public string TipoLibro { get; set; }

    public string Genero { get; set; }

    public string Editorial { get; set; }

    public string Autor { get; set; }

    public decimal CalificacionMedia { get; set; }

    [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
    public DateTime FechaLanzamiento { get; set; }
}