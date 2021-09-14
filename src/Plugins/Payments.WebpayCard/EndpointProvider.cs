using Grand.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Payments.WebpayCard
{
    public partial class EndpointProvider : IEndpointProvider
    {
        public void RegisterEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //success
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.WebpayCard.SuccessPayment",
                 "Plugins/PaymentWebpayCard/SuccessPayment",
                 new { controller = "PaymentWebpayCard", action = "SuccessPayment" }
            );
            //notify
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.WebpayCard.NotifyPayment",
                 "Plugins/PaymentWebpayCard/NotifyPayment",
                 new { controller = "PaymentWebpayCard", action = "NotifyPayment" }
            );
            //cancel
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.WebpayCard.CancelPayment",
                 "Plugins/PaymentWebpayCard/CancelPayment",
                 new { controller = "PaymentWebpayCard", action = "CancelPayment" }
            );
        }
        public int Priority => 0;

    }
}
