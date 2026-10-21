using Microsoft.AspNetCore.Identity;

namespace AppForSEII.API.Models;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    public ApplicationUser()
    {
    }
    public ApplicationUser(string id, string name, string surname, string userName,string address)
    {
        Id = id;
        Name = name;
        Surname = surname;
        UserName = userName;
        Email = userName;
        Address = address;

    }

    [StringLength(50)]
    public string? Name {get;set;}

    [StringLength(50)]
    public string? Surname {get;set;}

    [StringLength(150)]
    public string? Address {get;set;}

    public IList<Resena> Resenas { get; set; } = new List<Resena>();
    public IList<Reposicion> Reposiciones { get; set; } = new List<Reposicion>();
     public IList<Compra> Compras { get; set; } = new List<Compra>();

    

    
}
