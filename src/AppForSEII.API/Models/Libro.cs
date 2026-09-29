namespace AppForSEII.API.Models;
public class Libro
{
    public Libro()
    {
    }

    public Libro(int id, string titulo, string autor, decimal precioTotal, int stock)
    {
        Id = id;
        Titulo = titulo;
        Autor = autor;
        PrecioTotal = precioTotal;
        Stock = stock;
    }

    public int Id { get; set; }

    public string Titulo { get; set; }

    public string Autor { get; set; }

    public decimal PrecioTotal { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser mayor que 0")]
    public int Stock { get; set; }

    
    
}