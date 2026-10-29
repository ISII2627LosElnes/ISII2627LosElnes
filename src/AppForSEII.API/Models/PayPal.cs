namespace AppForSEII.API.Models;

public class PayPal: MetodoPago
{

    public PayPal(int id, string numeroTelefono): base(id)
    {
        NumeroTelefono = numeroTelefono;
    }
    public string NumeroTelefono { get; set; }

}