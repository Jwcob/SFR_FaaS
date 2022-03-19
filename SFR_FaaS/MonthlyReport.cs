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
    public static class MonthlyReport
    {
        [FunctionName("MonthlyReport")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            CustomerDataContext customerDataContext = new();
            TransactionDataContext transactionDataContext = new();

            string iban = req.Query["iban"];
            string month = req.Query["month"];

            string responseMessage;

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            iban ??= data?.iban;
            month ??= data?.month;

            if (string.IsNullOrEmpty(iban) || string.IsNullOrEmpty(month))
            {
                responseMessage = "Missing Data for retrieving Monthly Report. Please Check the Input.";
                return new BadRequestObjectResult(responseMessage);
            }
            else
            {
                try
                {
                    int numIban = int.Parse(iban);
                    var customer = customerDataContext.Customers
                        .Where(c => c.Iban == numIban)
                        .FirstOrDefault();

                    var transactions = transactionDataContext.Transactions.ToList()
                        .Where(t => t.Debtor == numIban || t.Creditor == numIban)
                        .Where(t => t.ExecutionDate.ToString("MMMM") == month)
                        .ToList();

                    Report report = new()
                    {
                        Customer = customer,
                        Transactions = transactions,
                    };

                    var responseJson = JsonConvert.SerializeObject(report);

                    return new OkObjectResult(responseJson);
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
