using ECommerceProject.API.DataAccess;
using ECommerceProject.API.Entities;
using ECommerceProject.Core;
using ECommerceProject.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerceProject.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Admin")]
public class CartController : ControllerBase
{
    // GetOrCreate: sepet getir ya da oluştur
    // AddToCart: Sepete ürün ekleme

    private DatabaseContext _db;
    private IConfiguration _configuration;

    public CartController(DatabaseContext databaseContext, IConfiguration configuration)
    {
        _db = databaseContext;
        _configuration = configuration;
    }

    [HttpGet("GetOrCreate/{accountId}")]
    [ProducesResponseType(200, Type = typeof(Resp<CartModel>))]
    public IActionResult GetOrCreate([FromRoute] int accountId)
    {
        Resp<CartModel> response = new Resp<CartModel>();
        Cart? cart = _db.Carts
            .Include(x => x.CartProducts)
            .SingleOrDefault(x => x.AccountId == accountId && x.IsClosed == false);

        if (cart == null)
        {
            cart = new Cart
            {
                AccountId = accountId,
                Date = DateTime.UtcNow,
                IsClosed = false,
                CartProducts = new List<CartProduct>()
            };
            _db.Carts.Add(cart);
            _db.SaveChanges();
        }

        CartModel data = CartToCartModel(cart);

        response.Data = data;
        return Ok(response);
    }

    [HttpPost("AddToCart/{accountId}")]
    [ProducesResponseType(200, Type = typeof(Resp<CartModel>))]
    public IActionResult AddToCart([FromRoute] int accountId, [FromBody] AddToCartModel model)
    {
        Resp<CartModel> response = new Resp<CartModel>();
        Cart? cart = _db.Carts
            .Include(x => x.CartProducts)
            .SingleOrDefault(x => x.AccountId == accountId && x.IsClosed == false);

        if (cart == null)
        {
            cart = new Cart
            {
                AccountId = accountId,
                Date = DateTime.UtcNow,
                IsClosed = false,
                CartProducts = new List<CartProduct>()
            };
            _db.Carts.Add(cart);
            _db.SaveChanges();
        }

        Product? product = _db.Products.Find(model.ProductId);
        cart.CartProducts.Add(new CartProduct
        {
            CartId = cart.Id,
            ProductId = product.Id,
            UnitPrice = product.UnitPrice,
            DiscountedPrice = product.DiscountedPrice,
            Quantity = model.Quantity
        });
        _db.SaveChanges();

        CartModel data = CartToCartModel(cart);
        response.Data = data;
        
        return Ok(response);
    }

    private static CartModel CartToCartModel(Cart cart)
    {
        CartModel data = new CartModel
        {
            Id = cart.Id,
            AccountId = cart.AccountId,
            Date = cart.Date,
            IsClosed = cart.IsClosed,
            CartProducts = new List<CartProductModel>()
        };

        foreach (CartProduct cartProduct in cart.CartProducts)
        {
            data.CartProducts.Add(new CartProductModel
            {
                Id = cartProduct.Id,
                CartId = cartProduct.CartId.Value,
                UnitPrice = cartProduct.UnitPrice,
                DiscountedPrice = cartProduct.DiscountedPrice,
                Quantity = cartProduct.Quantity,
                ProductId = cartProduct.ProductId.Value
            });
        }

        return data;
    }
}

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Admin")]
public class CategoryController : ControllerBase
{
    private DatabaseContext _db;
    private IConfiguration _configuration;

    public CategoryController(DatabaseContext databaseContext, IConfiguration configuration)
    {
        _db = databaseContext;
        _configuration = configuration;
    }

    [HttpGet("list")]
    [ProducesResponseType(200, Type = typeof(Resp<List<CategoryModel>>))]
    public IActionResult List()
    {
        Resp<List<CategoryModel>> response = new Resp<List<CategoryModel>>();
        List<CategoryModel> list = _db.Categories.Select(
            x => new CategoryModel { Id = x.Id, Name = x.Name, Description = x.Description }).ToList();

        response.Data = list;

        return Ok(response);
    }

    [HttpGet("get/{id}")]
    [ProducesResponseType(200, Type = typeof(Resp<CategoryModel>))]
    [ProducesResponseType(404, Type = typeof(Resp<CategoryModel>))]
    public IActionResult getById([FromRoute] int id)
    {
        Resp<CategoryModel> response = new Resp<CategoryModel>();

        Category? category = _db.Categories.SingleOrDefault(x => x.Id == id);
        CategoryModel data = null;
        if (category == null)
            return NotFound(response);

        data = new CategoryModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        response.Data = data;

        return Ok(response);
    }

    [HttpPost("create")]
    [ProducesResponseType(200, Type = typeof(Resp<CategoryModel>))]
    [ProducesResponseType(400, Type = typeof(Resp<CategoryModel>))]
    public IActionResult Create([FromBody] CategoryCreateModel model)
    {
        Resp<CategoryModel> response = new Resp<CategoryModel>();
        string categoryName = model.Name.Trim().ToLower();

        if (_db.Categories.Any(x => x.Name.ToLower() == categoryName))
        {
            response.AddError(nameof(model.Name), "Bu kategori adi zaten mevcuttur.");
            return BadRequest(response);
        }

        Category category = new Category
        {
            Name = model.Name,
            Description = model.Description
        };
        _db.Categories.Add(category);
        _db.SaveChanges();

        CategoryModel data = new CategoryModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
        response.Data = data;

        return Ok(response);
    }

    [HttpPut("update/{id}")]
    [ProducesResponseType(200, Type = typeof(Resp<CategoryModel>))]
    [ProducesResponseType(400, Type = typeof(Resp<CategoryModel>))]
    [ProducesResponseType(404, Type = typeof(Resp<CategoryModel>))]
    public IActionResult Update([FromRoute] int id, [FromBody] CategoryUpdateModel model)
    {
        Resp<CategoryModel> response = new Resp<CategoryModel>();
        Category? category = _db.Categories.Find(id);
        if (category == null)
            return NotFound(response);

        string categoryName = model.Name.Trim().ToLower();

        if (_db.Categories.Any(x => x.Name.ToLower() == categoryName && x.Id != id))
        {
            response.AddError(nameof(model.Name), "Bu kategori adi zaten mevcuttur.");
            return BadRequest(response);
        }

        category.Name = model.Name;
        category.Description = model.Description;

        _db.SaveChanges();

        CategoryModel data = new CategoryModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };

        response.Data = data;

        return Ok(response);
    }

    [HttpDelete("delete/{id}")]
    [ProducesResponseType(200, Type = typeof(Resp<object>))]
    [ProducesResponseType(404, Type = typeof(Resp<object>))]
    public IActionResult Delete([FromRoute] int id)
    {
        Resp<object> response = new Resp<object>();

        Category? category = _db.Categories.Find(id);
        if (category == null)
            return NotFound(response);

        _db.Categories.Remove(category);
        _db.SaveChanges();

        return Ok(response);
    }
}