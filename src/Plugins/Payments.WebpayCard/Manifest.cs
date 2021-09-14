using Grand.Infrastructure;
using Grand.Infrastructure.Plugins;
using Payments.WebpayCard;

[assembly: PluginInfo(
    FriendlyName = "Webpay Card",
    Group = "Payment methods",
    SystemName = WebpayCardPaymentDefaults.ProviderSystemName,
    SupportedVersion = GrandVersion.SupportedPluginVersion,
    Author = "vdanilchik",
    Version = "1.00"
)]