namespace AppForSEII.API.Models;

public abstract class MetodoPago
{
    public MetodoPago(int id)
    {
        Id = id;
    }

    [Key]
    public int Id { get; set; }
}