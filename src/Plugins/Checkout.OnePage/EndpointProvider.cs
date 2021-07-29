using Grand.Domain.Data;
using Grand.Infrastructure.Configuration;
using Grand.Infrastructure.Endpoints;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Checkout.OnePage
{
    public partial class EndpointProvider : IEndpointProvider
    {
        public void RegisterEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            var pattern = "";
            if (DataSettingsManager.DatabaseIsInstalled())
            {
                var config = endpointRouteBuilder.ServiceProvider.GetRequiredService<AppConfig>();
                if (config.SeoFriendlyUrlsForLanguagesEnabled)
                {
                    pattern = $"{{language:lang={config.SeoFriendlyUrlsDefaultCode}}}/";
                }
            }

            endpointRouteBuilder.MapControllerRoute("Checkout",
                            pattern + "checkout/",
                            new { controller = "OnePageCheckout", action = "OnePageCheckoutStart" });

            endpointRouteBuilder.MapControllerRoute("CheckoutCompleted",
                            pattern + "checkout/completed/{orderId?}",
                            new { controller = "OnePageCheckout", action = "OnePageCheckoutCompleted" });

            endpointRouteBuilder.MapControllerRoute("CheckoutSelectPayment",
                            pattern + "checkout/SelectPaymentMethod/",
                            new { controller = "OnePageCheckout", action = "SelectPaymentMethod" });

            endpointRouteBuilder.MapControllerRoute("CheckoutConfirmOrder",
                            pattern + "checkout/ConfirmOrder/",
                            new { controller = "OnePageCheckout", action = "ConfirmOrder" });

            endpointRouteBuilder.MapControllerRoute("CheckoutCompleteRedirectionPayment",
                            pattern + "checkout/CompleteRedirectionPayment",
                            new { controller = "OnePageCheckout", action = "CompleteRedirectionPayment" });
        }
        public int Priority => 2;

    }
}
