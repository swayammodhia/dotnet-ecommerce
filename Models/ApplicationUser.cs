using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ElectronyatShop.Models;

public class ApplicationUser : IdentityUser
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; }

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; }

    public string FullName => $"{ FirstName } { LastName }";

    [Required]
    [MaxLength(255)]
    public string Address { get; set; }

    public ICollection<Order>? Orders { get; set; }
}
