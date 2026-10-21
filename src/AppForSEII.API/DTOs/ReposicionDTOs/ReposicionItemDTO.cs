namespace AppForSEII.API.ReposicionDTOs;

public class ReposicionItemDTO
{
    public ReposicionItemDTO(string titulo, string autor, int cantidad)
    {
        Titulo = titulo;
        Autor = autor;
        Cantidad = cantidad;
    }

    public string Titulo { get; set; }
    public string Autor { get; set; }
    public int Cantidad { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != typeof(ReposicionItemDTO))
        {
            return false;
        }

        ReposicionItemDTO otro = (ReposicionItemDTO)obj;

        return Titulo == otro.Titulo &&
               Autor == otro.Autor &&
               Cantidad == otro.Cantidad;
    }
}