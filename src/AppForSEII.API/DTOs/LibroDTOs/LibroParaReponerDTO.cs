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
    public DateTime FechaLanzamiento { get; set; }
    public decimal PrecioReposicion { get; set; }
    public string Autor { get; set; }
}