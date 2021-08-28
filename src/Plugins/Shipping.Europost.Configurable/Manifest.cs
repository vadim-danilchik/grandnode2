using Grand.Infrastructure;
using Grand.Infrastructure.Plugins;
using Shipping.Europost.Configurable;

[assembly: PluginInfo(
    FriendlyName = "Shipping Europost",
    Group = "Shipping rate",
    SystemName = EuropostShippingDefaults.ProviderSystemName,
    SupportedVersion = GrandVersion.SupportedPluginVersion,
    Author = "vdanilchik",
    Version = "1.00"
)]