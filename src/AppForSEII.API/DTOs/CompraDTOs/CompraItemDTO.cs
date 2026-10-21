namespace AppForSEII.API.CompraDTOs;

public class CompraItemDTO
{
    public CompraItemDTO(string titulo, string genero, string editorial, int cantidad)
    {
        Titulo = titulo;
        Genero = genero;
        Editorial = editorial;
        Cantidad = cantidad;
    }

    public string Titulo { get; set; }
    public string Genero { get; set; }
    public string Editorial { get; set; }
    public int Cantidad { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != typeof(CompraItemDTO))
        {
            return false;
        }

        CompraItemDTO otro = (CompraItemDTO)obj;

        return Titulo == otro.Titulo &&
               Genero == otro.Genero &&
               Editorial == otro.Editorial &&
               Cantidad == otro.Cantidad;
    }
}