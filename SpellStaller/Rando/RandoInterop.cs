using ItemChanger;
using ItemChanger.Tags;
using ItemChanger.UIDefs;
using Modding;
using RandomizerMod.RC;

namespace SpellStaller.Rando
{
    internal static class RandoInterop
    {
        public static void Hook()
        {
            RandoMenuPage.Hook();
            RequestModifier.Hook();
            LogicAdder.Hook();

            //RandoController.OnExportCompleted += AddRgbModule;

            DefineItems();

            if (ModHooks.GetMod("RandoSettingsManager") is Mod)
            {
                RSMInterop.Hook();
            }
        }

        //private static void AddRgbModule(RandoController controller)
        //{
        //    if (!SpellStaller.GS.Enabled)
        //    {
        //        return;
        //    }

        //    ItemChangerMod.Modules.GetOrAdd<RgbModule>();
        //}

        public static void DefineItems()
        {
            var charms = SpellStaller.Instance.Charms;
            foreach (var entry in charms)
            {
                var sprite = new EmbeddedSprite(entry.Key.Replace(" ", ""));
                Finder.DefineCustomItem(new ItemChanger.Items.CharmItem()
                {
                    charmNum = entry.Value.Id,
                    name = entry.Key.Replace(" ", "_"),
                    UIDef = new MsgUIDef()
                    {
                        name = new LanguageString("UI", $"CHARM_NAME_{entry.Value.Id}"),
                        shopDesc = new LanguageString("UI", $"CHARM_DESC_{entry.Value.Id}"),
                        sprite = sprite
                    }
                });
            }
        }

        public static string Clean(string name)
        {
            return name.Replace("-", " - ");
        }

        public static InteropTag AddTag(TaggableObject obj)
        {
            InteropTag tag = obj.GetOrAddTag<InteropTag>();
            tag.Message = "RandoSupplementalMetadata";
            tag.Properties["ModSource"] = SpellStaller.Instance.GetName();

            return tag;
        }
    }
}
