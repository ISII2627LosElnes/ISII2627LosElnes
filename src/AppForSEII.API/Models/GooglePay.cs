namespace AppForSEII.API.Models;

public class GooglePay
{
    public GooglePay(String email)
    {
        Email = email;
    }

    [Key]
    public String Email { get; set; }
}