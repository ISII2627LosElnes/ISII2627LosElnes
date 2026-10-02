namespace AppForSEII.API.Models;

public class Reposicion
{
    public Reposicion()
    {
    }

    public Reposicion(string id, DateTime fechaReposicion, double precioTotal, string comentario, IList<ReposicionItem> reposicionItems)
    {
        Id = id;
        FechaReposicion = fechaReposicion;
        PrecioTotal = precioTotal;
        Comentario = comentario;
        ReposicionItems = reposicionItems;
    }

    [Key]
    public string Id { get; set; }
    [System.ComponentModel.DataAnnotations.DataTypeAttribute(System.ComponentModel.DataAnnotations.DataType.Date)]
    public DateTime FechaReposicion { get; set; }

    public double PrecioTotal { get; set; }

    [StringLength(100, MinimumLength = 20, ErrorMessage = "El comentario debe tener entre 20 y 100 caracteres.")]
    public string? Comentario { get; set; }
    public IList<ReposicionItem> ReposicionItems { get; set; } = new List<ReposicionItem>();
}