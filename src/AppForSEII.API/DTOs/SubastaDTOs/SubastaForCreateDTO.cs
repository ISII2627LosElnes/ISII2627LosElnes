namespace AppForSEII.API.DTOs.SubastaDTOs
{
    public class SubastaForCreateDTO : IEquatable<SubastaForCreateDTO>
    {
        public SubastaForCreateDTO(string nombre, string apellidos, string telefono, int metodoPago, string? descripcion, IList<SubastaItemDTO> subastaItems)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            Apellidos = apellidos ?? throw new ArgumentNullException(nameof(apellidos));
            Telefono = telefono ?? throw new ArgumentNullException(nameof(telefono));
            MetodoPago = metodoPago;
            Descripcion = descripcion;
            SubastaItems = subastaItems ?? throw new ArgumentNullException(nameof(subastaItems));
        }

        public SubastaForCreateDTO()
        {
            SubastaItems = new List<SubastaItemDTO>();
        }


        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellidos { get; set; }

        [Required]
        public string Telefono { get; set; }

        [Required]
        public int MetodoPago { get; set; }

        [StringLength(100, MinimumLength = 20, ErrorMessage = "Longitud mínima de 20 caracteres")]
        public string? Descripcion { get; set; }
        public IList<SubastaItemDTO> SubastaItems { get; set; }=new List<SubastaItemDTO>();
        

        public decimal PrecioTotal
        {
            get
            {
                return SubastaItems.Sum(s => s.PrecioPuja);
            }
        }

        
        public bool Equals(SubastaForCreateDTO? other)
        {
            return other is not null
                && Nombre == other.Nombre
                && Apellidos == other.Apellidos
                && Telefono == other.Telefono
                && MetodoPago == other.MetodoPago
                && Descripcion == other.Descripcion
                && SubastaItems.SequenceEqual(other.SubastaItems);
        }

        public override bool Equals(object? obj)
        {
            return obj is SubastaForCreateDTO other && Equals(other);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Nombre);
            hash.Add(Apellidos);
            hash.Add(Telefono);
            hash.Add(MetodoPago);
            hash.Add(Descripcion);
            foreach (var item in SubastaItems)
            {
                hash.Add(item);
            }
            return hash.ToHashCode();
        }
    }
}