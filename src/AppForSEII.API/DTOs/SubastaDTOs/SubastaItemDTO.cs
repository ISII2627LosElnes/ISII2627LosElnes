namespace AppForSEII.API.DTOs.SubastaDTOs;

public class SubastaItemDTO
{
    public SubastaItemDTO(string titulo, string autor, string? descripcion)
    {
        Titulo = titulo;
        Autor = autor;
        Descripcion = descripcion;
    }

    public string Titulo { get; set; }
    public string Autor { get; set; }
    public string? Descripcion { get; set; }

        public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != typeof(SubastaItemDTO))
        {
            return false;
        }

        SubastaItemDTO otro = (SubastaItemDTO)obj;

        return Titulo == otro.Titulo &&
            Autor == otro.Autor &&
            Descripcion == otro.Descripcion;
    }

}