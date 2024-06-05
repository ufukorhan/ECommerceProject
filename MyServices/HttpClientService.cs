using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace MyServices;

public class HttpClientService
{
    public string Domain { get; set; }

    public HttpClientService(string domain)
    {
        Domain = domain;
    }


    public HttpClientServiceResponse<TResponse> Post<TRequest, TResponse>(string fragment, TRequest data,
        string token = "")
    {
        HttpClient client = new HttpClient();
        if (string.IsNullOrEmpty(token) == false)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                scheme: JwtBearerDefaults.AuthenticationScheme, token);
        }

        StringContent content = new StringContent(JsonSerializer.Serialize(data), encoding: Encoding.UTF8,
            mediaType: "application/json");

        HttpResponseMessage response = client.PostAsync($"{Domain}{fragment}", content).Result;

        HttpClientServiceResponse<TResponse> result = new HttpClientServiceResponse<TResponse>();

        result.StatusCode = response.StatusCode;
        result.ResponseContent = response.Content.ReadAsStringAsync().Result;

        if (response.StatusCode == HttpStatusCode.OK)
        {
            result.Data = JsonSerializer.Deserialize<TResponse>(result.ResponseContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        return result;
    }
}