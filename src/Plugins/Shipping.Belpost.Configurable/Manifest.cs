using Grand.Infrastructure;
using Grand.Infrastructure.Plugins;
using Shipping.Belpost.Configurable;

[assembly: PluginInfo(
    FriendlyName = "Shipping Belpost",
    Group = "Shipping rate",
    SystemName = BelpostShippingDefaults.ProviderSystemName,
    SupportedVersion = GrandVersion.SupportedPluginVersion,
    Author = "vdanilchik",
    Version = "1.00"
)]