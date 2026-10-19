using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.LibroDTOs;

public class LibroParaSubastarDTO
{
     public LibroParaSubastarDTO(int id, string titulo, string editorial, string autor, string tipoLibro)
    {
        Id = id;
        Titulo = titulo;
        Editorial = editorial;
        Autor = autor;
        TipoLibro = tipoLibro;
    }
    public int Id { get; set; }

    public string Titulo { get; set; }
    public string Editorial { get; set; }
    public string Autor { get; set; }
    public string TipoLibro { get; set; }

    public override bool Equals(object? obj)
    {
        return obj is LibroParaSubastarDTO other &&
               Id == other.Id &&
               Titulo == other.Titulo &&
               Editorial == other.Editorial &&
               Autor == other.Autor &&
               TipoLibro == other.TipoLibro;
    }
}