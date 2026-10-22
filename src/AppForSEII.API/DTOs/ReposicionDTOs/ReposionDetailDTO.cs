namespace AppForSEII.API.ReposicionDTOs;

public class ReposicionDetailDTO
{
    public ReposicionDetailDTO(string nombre, string apellido, string telefono, DateTime fechaReposicion, double precioTotal, IList<ReposicionItemDTO> reposicionItems, String metodoPago)
    {
        Nombre = nombre;
        Apellido = apellido;
        Telefono = telefono;
        FechaReposicion = fechaReposicion;
        PrecioTotal = precioTotal;
        ReposicionItems = reposicionItems;
        MetodoPago = metodoPago;
    }

    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Telefono { get; set; }
    public DateTime FechaReposicion { get; set; }
    public double PrecioTotal { get; set; }
    public IList<ReposicionItemDTO> ReposicionItems { get; set; }
    public String MetodoPago { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != typeof(ReposicionDetailDTO))
        {
            return false;
        }

        ReposicionDetailDTO otro = (ReposicionDetailDTO)obj;

        return Nombre == otro.Nombre &&
               Apellido == otro.Apellido &&
               Telefono == otro.Telefono &&
               FechaReposicion == otro.FechaReposicion &&
               PrecioTotal == otro.PrecioTotal &&
               ReposicionItems.SequenceEqual(otro.ReposicionItems);
    }
}