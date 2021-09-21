using Grand.Infrastructure;
using Grand.Infrastructure.Plugins;
using Shipping.Courier.Configurable;

[assembly: PluginInfo(
    FriendlyName = "Shipping Courier",
    Group = "Shipping rate",
    SystemName = CourierShippingDefaults.ProviderSystemName,
    SupportedVersion = GrandVersion.SupportedPluginVersion,
    Author = "vdanilchik",
    Version = "1.00"
)]