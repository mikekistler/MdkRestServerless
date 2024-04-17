using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MdkRestServerless
{
    public class ProductItemEndpoint
    {
        private readonly ILogger _logger;
        private readonly AppDbContext _dbContext;

        public ProductItemEndpoint(ILoggerFactory loggerFactory, AppDbContext dbContext)
        {
            _logger = loggerFactory.CreateLogger<ProductItemEndpoint>();
            _dbContext = dbContext;
        }

        [Function("ProductItemEndpoint")]
        public async Task<HttpResponseData> RunAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", "patch", "delete", Route = "products/{id}")] HttpRequestData req, int id)
        {
            _logger.LogInformation($"ProductItemEndpoint function processed a ${req.Method} request.");

            if (req.Method == "GET")
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    return req.CreateResponse(HttpStatusCode.NotFound);
                }
                var response = req.CreateResponse(HttpStatusCode.OK);
                // response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                await response.WriteAsJsonAsync(product);

                return response;
            }
            if (req.Method == "PATCH")
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    return req.CreateResponse(HttpStatusCode.NotFound);
                }
                // deserialize the request body into a Product object with System.Text.Json
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var productPatch = JsonSerializer.Deserialize<JsonNode>(requestBody);
                if (productPatch == null)
                {
                    _logger.LogError("Invalid product data.");
                    return req.CreateResponse(HttpStatusCode.BadRequest);
                }
                // update the product with the patch data
                if (productPatch["Name"]?.GetValue<string>() is string name)
                {
                    product.Name = name;
                }
                if (productPatch["Price"]?.GetValue<decimal>() is decimal price)
                {
                    product.Price = price;
                }
                await _dbContext.SaveChangesAsync();

                var response = req.CreateResponse(HttpStatusCode.OK);
                // response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                await response.WriteAsJsonAsync(product);

                return response;
            }
            if (req.Method == "DELETE")
            {
                var product = await _dbContext.Products.FirstOrDefaultAsync(p => p.Id == id);
                if (product == null)
                {
                    return req.CreateResponse(HttpStatusCode.NotFound);
                }
                _dbContext.Products.Remove(product);
                await _dbContext.SaveChangesAsync();

                return req.CreateResponse(HttpStatusCode.NoContent);
            }

            return req.CreateResponse(HttpStatusCode.MethodNotAllowed);
        }
    }
}
