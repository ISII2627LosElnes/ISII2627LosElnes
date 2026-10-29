namespace AppForSEII.API.Models
{
    public class Compra
    {
        public Compra()
        {
            
        }

    public Compra(int id, DateTime fechaCompra, decimal precioTotal, string codigoDescuento, ApplicationUser usuario, MetodoPago metodoPago)
        {
            Id = id;
            FechaCompra = fechaCompra;
            PrecioTotal = precioTotal;
            CodigoDescuento = codigoDescuento;
            Usuario = usuario;
            MetodoPago = metodoPago;
        }
        
        [Key]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime FechaCompra { get; set; }

        public decimal PrecioTotal { get; set; }

        [StringLength(10, MinimumLength = 5, ErrorMessage = "El código de descuento debe tener entre 5 y 10 caracteres.")]
        public string? CodigoDescuento { get; set; } 

        public IList<CompraItem> CompraItems { get; set; }= new List<CompraItem>();

        public ApplicationUser Usuario { get; set; }

        public MetodoPago MetodoPago { get; set; }


        public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != typeof(Compra))
        {
            return false;
        }

        Compra otra = (Compra)obj;

        return Id == otra.Id &&
               FechaCompra == otra.FechaCompra &&
               PrecioTotal == otra.PrecioTotal &&
               CodigoDescuento == otra.CodigoDescuento &&
               (Usuario != null ? Usuario.Equals(otra.Usuario) : otra.Usuario == null) &&
               (MetodoPago != null ? MetodoPago.Equals(otra.MetodoPago) : otra.MetodoPago == null) &&
               CompraItems.SequenceEqual(otra.CompraItems);
    }
        

    }
}