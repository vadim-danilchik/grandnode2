using Grand.Infrastructure;
using Grand.Infrastructure.Plugins;
using Payments.PostOnDelivery;

[assembly: PluginInfo(
    FriendlyName = "Post On Delivery (COD)",
    Group = "Payment methods",
    SystemName = PostOnDeliveryPaymentDefaults.ProviderSystemName,
    SupportedVersion = GrandVersion.SupportedPluginVersion,
    Author = "vdanilchik",
    Version = "1.00"
)]