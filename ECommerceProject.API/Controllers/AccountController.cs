using System.Security.Claims;
using ECommerceProject.API.DataAccess;
using ECommerceProject.API.Entities;
using ECommerceProject.Core;
using Microsoft.AspNetCore.Mvc;
using MyServices;

namespace ECommerceProject.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    // Applyment: Satici Basvuru
    // Register : Üye Kaydi
    // Authenticate: Kimlik doğrulama


    private DatabaseContext _db;
    private IConfiguration _configuration;

    public AccountController(DatabaseContext databaseContext, IConfiguration configuration)
    {
        _db = databaseContext;
        _configuration = configuration;
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

    [HttpPost("register")]
    [ProducesResponseType(200, Type = typeof(Resp<RegisterResponseModel>))]
    [ProducesResponseType(400, Type = typeof(Resp<RegisterResponseModel>))]
    public IActionResult Register([FromBody] RegisterRequestModel model)
    {
        Resp<RegisterResponseModel> response = new Resp<RegisterResponseModel>();

        model.Username = model.Username.Trim().ToLower();
        if (_db.Accounts.Any(x => x.Username.ToLower() == model.Username))
        {
            response.AddError(nameof(model.Username), "Bu kullanici adi zaten kullaniliyor.");
            return BadRequest(response);
        }
        else
        {
            Account account = new Account
            {
                Username = model.Username,
                Password = model.Password,
                Type = AccountType.Member
            };
            _db.Accounts.Add(account);
            _db.SaveChanges();

            RegisterResponseModel data = new RegisterResponseModel
            {
                Id = account.Id,
                Username = account.Username
            };
            response.Data = data;
            return Ok(response);
        }
    }

    [HttpPost("authenticate")]
    [ProducesResponseType(200, Type = typeof(Resp<AuthenticateResponseModel>))]
    [ProducesResponseType(400, Type = typeof(Resp<AuthenticateResponseModel>))]
    public IActionResult Authenticate([FromBody] AuthenticateRequestModel model)
    {
        Resp<AuthenticateResponseModel> response = new Resp<AuthenticateResponseModel>();
        model.Username = model.Username.Trim().ToLower();
        Account? account =
            _db.Accounts.SingleOrDefault(
                x => x.Username.ToLower() == model.Username && x.Password == model.Password);
        if (account != null)
        {
            if (account.IsApplyment)
            {
                response.AddError("*", "Henüz Satici başvurusu tamamlanmamıştır.");
                return BadRequest(response);
            }

            string key = _configuration["JwtOptions:Key"]!;
            List<Claim> claims = new List<Claim>
            {
                new("id", account.Id.ToString()),
                new(ClaimTypes.Name, account.Username),
                new("type", ((int)account.Type).ToString()),
                new(ClaimTypes.Role, account.Type.ToString())
            };


            var token = TokenService.GenerateToken(jwtKey: key, expires: DateTime.Now.AddDays(30), claims: claims);

            AuthenticateResponseModel data = new AuthenticateResponseModel { Token = token };
            response.Data = data;

            return Ok(response);
        }

        response.AddError("*", "Kullanici adı ya da şifre eşleşmiyor");
        return BadRequest(response);
    }
}