using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceProject.API.Entities;

[Table("CartProducts")]
public class CartProduct
{
    [Key] public int Id { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public int Quantity { get; set; }

    public int? ProductId { get; set; }
    public int? CartId { get; set; }

    public virtual Product Product { get; set; }
    public virtual Cart Cart { get; set; }
}