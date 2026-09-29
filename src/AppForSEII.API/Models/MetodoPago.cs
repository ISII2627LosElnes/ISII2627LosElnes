namespace AppForSEII.API.Models;

public abstract class MetodoPago
{
    public MetodoPago(string id)
    {
        Id = id;
    }

    [Key]
    public string Id { get; set; }
}