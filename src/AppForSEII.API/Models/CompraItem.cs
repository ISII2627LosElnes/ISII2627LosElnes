namespace AppForSEII.API.Models;
[PrimaryKey(nameof(LibroId), nameof(CompraId))]
public class CompraItem
{
    public int Cantidad { get; set; } 

    public int LibroId { get; set; }

    public int CompraId { get; set; }


}


