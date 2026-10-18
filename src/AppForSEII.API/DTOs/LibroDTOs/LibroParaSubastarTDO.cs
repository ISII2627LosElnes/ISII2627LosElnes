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
}