namespace AppForSEII.API.Models;

public class Visa : MetodoPago
{
    public Visa(string id, string numeroTarjeta, DateTime fechaCaducidad) : base(id)
    {
        NumeroTarjeta = numeroTarjeta;
        FechaCaducidad = fechaCaducidad;
    }

    [Required]
    [CreditCard]
    [StringLength(16)]
    public string NumeroTarjeta { get; set; }
    [Required]
    public DateTime FechaCaducidad { get; set; }
}