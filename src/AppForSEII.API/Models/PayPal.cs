namespace AppForSEII.API.Models;

public class PayPal
{

    public PayPal(int numeroTelefono)
    {
        NumeroTelefono = numeroTelefono;
    }

    [Key]
    public int NumeroTelefono { get; set; }

}