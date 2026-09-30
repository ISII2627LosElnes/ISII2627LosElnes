
namespace AppForSEII.API.Models;

public class Resena
{
     public Resena()
{
}

public Resena(int id, DateTime fechaResena, string titulo)
{
    Id = id;
    FechaResena = fechaResena;
    Titulo = titulo;
}
    [Key]
    public int Id { get; set; }

    [System.ComponentModel.DataAnnotations.DataTypeAttribute(
    System.ComponentModel.DataAnnotations.DataType.Date)]
    public DateTime FechaResena { get; set; }

     [StringLength(20, MinimumLength = 10,
        ErrorMessage = "El título de la reseña debe tener entre 10 y 20 caracteres.")]
    public string Titulo { get; set; }

    public IList<ResenaItem> ResenaItems { get; set; } = new List<ResenaItem>();
}