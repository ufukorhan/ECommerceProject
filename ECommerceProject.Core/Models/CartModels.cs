namespace ECommerceProject.Core.Models;

public class CartModel
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public bool IsClosed { get; set; }
    public int AccountId { get; set; }
    public List<CartProductModel> CartProducts { get; set; }
}

public class CartProductModel
{
    public int Id { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public int Quantity { get; set; }

    public int ProductId { get; set; }
    public int CartId { get; set; }
}

public class AddToCartModel
{
    public int Quantity { get; set; }

    public int? ProductId { get; set; }
}