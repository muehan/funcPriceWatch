using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;
using System;
using System.Net;
using System.Text.Json;

namespace functions
{
    public static class DailyReport
    {
        [FunctionName("DailyReport")]
        public static IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [SendGrid(ApiKey = "SendGridApiKey")]ICollector<SendGridMessage> sender,
            ILogger log)
        {
            log.LogInformation($"DailyReport function triggered");

            var client = new WebClient();
            var productText = client.DownloadString(Environment.GetEnvironmentVariable("ProductUrl"));
            var products = JsonSerializer.Deserialize<ProductModel[]>(productText);

            var priceText = client.DownloadString(Environment.GetEnvironmentVariable("PriceUrl"));
            var prices = JsonSerializer.Deserialize<PriceModel[]>(priceText);

            var message = new SendGridMessage();
            message.From = new EmailAddress(Environment.GetEnvironmentVariable("EmailSender"));
            message.AddTo(Environment.GetEnvironmentVariable("EmailReceiver"));
            message.Subject = $"Daily Pricewatch Report {DateTime.Now.ToShortDateString()}";
            message.HtmlContent = "Thank you for your order";

            sender.Add(message);

            client.Dispose();

            return (ActionResult) new OkObjectResult($"Hello there");
        }
    }

    public class ProductModel {
        public Guid Id { get; set; }
        public Guid Producttypeid { get; set; }
        public string Productid { get; set; }
        public string ProductidAsString { get; set; }
        public string Name { get; set; }
        public string Fullname { get; set; }
        public string SimpleName { get; set; }
        public DateTime Date { get; set; }
    }

    public class PriceModel {
        public Guid Id { get; set; }
    }
}
