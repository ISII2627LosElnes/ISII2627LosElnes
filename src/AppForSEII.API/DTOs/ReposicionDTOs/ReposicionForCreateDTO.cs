namespace AppForSEII.API.ReposicionDTOs
{
    public class ReposicionForCreateDTO
    {
        public ReposicionForCreateDTO(string nombre, string apellido, string telefono, MetodoPago metodoPago, List<ReposicionItemDTO> reposicionItems, string? comentario)
        {
            Nombre = nombre;
            Apellido = apellido;
            Telefono = telefono;
            MetodoPago = metodoPago;
            ReposicionItems = reposicionItems;
            Comentario = comentario;
        }
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, introduzca su nombre")]
        public string Nombre { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, introduzca su apellido")]
        public string Apellido { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, introduzca su teléfono")]
        public string Telefono { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Por favor, seleccione un método de pago")]
        public MetodoPago MetodoPago { get; set; }
        [Required]
        public List<ReposicionItemDTO> ReposicionItems { get; set; }
        
        [StringLength(100, MinimumLength = 20, ErrorMessage = "El comentario debe tener entre 20 y 100 caracteres")]
        public string? Comentario { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is ReposicionForCreateDTO dTO &&
                   Nombre == dTO.Nombre &&
                   Apellido == dTO.Apellido &&
                   Telefono == dTO.Telefono &&
                   MetodoPago == dTO.MetodoPago &&
                   Comentario == dTO.Comentario &&
                   ReposicionItems.SequenceEqual(dTO.ReposicionItems);
        }

    }
}