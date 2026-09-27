namespace AppForSEII.API.Models;

public class Subasta
{
    // Constructor con parámetros
    public Subasta(String id, double precioSubasta, DateTime fechaSubasta)
    {
        Id = id;
        PrecioSubasta = precioSubasta;
        FechaSubasta = fechaSubasta;
    }
    

    // Constructor vacío
    public Subasta()
    {
    }


    [Key]
    public String Id { get; set; }

    public double PrecioSubasta { get; set; }

    public DateTime FechaSubasta { get; set; }
}