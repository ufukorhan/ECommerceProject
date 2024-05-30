using ECommerceProject.API.DataAccess;
using ECommerceProject.API.Entities;
using ECommerceProject.Core;
using Microsoft.AspNetCore.Mvc;


namespace ECommerceProject.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    // Applyment: Satici Basvuru
    // Register : Üye Kaydi
    // Authenticate: Kimlik doğrulama


    private DatabaseContext _db;

    public AccountController(DatabaseContext databaseContext)
    {
        _db = databaseContext;
    }

    [HttpPost("merchant/applyment")]
    [ProducesResponseType(200, Type = typeof(Resp<ApplymentAccountResponseModel>))]
    [ProducesResponseType(400, Type = typeof(Resp<ApplymentAccountResponseModel>))]
    public IActionResult Applyment([FromBody] ApplymentAccountRequestModel model)
    {
        Resp<ApplymentAccountResponseModel> response = new Resp<ApplymentAccountResponseModel>();

        model.Username = model.Username.Trim().ToLower();
        if (_db.Accounts.Any(x => x.Username.ToLower() == model.Username))
        {
            response.AddError(nameof(model.Username), "Bu kullanici adi zaten kullaniliyor.");
            return BadRequest(response);
        }

        Account account = new Account
        {
            Username = model.Username,
            Password = model.Password,
            CompanyName = model.CompanyName,
            ContactEmail = model.ContactEmail,
            ContactName = model.ContactName,
            Type = AccountType.Merchant,
            IsApplyment = true
        };
        _db.Accounts.Add(account);
        _db.SaveChanges();

        ApplymentAccountResponseModel applymentAccountResponseModel = new ApplymentAccountResponseModel
        {
            Id = account.Id,
            Username = account.Username,
            ContactName = account.ContactName,
            CompanyName = account.CompanyName,
            ContactEmail = account.ContactEmail
        };

        response.Data = applymentAccountResponseModel;

        return Ok(response);
    }
}