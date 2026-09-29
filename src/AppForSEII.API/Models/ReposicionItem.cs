namespace AppForSEII.API.Models;

[PrimaryKey(nameof(LibroId), nameof(ReposicionId))]
public class ReposicionItem
{
    public ReposicionItem()
    {
    }
    public ReposicionItem(string libroId, string reposicionId, int cantidadReposicion, Libro libro, Reposicion reposicion)
    {
        CantidadReposicion = cantidadReposicion;
        LibroId = libroId;
        ReposicionId = reposicionId;
        Libro = libro;
        Reposicion = reposicion;
    }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad de reposición debe ser como mínimo 1.")]
    public int CantidadReposicion { get; set; }

    public string LibroId { get; set; }

    public string ReposicionId { get; set; }

    public Libro Libro { get; set; }

    public Reposicion Reposicion { get; set; }
}