using System.ComponentModel.DataAnnotations;

namespace ECommerceProject.Core.Models;

public class ProductCreateModels
{
    [Required] [StringLength(100)] public string Name { get; set; }
    [StringLength(1000)] public string Description { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public bool Discontinued { get; set; }
    public int CategoryId { get; set; }
}

public class ProductUpdateModels
{
    [Required] [StringLength(100)] public string Name { get; set; }
    [StringLength(1000)] public string Description { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public bool Discontinued { get; set; }
    public int CategoryId { get; set; }
}

public class ProductModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountedPrice { get; set; }
    public bool Discontinued { get; set; }
    public int CategoryId { get; set; }
    public int AccountId { get; set; }
    public string CategoryName { get; set; }
    public string AccountCompanyName { get; set; }
}