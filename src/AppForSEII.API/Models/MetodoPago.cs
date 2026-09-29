namespace AppForSEII.API.Models;

public class MetodoPago
{
    public MetodoPago(string id)
    {
        Id = id;
    }

    [Key]
    public string Id { get; set; }
}