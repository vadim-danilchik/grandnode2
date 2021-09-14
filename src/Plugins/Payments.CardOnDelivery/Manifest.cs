using Grand.Infrastructure;
using Grand.Infrastructure.Plugins;
using Payments.CardOnDelivery;

[assembly: PluginInfo(
    FriendlyName = "Card On Delivery (COD)",
    Group = "Payment methods",
    SystemName = CardOnDeliveryPaymentDefaults.ProviderSystemName,
    SupportedVersion = GrandVersion.SupportedPluginVersion,
    Author = "vdanilchik",
    Version = "1.00"
)]