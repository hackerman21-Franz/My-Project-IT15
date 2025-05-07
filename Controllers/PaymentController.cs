using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProjectIT15.Migrations;
using MyProjectIT15.Models;
using MyProjectIT15.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MyProjectIT15.Controllers
{
    public class PaymentController : Controller
    {
        private readonly PayMongoService _payMongoService;
        private readonly ApplicationDBContext _context;
        private readonly UserManager<User> _userManager;
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        private const string PayMongoSecretKey = "sk_test_vhfwqM8ZvUdi8fFtCg7bo2dn";
        private const string PayMongoCheckoutUrl = "https://api.paymongo.com/v1/checkout_sessions";

        public PaymentController(UserManager<User> userManager, PayMongoService payMongoService, IConfiguration configuration, ApplicationDBContext context)
        {
            _payMongoService = payMongoService;
            _userManager = userManager;
            _context = context;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                Convert.ToBase64String(Encoding.UTF8.GetBytes($"{PayMongoSecretKey}:")));
            _configuration = configuration;
        }

        //[HttpGet("checkout")]
        //public async Task<IActionResult> Checkout(int billingId, decimal amount)
        //{

        //    var user = await _userManager.GetUserAsync(User);

        //    var billingDetails = await _context.Billings.FindAsync(billingId);
        //    if (billingDetails == null)
        //    {
        //        TempData["ShowError"] = true;
        //        TempData["Error"] = "Billing not found!";
        //        return RedirectToAction("UserBilling", "Meter");
        //    }

        //    decimal totalPaid = await _context.Payments
        //        .Where(p => p.BillingId == billingId) /*&& p.UserId == user.Id)*/
        //        .SumAsync(p => p.TotalPaid);


        //    string referenceNumber = Guid.NewGuid().ToString();


        //        var payment = new Payment
        //        {
        //            BillingId = billingId,
        //            UserId = user?.Id,
        //            TotalPaid = amount,
        //            ReferenceNumber = referenceNumber,
        //            PaymentMethod = "Not Specified",
        //            Status = "Pending",
        //            CreatedAt = DateTime.UtcNow
        //        };

        //        _context.Payments.Add(payment);


        //    await _context.SaveChangesAsync();

        //    var requestData = new
        //    {
        //        data = new
        //        {
        //            attributes = new
        //            {
        //                description = $"Payment for {billingDetails.ReadingDate}",
        //                line_items = new[] {
        //            new {
        //                currency = "PHP",
        //                amount = (int)(amount * 100),
        //                name = billingDetails.ReadingDate,
        //                quantity = 1
        //            }
        //        },
        //                payment_method_types = new[] { "card", "gcash", "grab_pay" },
        //                send_email_receipt = true,
        //                show_description = true,
        //                success_url = $"https://localhost:7252/Payment/PaymentCallback?transactionId={referenceNumber}&billingId={billingId}&success=true",
        //                cancel_url = $"https://localhost:7252/Payment/PaymentCallback?transactionId={referenceNumber}&billingId={billingId}&success=false"
        //            }
        //        }
        //    };

        //    var jsonRequest = JsonSerializer.Serialize(requestData);
        //    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

        //    HttpResponseMessage response = await _httpClient.PostAsync(PayMongoCheckoutUrl, content);

        //    if (!response.IsSuccessStatusCode)
        //    {

        //        TempData["ErrorMessage"] = "Failed to create PayMongo checkout session.";
        //        return RedirectToAction("UserBillings", "Meter");
        //    }

        //    var jsonResponse = await response.Content.ReadAsStringAsync();
        //    var checkoutResponse = JsonSerializer.Deserialize<PayMongoResponse>(jsonResponse);

        //    string checkoutUrl = checkoutResponse?.data?.attributes?.checkout_url;

        //    return Redirect(checkoutUrl);
        //}

        //[HttpGet("PaymentCallback")]
        //public async Task<IActionResult> PaymentCallback(string transactionId, int billingId, bool success)
        //{
        //    var payment = await _context.Payments
        //        .Where(p => p.ReferenceNumber == transactionId)
        //        .FirstOrDefaultAsync();

        //    var billingDetails = await _context.Billings.FindAsync(billingId);
        //    if (billingDetails == null) return NotFound("Event not found");
        //    if (payment == null || billingDetails == null) return NotFound("Transaction or event not found");

        //    if (success)
        //    {
        //        payment.ReferenceNumber = transactionId;

        //        decimal totalPaid = await _context.Payments
        //            .Where(p => p.BillingId == billingId)
        //            .SumAsync(p => p.TotalPaid);


        //            billingDetails.Status = "Paid"; // Full payment

        //        await _context.SaveChangesAsync();
        //        TempData["SuccessMessage"] = "Payment successful!";
        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = "Payment failed. Please try again.";
        //    }

        //    return RedirectToAction("UserBillings", "Meter");
        //}

        //[HttpPost("PayMongoWebhook")]
        //public async Task<IActionResult> PayMongoWebhook([FromBody] JsonDocument payload)
        //{
        //    try
        //    {
        //        var root = payload.RootElement;
        //        var eventType = root.GetProperty("data").GetProperty("attributes").GetProperty("event").GetString();

        //        if (eventType == "payment.paid")
        //        {
        //            var paymentData = root.GetProperty("data").GetProperty("attributes");

        //            string transactionId = root.GetProperty("data").GetProperty("id").GetString();
        //            decimal amount = paymentData.GetProperty("amount").GetDecimal() / 100; // Convert from centavos
        //            string status = paymentData.GetProperty("status").GetString();
        //            string referenceNumber = paymentData.GetProperty("external_reference_number").GetString(); // FIXED
        //            string paymentMethod = paymentData.GetProperty("payment_method").GetProperty("type").GetString(); // FIXED
        //            DateTime paymentDate = DateTime.UtcNow;

        //            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.ReferenceNumber == referenceNumber);
        //            if (payment != null)
        //            {
        //                // Update existing payment record
        //                payment.TotalPaid += amount;
        //                payment.Status = status;
        //                payment.PaymentMethod = paymentMethod;
        //                payment.CreatedAt = paymentDate;
        //            }
        //            else
        //            {
        //                // Create a new payment record
        //                _context.Payments.Add(new Payment
        //                {
        //                    ReferenceNumber = referenceNumber,
        //                    TotalPaid = amount,
        //                    Status = status,
        //                    PaymentMethod = paymentMethod,
        //                    CreatedAt = paymentDate
        //                });
        //            }
        //            await _context.SaveChangesAsync();
        //            return Ok(new { message = "Payment recorded successfully" });
        //        }

        //        return BadRequest(new { message = "Unhandled event type" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { error = ex.Message });
        //    }
        //}



        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(decimal amount, int billingId)
        {
            var user = await _userManager.GetUserAsync(User);
            var referenceNumber = $"REF-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            var result = await _payMongoService.CreateCheckoutSessionAsync(
                            amount,
                            "Billing Payment",
                            referenceNumber,
                            new[] { "gcash", "card", "grab_pay", "paymaya" } // Add more as needed
                            );

            var json = JObject.Parse(result);
            var checkoutUrl = json["data"]?["attributes"]?["checkout_url"]?.ToString();
            var checkoutSessionId = json["data"]?["id"]?.ToString();  // Extract checkoutSessionId

            // Store the payment in your database
            var payment = new Payment
            {
                BillingId = billingId,
                UserId = user?.Id,
                TotalPaid = amount,
                Status = "pending",
                CreatedAt = DateTime.UtcNow,
                ReferenceNumber = referenceNumber,
                PaymentMethod = "not specified",
                CheckoutSessionId = checkoutSessionId
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Redirect user to PayMongo's checkout page
            return Redirect(checkoutUrl);
        }

        //[HttpGet]
        //public async Task<IActionResult> VerifyPayment([FromQuery(Name = "ref")] string referenceNumber)
        //{
        //    var payment = await _context.Payments.FirstOrDefaultAsync(p => p.ReferenceNumber == referenceNumber);
        //    if (payment == null)
        //    {
        //        return Content("Payment not found.");
        //    }

        //    var result = await _payMongoService.GetCheckoutSessionStatusAsync(payment.CheckoutSessionId);
        //    var json = JObject.Parse(result);

        //    var status = json["data"]?["attributes"]?["status"]?.ToString();
        //    if (string.IsNullOrEmpty(status))
        //    {
        //        return Content("Unable to retrieve payment status.");
        //    }

        //    // Modify the condition here to check for both 'paid' and 'active'
        //    if ((status == "paid" || status == "active") && payment.Status != "paid")
        //    {
        //        payment.Status = "paid";  // Mark as paid
        //        payment.LastUpdatedAt = DateTime.UtcNow;  // Update the timestamp
        //        _context.Update(payment);
        //        await _context.SaveChangesAsync();
        //        return Content($"Payment verified and updated. New status: {payment.Status}");
        //    }
        //    else
        //    {
        //        payment.Status = "failed";  // Mark as paid
        //        payment.LastUpdatedAt = DateTime.UtcNow;  // Update the timestamp
        //        _context.Update(payment);
        //        await _context.SaveChangesAsync();
        //        return Content($"Payment Not verified and updated. New status: {payment.Status}");
        //    }

        //    return Content($"Payment status: {status} (no update needed)");
        //}

        [HttpGet]
        public async Task<IActionResult> VerifyPayment([FromQuery(Name = "ref")] string referenceNumber, [FromQuery(Name = "status")] string status)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.ReferenceNumber == referenceNumber);
            if (payment == null)
            {
                return Content("Payment not found.");
            }

            // If payment is not verified or canceled
            if (status == "failed")
            {
                // Update the status of the payment to failed
                payment.Status = "failed";
                payment.LastUpdatedAt = DateTime.UtcNow; // Update the timestamp
                _context.Update(payment);
                await _context.SaveChangesAsync();

                return Content($"Payment not verified and updated. New status: {payment.Status}");
            }

            // For successful payment or active payment that needs to be confirmed
            var result = await _payMongoService.GetCheckoutSessionStatusAsync(payment.CheckoutSessionId);
            var json = JObject.Parse(result);

            var paymentStatus = json["data"]?["attributes"]?["status"]?.ToString();
            
            if (string.IsNullOrEmpty(paymentStatus))
            {
                return Content("Unable to retrieve payment status.");
            }

            // Check if payment is paid or active
            if ((paymentStatus == "paid" || paymentStatus == "active") && payment.Status != "paid")
            {
                // Extract the paymentId from the session response
                var paymentId = json["data"]?["attributes"]?["payments"]?[0]?["id"]?.ToString();
                if (string.IsNullOrEmpty(paymentId))
                {
                    return Content("Payment ID not found.");
                }

                // Fetch payment details using the paymentId
                var paymentDetailsJson = await _payMongoService.GetPaymentDetailsAsync(paymentId);
                Console.WriteLine("Payment Details JSON: " + paymentDetailsJson);  // Log the full response for debugging

                var paymentDetails = JObject.Parse(paymentDetailsJson);

                // Extract the payment method from the payment details response
                var actualMethod = paymentDetails["data"]?["attributes"]?["payment_method"]?["type"]?.ToString();

                // If we cannot find the payment method, log it as "unknown"
                if (string.IsNullOrEmpty(actualMethod))
                {
                    Console.WriteLine("Payment method is still not found.");
                    actualMethod = "gcash";  // Use "unknown" if we cannot find the method
                }


                payment.PaymentMethod = actualMethod;
                payment.Status = "paid"; // Update status to paid
                payment.LastUpdatedAt = DateTime.UtcNow; // Update timestamp

                _context.Update(payment);
                await _context.SaveChangesAsync();
                //return Content($"Payment verified and updated. New status: {payment.Status}");

                var billing = await _context.Billings.FirstOrDefaultAsync(b => b.Id == payment.BillingId);
                if (billing != null)
                {
                    billing.Status = "Paid";
                    _context.Billings.Update(billing);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction("Success","Payment");

            }

            return Content($"Payment status: {paymentStatus} (no update needed)");
        }

        public async Task<IActionResult> Failed(string reference)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.ReferenceNumber == reference);
            if (payment == null) return NotFound("Payment not found.");

            var payMongoService = new PayMongoService();
            var sessionDataJson = await payMongoService.GetCheckoutSessionStatusAsync(payment.CheckoutSessionId);

            dynamic sessionData = JsonConvert.DeserializeObject(sessionDataJson);
            string status = sessionData.data.attributes.status;

            if (status != "paid" && payment.Status != "paid")
            {
                payment.Status = "failed";
                payment.LastUpdatedAt = DateTime.UtcNow;
                _context.Update(payment);
                await _context.SaveChangesAsync();
            }

            return View(); // Or Content("Payment failed");
        }





        //[HttpPost("simulate-payment-success")]
        //public async Task<IActionResult> SimulatePaymentSuccess(string referenceNumber)
        //{
        //    Console.WriteLine("SimulatePaymentSuccess method triggered.");
        //    var payment = _context.Payments.FirstOrDefault(p => p.ReferenceNumber == referenceNumber);
        //    if (payment == null)
        //    {
        //        Console.WriteLine("Payment not found.");
        //        return NotFound("Payment not found.");
        //    }

        //    Console.WriteLine("Payment found. Updating status to 'paid'...");


        //    payment.Status = "paid";
        //    payment.LastWebhookEvent = "manual.test";
        //    payment.LastUpdatedAt = DateTime.UtcNow;

        //    _context.Update(payment);
        //    await _context.SaveChangesAsync();
        //    return Ok("Payment marked as paid.");
        //}

        public IActionResult Success()
        {
            return View(); // Show a success message
        }

        //public IActionResult Failed()
        //{
        //    return View(); // Show an error or retry option
        //}

        private class PayMongoResponse
        {
            public Data data { get; set; }
        }

        private class Data
        {
            public Attributes attributes { get; set; }
        }

        private class Attributes
        {
            public string checkout_url { get; set; }
        }
    }
}
