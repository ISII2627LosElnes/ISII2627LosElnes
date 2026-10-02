namespace AppForSEII.API.Models;

public class PayPal: MetodoPago
{

    public PayPal(string id, string numeroTelefono): base(id)
    {
        NumeroTelefono = numeroTelefono;
    }

    [Key]
    public string NumeroTelefono { get; set; }

}