using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.LibroDTOs;

public class LibroParaComprarDTO
{
    public LibroParaComprarDTO(int id, string titulo, decimal precioCompra, DateTime fechaLanzamiento, string genero, string editorial)
    {
        Id = id;
        Titulo = titulo;
        PrecioCompra = precioCompra;
        FechaLanzamiento = fechaLanzamiento;
        Genero = genero;
        Editorial = editorial;
    }

    public int Id { get; set; }
    public string Titulo { get; set; }
    public decimal PrecioCompra { get; set; }

    [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
    public DateTime FechaLanzamiento { get; set; }

    public string Genero { get; set; }
    public string Editorial { get; set; }

   
    public override bool Equals(object? obj)
    {
        if (obj is not LibroParaComprarDTO other)
            return false;

        return Id == other.Id &&
               Titulo == other.Titulo &&
               PrecioCompra == other.PrecioCompra &&
               FechaLanzamiento == other.FechaLanzamiento &&
               Genero == other.Genero &&
               Editorial == other.Editorial;
    }

    
    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Titulo, PrecioCompra, FechaLanzamiento, Genero, Editorial);
    }
}