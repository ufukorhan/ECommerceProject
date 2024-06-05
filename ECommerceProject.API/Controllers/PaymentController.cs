using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ECommerceProject.API.DataAccess;
using ECommerceProject.API.Entities;
using ECommerceProject.Core;
using ECommerceProject.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyServices;
using PaymentAPI.models;

namespace ECommerceProject.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Admin")]
public class PaymentController : ControllerBase
{
    private DatabaseContext _db;
    private IConfiguration _configuration;

    public PaymentController(DatabaseContext databaseContext, IConfiguration configuration)
    {
        _db = databaseContext;
        _configuration = configuration;
    }

    [HttpPost("Pay/{cartId}")]
    [ProducesResponseType(200, Type = typeof(Resp<PaymentModel>))]
    [ProducesResponseType(400, Type = typeof(Resp<string>))]
    public IActionResult Pay([FromRoute] int cartId, [FromBody] PayModel model)
    {
        Resp<PaymentModel> result = new Resp<PaymentModel>();
        Cart? cart = _db.Carts
            .Include(x => x.CartProducts)
            .SingleOrDefault(x => x.Id == cartId);

        string paymentApiEndpoint = _configuration["PaymentAPI:Endpoint"]!;
        
        HttpClientService client = new HttpClientService(domain: paymentApiEndpoint);

        if (!cart.IsClosed)
        {
            decimal totalPrice = model.TotalPriceOverride ?? cart.CartProducts.Sum(x => x.Quantity * x.DiscountedPrice);

            AuthRequestModel authRequestModel = new AuthRequestModel { Username = "ufukorhan", Password = "123123" };

            HttpClientServiceResponse<AuthResponseModel> authResponse =
                client.Post<AuthRequestModel, AuthResponseModel>(
                    fragment: "/Pay/authenticate", data: authRequestModel);

            if (authResponse.StatusCode == HttpStatusCode.OK)
            {
                string token = authResponse.Data.Token;

                PaymentRequestModel paymentRequestModel = new PaymentRequestModel
                {
                    CardNumber = model.CardNumber,
                    CardName = model.CardName,
                    ExpireDate = model.ExpireDate,
                    CVV = model.CVV,
                    TotalPrice = totalPrice
                };

                HttpClientServiceResponse<PaymentResponseModel> paymentResponse =
                    client.Post<PaymentRequestModel, PaymentResponseModel>(
                        fragment: "/Pay/Payment", data: paymentRequestModel, token: token);

                if (paymentResponse.StatusCode == HttpStatusCode.OK)
                {
                    if (paymentResponse.Data.Result == "ok")
                    {
                        string transactionId = paymentResponse.Data.TransactionId;
                        Payment payment = new Payment
                        {
                            CartId = cartId,
                            AccountId = cart.AccountId,
                            InvoiceAddress = model.InvoiceAddress,
                            ShippedAddress = model.ShippedAddress,
                            Type = model.Type,
                            TransactionId = transactionId,
                            Date = DateTime.UtcNow,
                            IsCompleted = true,
                            TotalPrice = totalPrice
                        };
                        cart.IsClosed = true;
                        _db.Payments.Add(payment);
                        _db.SaveChanges();

                        PaymentModel data = new PaymentModel
                        {
                            Id = payment.Id,
                            AccountId = payment.AccountId,
                            CartId = payment.CartId,
                            Date = payment.Date,
                            InvoiceAddress = payment.InvoiceAddress,
                            IsCompleted = payment.IsCompleted,
                            ShippedAddress = payment.ShippedAddress,
                            TotalPrice = payment.TotalPrice,
                            Type = payment.Type
                        };

                        result.Data = data;
                        return Ok(result);
                    }

                    Resp<string> paymentOkResult = new Resp<string>();
                    paymentOkResult.AddError("payment", "Ödeme alinamadi.");
                    return BadRequest(paymentOkResult);
                }

                Resp<string> paymentResult = new Resp<string>();
                paymentResult.AddError("payment", paymentResponse.ResponseContent);
                return BadRequest(paymentResult);
            }

            Resp<string> authResult = new Resp<string>();
            authResult.AddError("username", authResponse.ResponseContent);

            return BadRequest(authResult);
        }

        Payment paymentRes = _db.Payments.SingleOrDefault(x => x.CartId == cartId);
        if (paymentRes == null)
        {
            result.AddError(
                "cart",
                $"Sepet kapalı ama ödemesi yapılmamış görünmektedir. Sorun için sistem sağlayıcısı ile görüşmeniz gerekiypr.. Cart Id : {cartId}");
            return BadRequest(result);
        }

        PaymentModel payData = new PaymentModel
        {
            Id = paymentRes.Id,
            AccountId = paymentRes.AccountId,
            CartId = paymentRes.CartId,
            Date = paymentRes.Date,
            InvoiceAddress = paymentRes.InvoiceAddress,
            IsCompleted = paymentRes.IsCompleted,
            ShippedAddress = paymentRes.ShippedAddress,
            TotalPrice = paymentRes.TotalPrice,
            Type = paymentRes.Type
        };

        result.Data = payData;

        return Ok(result);
    }
}