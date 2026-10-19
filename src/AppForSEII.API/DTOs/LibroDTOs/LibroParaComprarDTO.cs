using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.LibroDTOs;

public class LibroParaComprarDTO
{
    public LibroParaComprarDTO(int id, string titulo, decimal precioTotal, DateTime fechaLanzamiento, string genero, string editorial)
    {
        Id = id;
        Titulo = titulo;
        PrecioTotal = precioTotal;
        FechaLanzamiento = fechaLanzamiento;
        Genero = genero;
        Editorial = editorial;
    }

    public int Id { get; set; }

    public string Titulo { get; set; }

    public decimal PrecioTotal { get; set; }

    [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
    public DateTime FechaLanzamiento { get; set; }

    public string Genero { get; set; }

    public string Editorial { get; set; }

    public bool Equals(LibroParaComprarDTO? other)
    {
        if (other == null) return false;
        return Id == other.Id &&
               Titulo == other.Titulo &&
               PrecioTotal == other.PrecioTotal &&
               FechaLanzamiento == other.FechaLanzamiento &&
               Genero == other.Genero &&
               Editorial == other.Editorial;
    }

 
}