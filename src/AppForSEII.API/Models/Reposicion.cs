namespace AppForSEII.API.Models;

public class Reposicion
{
    public Reposicion()
    {
    }

    public Reposicion(string id, DateTime fechaReposicion, double precioTotal, string comentario)
    {
        Id = id;
        FechaReposicion = fechaReposicion;
        PrecioTotal = precioTotal;
        Comentario = comentario;
    }

    [Key]
    public string Id { get; set; }

    public DateTime FechaReposicion { get; set; }

    public double PrecioTotal { get; set; }

    [StringLength(100, MinimumLength = 20, ErrorMessage = "El comentario debe tener entre 20 y 100 caracteres.")]
    public string? Comentario { get; set; }
}