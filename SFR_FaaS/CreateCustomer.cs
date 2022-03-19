using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.Entity;

namespace SFR_FaaS
{
    public static class CreateCustomer
    {
        [FunctionName("CreateCustomer")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            CustomerDataContext customerDataContext = new();

            string name = req.Query["name"];
            string adress = req.Query["adress"];
            string iban = req.Query["iban"];

            string responseMessage;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name ??= data?.name;
            adress ??= data?.adress;
            iban ??= data?.iban;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(adress) || string.IsNullOrEmpty(iban))
            {
                responseMessage = "Missing Data for Customer Creation. Please Check the Input.";
                return new BadRequestObjectResult(responseMessage);
            }
            else
            {
                try
                {
                    Customer customer = new() { Name = name, Adress = adress, Iban = int.Parse(iban) };
                    customerDataContext.Entry(customer).State = customer.Id == 0 ? EntityState.Added : EntityState.Modified;
                    customerDataContext.SaveChanges();

                    responseMessage = $"Successfully created Customer with name: {name}.";
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
