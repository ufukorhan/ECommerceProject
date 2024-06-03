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