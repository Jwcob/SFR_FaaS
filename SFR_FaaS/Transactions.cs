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
    public static class Transactions
    {
        [FunctionName("Transactions")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

            TransactionDataContext transactionDataContext = new();

            string amount = req.Query["amount"];
            string description = req.Query["description"];
            string creditor = req.Query["creditor"];
            string debtor = req.Query["debtor"];

            string responseMessage;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            amount ??= data?.amount;
            description ??= data?.description;
            creditor ??= data?.creditor;
            debtor ??= data?.debtor;

            if (string.IsNullOrEmpty(amount) || string.IsNullOrEmpty(creditor) || string.IsNullOrEmpty(debtor))
            {
                responseMessage = "Missing Data for Transaction Creation. Please Check the Input.";
                return new BadRequestObjectResult(responseMessage);
            }
            else
            {
                try
                {
                    Transaction transaction = new()
                    {
                        Amount = amount,
                        Description = string.IsNullOrEmpty(description) ? "" : description,
                        Creditor = int.Parse(creditor),
                        Debtor = int.Parse(debtor),
                        ExecutionDate = DateTime.Now
                    };

                    transactionDataContext.Entry(transaction).State = transaction.Id == 0 ? EntityState.Added : EntityState.Modified;
                    transactionDataContext.SaveChanges();

                    responseMessage = $"Created Transaction with Amount: {amount} for Creditor: {creditor} and Debtor: {debtor}.";

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
