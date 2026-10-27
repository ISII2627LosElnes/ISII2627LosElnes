namespace AppForSEII.API.ResenaDTOs;
using System.ComponentModel.DataAnnotations;


public class ResenaForCreateDTO
{
    public ResenaForCreateDTO(string nombre, string apellidos, string direccion, string telefono, string tituloResena, IList<ResenaItemDTO> resenaItems)
    {
        // CalificacionMedia = calificacionMedia;
       // PrecioCompra = precioCompra;
        Nombre = nombre;
        Apellidos = apellidos;
        Direccion = direccion;
        Telefono = telefono;
        TituloResena = tituloResena;
        ResenaItems = resenaItems;
    
    }

   // public string CalificacionMedia { get; set; }
   // public decimal PrecioCompra { get; set; }
   [Required]
    public string Nombre { get; set; }
    [Required]
    public string Apellidos { get; set; }
    [Required]
    public string Direccion { get; set; }
    [Required]
    public string Telefono { get; set; }

    [Required]
    [StringLength(20, MinimumLength = 10)]
    public string TituloResena { get; set; }
    [Required]
    public IList<ResenaItemDTO> ResenaItems { get; set; } 

    public override bool Equals(object? obj)
{
    if (obj == null || obj.GetType() != typeof(ResenaForCreateDTO))
    {
        return false;
    }

    ResenaForCreateDTO otro = (ResenaForCreateDTO)obj;

    return Nombre == otro.Nombre &&
           Apellidos == otro.Apellidos &&
           Direccion == otro.Direccion &&
           Telefono == otro.Telefono &&
           TituloResena == otro.TituloResena &&
           ResenaItems.SequenceEqual(otro.ResenaItems);
}
        
    
}