using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyServices;
using PaymentAPI.models;

namespace PaymentAPI.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
public class PayController : Controller
{
    private IConfiguration _configuration;

    public PayController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [AllowAnonymous]
    [HttpPost("authenticate")]
    [ProducesResponseType(200, Type = typeof(AuthResponseModel))]
    [ProducesResponseType(400, Type = typeof(string))]
    public IActionResult Authenticate([FromBody] AuthRequestModel model)
    {
        string uid = _configuration["Auth:Uid"]!;
        string pass = _configuration["Auth:Pass"]!;

        if (model.Username == uid && model.Password == pass)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim("uid", uid));

            string token = TokenService.GenerateToken(
                _configuration["JwtOptions:Key"]!,
                DateTime.Now.AddDays(30),
                claims,
                _configuration["JwtOptions:Issuer"]!,
                _configuration["JwtOptions:Audience"]!);
            return Ok(new AuthResponseModel { Token = token });
        }

        return BadRequest("Username ve Password eşleşmiyor!");
    }

    [HttpPost("payment")]
    [ProducesResponseType(200, Type = typeof(PaymentResponseModel))]
    [ProducesResponseType(400, Type = typeof(string))]
    public IActionResult Payment([FromBody] PaymentRequestModel model)
    {
        string cardNo = _configuration["CardTest:No"]!;
        string name = _configuration["CardTest:Name"]!;
        string exp = _configuration["CardTest:Exp"]!;
        string cvv = _configuration["CardTest:CVV"]!;

        if (model.CardNumber == cardNo && model.CardName == name && model.ExpireDate == exp && model.CVV == cvv)
        {
            return Ok(new PaymentResponseModel { Result = "ok", TransactionId = Guid.NewGuid().ToString() });
        }

        return BadRequest("Kart bilgileri geçersiz. Ödeme alınamadi.");
    }
}