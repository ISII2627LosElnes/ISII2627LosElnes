namespace AppForSEII.API.ResenaDTOs;

public class ResenaDetailDTO
{
    public ResenaDetailDTO(string nombre, string apellidos, string telefono, DateTime fechaResena, IList<ResenaItemDTO> resenas)
    {
        Nombre = nombre;
        Apellidos = apellidos;
        Telefono = telefono;
        FechaResena = fechaResena;
        ResenaItems = resenas;
    }

    public string Nombre { get; set; }
    public string Apellidos { get; set; }
    public string Telefono { get; set; }
    public DateTime FechaResena { get; set; }
    public IList<ResenaItemDTO> ResenaItems { get; set; }

    public override bool Equals(object? obj)
{
    if (obj == null || obj.GetType() != typeof(ResenaDetailDTO))
    {
        return false;
    }

    ResenaDetailDTO otro = (ResenaDetailDTO)obj;

    return Nombre == otro.Nombre &&
           Apellidos == otro.Apellidos &&
           Telefono == otro.Telefono &&
           FechaResena == otro.FechaResena &&
           ResenaItems.SequenceEqual(otro.ResenaItems);
}
}