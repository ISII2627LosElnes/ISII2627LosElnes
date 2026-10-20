namespace AppForSEII.API.ResenaDTOs;

public class ResenaItemDTO
{
    public ResenaItemDTO(string titulo, string editorial, string tipoLibro, int calificacion, string? descripcion)
    {
        Titulo = titulo;
        Editorial = editorial;
        TipoLibro = tipoLibro;
        Calificacion = calificacion;
        Descripcion = descripcion;
    }

    public string Titulo { get; set; }
    public string Editorial { get; set; }
    public string TipoLibro { get; set; }
    public int Calificacion { get; set; }
    public string? Descripcion { get; set; }

        public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != typeof(ResenaItemDTO))
        {
            return false;
        }

        ResenaItemDTO otro = (ResenaItemDTO)obj;

        return Titulo == otro.Titulo &&
            Editorial == otro.Editorial &&
            TipoLibro == otro.TipoLibro &&
            Calificacion == otro.Calificacion &&
            Descripcion == otro.Descripcion;
    }

}