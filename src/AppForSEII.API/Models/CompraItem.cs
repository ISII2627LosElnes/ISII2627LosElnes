namespace AppForSEII.API.Models;
[PrimaryKey(nameof(LibroId), nameof(CompraId))]
public class CompraItem
{

    public CompraItem()
    {
    }

    public CompraItem(int id, int cantidad, Libro libro, Compra compra)
    {
        Cantidad = cantidad;
        Libro = libro;
        Compra = compra;
        LibroId = libro.Id;
        CompraId = compra.Id;
    }

    public Libro Libro { get; set; }

    public Compra Compra { get; set; }
     [Range(1,int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 1")]
    
    [Required]
    public int Cantidad { get; set; } 

    public int LibroId { get; set; }

    public int CompraId { get; set; }


}


