namespace AppForSEII.API.Models
{
    public class Compra
    {
        public Compra()
        {
            
        }

    public Compra(int id, DateTime fechaCompra, decimal precioTotal, string codigoDescuento)
        {
            Id = id;
            FechaCompra = fechaCompra;
            PrecioTotal = precioTotal;
            CodigoDescuento = codigoDescuento;
        }
        
        [Key]
        public int Id { get; set; }

        [System.ComponentModel.DataAnnotations.DataType(System.ComponentModel.DataAnnotations.DataType.Date)]
        public DateTime FechaCompra { get; set; }

        public decimal PrecioTotal { get; set; }

        [StringLength(10, MinimumLength = 5, ErrorMessage = "El código de descuento debe tener entre 5 y 10 caracteres.")]
         public string? CodigoDescuento { get; set; } 

          public IList<CompraItem> CompraItems { get; set; }= new List<CompraItem>();

    }
}