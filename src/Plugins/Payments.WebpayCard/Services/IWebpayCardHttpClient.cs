using Payments.WebpayCard.Models;
using System.Threading.Tasks;

namespace Payments.WebpayCard.Services
{
    public interface IWebpayHttpClient
    {
        Task<string> GetPaymentUrl(PaymentRequestModel values);
    }
}
