using System.Text;
using Newtonsoft.Json;
using RestSharp;

namespace MyProjectIT15.Services
{
    public class PayMongoService
    {
        private readonly string _secretKey = "sk_test_vhfwqM8ZvUdi8fFtCg7bo2dn"; // replace with your real secret key
        private readonly string _baseUrl = "https://api.paymongo.com/v1";

        public async Task<string> CreateCheckoutSessionAsync(decimal amount, string description, string referenceNumber, string[] paymentMethods)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest("checkout_sessions", Method.Post);
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_secretKey}:"));

            request.AddHeader("Authorization", $"Basic {credentials}");
            request.AddHeader("Content-Type", "application/json");

            var body = new
            {
                data = new
                {
                    attributes = new
                    {
                        send_email_receipt = true,
                        show_description = true,
                        show_line_items = true,
                        description = description,
                        line_items = new[]
                        {
                    new {
                        name = "Payment",
                        amount = (int)(amount * 100), // in centavos
                        currency = "PHP",
                        quantity = 1
                    }
                },
                        payment_method_types = paymentMethods,
                        reference_number = referenceNumber,
                        success_url = $"https://localhost:7252/Payment/VerifyPayment?ref={referenceNumber}",
                        //cancel_url = "https://localhost:7252/Payment/Failed"
                        cancel_url = $"https://localhost:7252/Payment/Failed?reference={referenceNumber}"

                    }
                }
            };

            request.AddJsonBody(body);
            var response = await client.ExecuteAsync(request);
            return response.Content;
        }

        public async Task<string> GetCheckoutSessionStatusAsync(string checkoutSessionId)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest($"checkout_sessions/{checkoutSessionId}", Method.Get);
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_secretKey}:"));

            request.AddHeader("Authorization", $"Basic {credentials}");
            request.AddHeader("Content-Type", "application/json");

            var response = await client.ExecuteAsync(request);
            return response.Content;
        }

        public async Task<string> GetPaymentDetailsAsync(string paymentId)
        {
            var client = new RestClient(_baseUrl);
            var request = new RestRequest($"payments/{paymentId}", Method.Get);
            var credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_secretKey}:"));

            request.AddHeader("Authorization", $"Basic {credentials}");
            request.AddHeader("Content-Type", "application/json");

            var response = await client.ExecuteAsync(request);
            return response.Content;
        }

    }
}
