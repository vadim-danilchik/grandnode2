using Grand.Infrastructure;
using Grand.Infrastructure.Plugins;
using Shipping.ShippingPickupPoint;

[assembly: PluginInfo(
    FriendlyName = "Shipping Pickup Point",
    Group = "Shipping rate",
    SystemName = ShippingPickupPointRateDefaults.ProviderSystemName,
    SupportedVersion = GrandVersion.SupportedPluginVersion,
    Author = "vdanilchik",
    Version = "1.00"
)]