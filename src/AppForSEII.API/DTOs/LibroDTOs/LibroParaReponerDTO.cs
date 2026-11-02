using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.LibroDTOs;

public class LibroParaReponerDTO
{
    public LibroParaReponerDTO(int id, string titulo, string genero, DateTime fechaLanzamiento,decimal precioReposicion, string autor)
    {
        Id = id;
        Titulo = titulo;
        Genero = genero;
        PrecioReposicion = precioReposicion;
        Autor = autor;
        FechaLanzamiento = fechaLanzamiento;
    }

    public int Id { get; set; }
    public string Titulo { get; set; }
    public string Genero { get; set; }
    
    [DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
    public DateTime FechaLanzamiento { get; set; }
    public decimal PrecioReposicion { get; set; }
    public string Autor { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is LibroParaReponerDTO dTO &&
               Id == dTO.Id &&
               Titulo == dTO.Titulo &&
               Genero == dTO.Genero &&
               FechaLanzamiento == dTO.FechaLanzamiento &&
               PrecioReposicion == dTO.PrecioReposicion &&
               Autor == dTO.Autor;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Titulo, Genero, FechaLanzamiento, PrecioReposicion, Autor);
    }
}