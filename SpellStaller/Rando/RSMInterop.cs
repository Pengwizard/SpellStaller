using RandoSettingsManager;
using RandoSettingsManager.SettingsManagement;
using RandoSettingsManager.SettingsManagement.Versioning;

namespace SpellStaller.Rando
{
    internal static class RSMInterop
    {
        public static void Hook()
        {
            RandoSettingsManagerMod.Instance.RegisterConnection(new SpellStallerSettingsProxy());
        }
    }

    internal class SpellStallerSettingsProxy : RandoSettingsProxy<GlobalSettings, string>
    {
        public override string ModKey => SpellStaller.Instance.GetName();

        public override VersioningPolicy<string> VersioningPolicy { get; } = new EqualityVersioningPolicy<string>(SpellStaller.Instance.GetVersion());

        public override void ReceiveSettings(GlobalSettings settings)
        {
            settings ??= new();
            RandoMenuPage.Instance.spellStallerMEF.SetMenuValues(settings);
        }

        public override bool TryProvideSettings(out GlobalSettings settings)
        {
            settings = SpellStaller.GS;
            return settings.Enabled;
        }
    }
}