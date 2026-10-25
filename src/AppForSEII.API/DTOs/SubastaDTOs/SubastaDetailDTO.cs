namespace AppForSEII.API.DTOs.SubastaDTOs;

public class SubastaDetailDTO
{
    public SubastaDetailDTO(string nombre, string apellidos, string telefono, DateTime fechaSubasta, double precioSubasta,IList<SubastaItemDTO> subastas)
    {
        Nombre = nombre;
        Apellidos = apellidos;
        Telefono = telefono;
        FechaSubasta = fechaSubasta;
        PrecioSubasta = precioSubasta;
        SubastaItems = subastas;
    }

    public string Nombre { get; set; }
    public string Apellidos { get; set; }
    public string Telefono { get; set; }
    public DateTime FechaSubasta { get; set; }
    public double PrecioSubasta { get; set; }
    public IList<SubastaItemDTO> SubastaItems { get; set; }

    public override bool Equals(object? obj)
{
    if (obj == null || obj.GetType() != typeof(SubastaDetailDTO))
    {
        return false;
    }

    SubastaDetailDTO otro = (SubastaDetailDTO)obj;

    return Nombre == otro.Nombre &&
           Apellidos == otro.Apellidos &&
           Telefono == otro.Telefono &&
           FechaSubasta == otro.FechaSubasta &&
           PrecioSubasta == otro.PrecioSubasta &&
           SubastaItems.SequenceEqual(otro.SubastaItems);
}
}