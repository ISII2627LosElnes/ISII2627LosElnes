namespace AppForSEII.API.CompraDTOs;

public class ComprasDetailDTO
{
    public ComprasDetailDTO(string nombre, string apellidos, string direccion, string telefono, DateTime fechaCompra, decimal precioTotal, string? codigoDescuento, IList<CompraItemDTO> compras, string metodoPago)
    {
        Nombre = nombre;
        Apellidos = apellidos;
        Direccion = direccion;
        Telefono = telefono;
        FechaCompra = fechaCompra;
        PrecioTotal = precioTotal;
        CodigoDescuento = codigoDescuento;
        Compraitems = compras;
        MetodoPago = metodoPago;
    }

    public string Nombre { get; set; }
    public string Apellidos { get; set; }
    public string Direccion { get; set; }
    public string Telefono { get; set; }
    public DateTime FechaCompra { get; set; }
    public decimal PrecioTotal { get; set; }
    public string? CodigoDescuento { get; set; }
    public string MetodoPago { get; set; }

    public IList<CompraItemDTO> Compraitems { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != typeof(ComprasDetailDTO))
        {
            return false;
        }

        ComprasDetailDTO otro = (ComprasDetailDTO)obj;

        return Nombre == otro.Nombre &&
               Apellidos == otro.Apellidos &&
               Direccion == otro.Direccion &&
               Telefono == otro.Telefono &&
               FechaCompra == otro.FechaCompra &&
               PrecioTotal == otro.PrecioTotal &&
               CodigoDescuento == otro.CodigoDescuento &&
               MetodoPago == otro.MetodoPago &&
               Compraitems.SequenceEqual(otro.Compraitems);
    }
}