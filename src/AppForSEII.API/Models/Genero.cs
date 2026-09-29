namespace AppForSEII.API.Models;

public class Genero
{

    public Genero()
{
}

public Genero(int id, string nombre)
{
    Id = id;
    Nombre = nombre;
}
public int Id { get; set; }

public string Nombre { get; set; } 

public IList<Libro> Libros { get; set; } = new List<Libro>();
}