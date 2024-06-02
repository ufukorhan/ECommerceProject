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
[Authorize(Roles = "Admin, Merchant")]
public class ProductController : ControllerBase
{
    private DatabaseContext _db;
    private IConfiguration _configuration;

    public ProductController(DatabaseContext databaseContext, IConfiguration configuration)
    {
        _db = databaseContext;
        _configuration = configuration;
    }

    [HttpGet("list")]
    [ProducesResponseType(200, Type = typeof(Resp<List<ProductModel>>))]
    public IActionResult List()
    {
        Resp<List<ProductModel>> response = new Resp<List<ProductModel>>();
        
        List<ProductModel> list = _db.Products
            .Include(x => x.Category)
            .Include(x => x.Account)
            .Select(
                x => new ProductModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UnitPrice = x.UnitPrice,
                    DiscountedPrice = x.DiscountedPrice,
                    Discontinued = x.Discontinued,
                    CategoryId = x.CategoryId,
                    AccountId = x.AccountId,
                    CategoryName = x.Category.Name,
                    AccountCompanyName = x.Account.CompanyName
                })
            .ToList();

        response.Data = list;

        return Ok(response);
    }
    
    [HttpGet("list/{accountId}")]
    [ProducesResponseType(200, Type = typeof(Resp<List<ProductModel>>))]
    public IActionResult ListByAccountId([FromRoute] int accountId)
    {
        Resp<List<ProductModel>> response = new Resp<List<ProductModel>>();

        // int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);

        List<ProductModel> list = _db.Products
            .Include(x => x.Category)
            .Include(x => x.Account)
            .Where(x => x.AccountId == accountId)
            .Select(
                x => new ProductModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    UnitPrice = x.UnitPrice,
                    DiscountedPrice = x.DiscountedPrice,
                    Discontinued = x.Discontinued,
                    CategoryId = x.CategoryId,
                    AccountId = x.AccountId,
                    CategoryName = x.Category.Name,
                    AccountCompanyName = x.Account.CompanyName
                })
            .ToList();

        response.Data = list;

        return Ok(response);
    }

    [HttpGet("get/{productId}")]
    [ProducesResponseType(200, Type = typeof(Resp<ProductModel>))]
    [ProducesResponseType(404, Type = typeof(Resp<ProductModel>))]
    public IActionResult GetById([FromRoute] int productId)
    {
        Resp<ProductModel> response = new Resp<ProductModel>();
        
        Product? product = _db.Products
            .Include(x => x.Category)
            .Include(x => x.Account)
            .SingleOrDefault(x => x.Id == productId);

        if (product == null)
            return NotFound(response);

        ProductModel data = new ProductModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            UnitPrice = product.UnitPrice,
            DiscountedPrice = product.DiscountedPrice,
            Discontinued = product.Discontinued,
            CategoryId = product.CategoryId,
            AccountId = product.AccountId,
            CategoryName = product.Category.Name,
            AccountCompanyName = product.Account.CompanyName
        };

        response.Data = data;

        return Ok(response);
    }

    [HttpPost("create")]
    [ProducesResponseType(200, Type = typeof(Resp<ProductModel>))]
    [ProducesResponseType(400, Type = typeof(Resp<ProductModel>))]
    public IActionResult Create([FromBody] ProductCreateModels model)
    {
        Resp<ProductModel> response = new Resp<ProductModel>();
        string productName = model.Name.Trim().ToLower();

        if (_db.Products.Any(x => x.Name.ToLower() == productName))
        {
            response.AddError(nameof(model.Name), "Bu ürün adi zaten mevcuttur.");
            return BadRequest(response);
        }

        int accountId = int.Parse(HttpContext.User.FindFirst("id").Value);

        Product product = new Product
        {
            Name = model.Name,
            Description = model.Description,
            UnitPrice = model.UnitPrice,
            DiscountedPrice = model.DiscountedPrice,
            Discontinued = model.Discontinued,
            CategoryId = model.CategoryId,
            AccountId = accountId
        };
        _db.Products.Add(product);
        _db.SaveChanges();

        product = _db.Products
            .Include(x => x.Category)
            .Include(x => x.Account)
            .SingleOrDefault(x => x.Id == product.Id)!;

        ProductModel data = new ProductModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            UnitPrice = product.UnitPrice,
            DiscountedPrice = product.DiscountedPrice,
            Discontinued = product.Discontinued,
            CategoryId = product.CategoryId,
            AccountId = product.AccountId,
            CategoryName = product.Category.Name,
            AccountCompanyName = product.Account.CompanyName
        };
        response.Data = data;

        return Ok(response);
    }

    /*[HttpPut("update/{id}")]
    public IActionResult Update([FromRoute] int id, [FromBody] CategoryUpdateModel model)
    {
    
    }

    [HttpDelete("delete/{id}")]
    public IActionResult Delete([FromRoute] int id)
    {

    }*/
}