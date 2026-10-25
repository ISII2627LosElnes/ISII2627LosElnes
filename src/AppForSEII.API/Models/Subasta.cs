namespace AppForSEII.API.Models;

public class Subasta
{
    public Subasta()
    {
        
    }

    // Constructor con parámetros
    public Subasta(int id, double precioSubasta, DateTime fechaSubasta, MetodoPago metodoPago)
    {
        Id = id;
        PrecioSubasta = precioSubasta;
        FechaSubasta = fechaSubasta;
        MetodoPago = metodoPago;
    }
    


    [Key]
    public int Id { get; set; }

    public double PrecioSubasta { get; set; }

    [System.ComponentModel.DataAnnotations.DataTypeAttribute(
    System.ComponentModel.DataAnnotations.DataType.Date)]
    public DateTime FechaSubasta { get; set; }
    public IList<SubastaItem> SubastaItems { get; set; } = new List<SubastaItem>();
    public ApplicationUser Usuario { get; set; }
    public MetodoPago MetodoPago { get; set; }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is not Subasta other)
        {
            return false;
        }

        return Id == other.Id &&
               PrecioSubasta.Equals(other.PrecioSubasta) &&
               FechaSubasta.Equals(other.FechaSubasta) &&
               System.Collections.Generic.EqualityComparer<MetodoPago>.Default.Equals(MetodoPago, other.MetodoPago);
    }

    public override int GetHashCode()
    {
        var hash = new System.HashCode();
        hash.Add(Id);
        hash.Add(PrecioSubasta);
        hash.Add(FechaSubasta);
        hash.Add(MetodoPago);
        return hash.ToHashCode();
    }
}