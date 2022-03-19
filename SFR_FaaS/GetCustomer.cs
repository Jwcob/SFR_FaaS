using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;

namespace SFR_FaaS
{
    public static class GetCustomer
    {
        [FunctionName("GetCustomer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            CustomerDataContext customerDataContext = new();

            string name = req.Query["name"];

            string responseMessage;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name ??= data?.name;

            if (string.IsNullOrEmpty(name))
            {
                responseMessage = "Missing Data for Customer Creation. Please Check the Input.";
                return new BadRequestObjectResult(responseMessage);
            }
            else
            {
                try
                {
                    var customer = customerDataContext.Customers
                            .Where(c => c.Name == name)
                            .FirstOrDefault();

                    responseMessage = customer is null ? 
                        "No Customer exists with given Name."
                        : $"Name: {customer.Name}, adress: {customer.Adress}, IBAN: {customer.Iban}";

                    return new OkObjectResult(responseMessage);
                }
                catch (Exception ex)
                {
                    responseMessage = ex.Message;
                    return new BadRequestObjectResult(responseMessage);
                }
            }
        }
    }
}
