using Grand.Business.Common.Interfaces.Logging;
using Newtonsoft.Json;
using Payments.WebpayCard.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Payments.WebpayCard.Services
{
    public class WebpayHttpClient : IWebpayHttpClient
    {
        private readonly HttpClient _client;
        private readonly WebpayCardPaymentSettings _WebpayCardPaymentSettings;
        private readonly ILogger _logger;
        public WebpayHttpClient(HttpClient client, WebpayCardPaymentSettings WebpayCardPaymentSettings, ILogger logger)
        {
            _client = client;
            _WebpayCardPaymentSettings = WebpayCardPaymentSettings;
            _logger = logger;
        }

        private string GetWebpayPaymentUrl()
        {
            return _WebpayCardPaymentSettings.UseSandbox ?
                WebpayHelper.WebpayUrlSandbox :
                WebpayHelper.WebpayUrl;
        }

        public virtual async Task<string> GetPaymentUrl(PaymentRequestModel values)
        {
            var formContent = new StringContent(
               JsonConvert.SerializeObject(values, Formatting.Indented),
               Encoding.UTF8, 
               "application/json");

            var response = await _client.PostAsync(GetWebpayPaymentUrl(), formContent);
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch(Exception ex)
            {
                await _logger.InsertLog(Grand.Domain.Logging.LogLevel.Error, "Get payment url", ex.Message);
            }
            
            var content = await response.Content.ReadAsStringAsync();

            var paymentResult = JsonConvert.DeserializeObject<PaymentRedirectModel>(content);

            return paymentResult.data.redirectUrl;
        }
    }
}
