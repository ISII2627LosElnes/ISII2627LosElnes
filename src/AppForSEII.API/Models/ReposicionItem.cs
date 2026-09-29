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

    public int CantidadReposicion { get; set; }

    public string LibroId { get; set; }

    public string ReposicionId { get; set; }

    public Libro Libro { get; set; }

    public Reposicion Reposicion { get; set; }
}