using System.ComponentModel.DataAnnotations;

namespace AppForSEII.API.CompraDTOs;

public class CompraForCreateDTO
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "Los apellidos son obligatorios")]
    public string Apellidos { get; set; }

    [Required(ErrorMessage = "La dirección es obligatoria")]
    public string Direccion { get; set; }

    [Required(ErrorMessage = "El teléfono es obligatorio")]
    [Phone(ErrorMessage = "Formato de teléfono inválido")]
    public string Telefono { get; set; }

    [Required(ErrorMessage = "El método de pago es obligatorio")]
    public string MetodoPagoId { get; set; } 

    [StringLength(10, MinimumLength = 5, ErrorMessage = "El código de descuento debe tener entre 5 y 10 caracteres")]
    public string? CodigoDescuento { get; set; }

    [Required(ErrorMessage = "Debe seleccionar al menos un libro")]
    [MinLength(1, ErrorMessage = "Debe seleccionar al menos un libro")]
    public List<CompraItemForCreateDTO> LibrosSeleccionados { get; set; } = new List<CompraItemForCreateDTO>();
}

public class CompraItemForCreateDTO
{
    [Required]
    public int LibroId { get; set; }

    [Required]
    [Range(2, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que 1")] 
    public int Cantidad { get; set; }
}