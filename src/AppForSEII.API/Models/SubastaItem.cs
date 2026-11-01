namespace AppForSEII.API.Models;

[PrimaryKey(nameof(LibroId), nameof(SubastaId))]

public class SubastaItem
{
    public SubastaItem()
    {
    }

    public SubastaItem(string? descripcion, decimal precioPuja, Libro libro, Subasta subasta, int cantidadSubasta)
    {
        Descripcion = descripcion;
        PrecioPuja = precioPuja;
        Libro = libro;
        LibroId = libro.Id;
        SubastaId = subasta.Id;
        Subasta = subasta;
        CantidadSubasta = cantidadSubasta;
    }

    
    [StringLength(100, MinimumLength = 20, ErrorMessage = "El código de descuento debe tener entre 20 y 100 caracteres.")]
    public string? Descripcion { get; set; }

    [Required]
    public decimal PrecioPuja { get; set; }
    public Subasta Subasta { get; set; }
    public Libro Libro { get; set; }

    public int LibroId { get; set; }

    public int SubastaId { get; set; }
    public int CantidadSubasta { get; set; }
}