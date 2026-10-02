namespace AppForSEII.API.Models;

public class GooglePay: MetodoPago
{
    public GooglePay(string id, string email): base(id)
    {
        Email = email;
    }

    [Key]
    public string Email { get; set; }
}