using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceProject.API.Entities;

[Table("Products")]
public class Product
{
    [Key] public int Id { get; set; }

    [Required] [StringLength(100)] public string Name { get; set; }
    [StringLength(1000)] public string Description { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public bool Discontinued { get; set; }
    public int CategoryId { get; set; }
    public int AccountId { get; set; }
    public virtual Category Category { get; set; }
    public virtual Account Account { get; set; }
}