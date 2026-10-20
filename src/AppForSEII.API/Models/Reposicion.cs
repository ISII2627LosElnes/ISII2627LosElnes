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
    public ApplicationUser Usuario { get; set; }

    //Equals y hashcode para comparar objetos Reposicion mediando el ID.
    public bool Equals(Reposicion? other) =>
        other is not null &&
        (ReferenceEquals(this, other) ||
         (Id is not null && other.Id is not null &&
          StringComparer.Ordinal.Equals(Id, other.Id)));

    public override bool Equals(object? obj) =>
        obj is Reposicion other && Equals(other);

    public override int GetHashCode() =>
        Id is null ? 0 : StringComparer.Ordinal.GetHashCode(Id);
    
}