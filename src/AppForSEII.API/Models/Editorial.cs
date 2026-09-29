namespace AppForSEII.API.Models;

public class Editorial
{

public Editorial(int id, string nombre)
    {
        Id = id;
        Nombre = nombre;
    }

    [Key]
public int Id { get; set; }

public string Nombre { get; set; }

public IList<Libro> Libros { get; set; } = new List<Libro>();
}