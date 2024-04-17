using System.Net;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MdkRestServerless;

public class ProductsEndpoint
{
    private readonly ILogger _logger;
    private readonly AppDbContext _dbContext;

    public ProductsEndpoint(ILoggerFactory loggerFactory, AppDbContext dbContext)
    {
        _logger = loggerFactory.CreateLogger<ProductsEndpoint>();
        _dbContext = dbContext;
    }

    [Function("ProductsEndpoint")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.User, "get", "post", Route ="products")] HttpRequestData req)
    {
        _logger.LogInformation($"ProductsEndpoint function processed a ${req.Method} request.");

        if (req.Method == "POST")
        {
            // deserialize the request body into a Product object with System.Text.Json
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var product = JsonSerializer.Deserialize<Product>(requestBody);
            if (product == null)
            {
                _logger.LogError("Invalid product data.");
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }
            _dbContext.Products.Add(product);
            await _dbContext.SaveChangesAsync();

            var response = req.CreateResponse(HttpStatusCode.Created);
            response.Headers.Add("Location", $"/products/{product.Id}");
            return response;
        }
        if (req.Method == "GET")
        {
            var products = await _dbContext.Products.ToArrayAsync<Product>();
            var response = req.CreateResponse(HttpStatusCode.OK);
            // response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            await response.WriteAsJsonAsync(products);
            return response;
        }

        return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
    }
}
