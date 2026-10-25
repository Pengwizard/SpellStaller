using RandomizerMod.RC;

namespace SpellStaller.Rando
{
    public class RequestModifier
    {
        public static void Hook()
        {
            //RequestBuilder.OnUpdate.Subscribe(-499, SetupItems);
            //RequestBuilder.OnUpdate.Subscribe(-499, DefinePools);
            RequestBuilder.OnUpdate.Subscribe(0, CloneSettings);

            RequestBuilder.OnUpdate.Subscribe(-499f, AddLocationAndItemToRandomization);
        }

        private static void AddLocationAndItemToRandomization(RequestBuilder rb)
        {
            if (!SpellStaller.GS.Enabled)
            {
                return;
            }

            var charms = SpellStaller.Instance.Charms;
            foreach (var entry in charms)
            {
                rb.AddItemByName(entry.Key.Replace(" ", "_"));
            }
        }

        //private static void SetupItems(RequestBuilder rb)
        //{
        //    if (!SpellStaller.GS.Enabled)
        //    {
        //        return;
        //    }

        //    foreach (RGB rgb in Consts.colorValues)
        //    {
        //        rb.EditItemRequest(Consts.NameByValue(rgb), info =>
        //        {
        //            info.getItemDef = () => new RandomizerMod.RandomizerData.ItemDef()
        //            {
        //                Name = Consts.NameByValue(rgb),
        //                Pool = "RGB",
        //                MajorItem = false,
        //                PriceCap = 255
        //            };
        //        });

        //        rb.AddItemByName(Consts.NameByValue(rgb), SpellStallerMod.GS.PartsPerColor);
        //    }
        //}

        //private static void DefinePools(RequestBuilder rb)
        //{
        //    if (!SpellStallerMod.GS.Enabled)
        //    {
        //        return;
        //    }
            
        //    ItemGroupBuilder group = null;
        //    string label = RBConsts.SplitGroupPrefix + "Rgb";
        //    foreach (ItemGroupBuilder igb in rb.EnumerateItemGroups())
        //    {
        //        if (igb.label == label)
        //        {
        //            group = igb;
        //            break;
        //        }
        //    }
        //    group ??= rb.MainItemStage.AddItemGroup(label);

        //    rb.OnGetGroupFor.Subscribe(0.01f, ResolveRgbGroup);
        //    bool ResolveRgbGroup(RequestBuilder rb, string item, RequestBuilder.ElementType type, out GroupBuilder gb)
        //    {
        //        gb = default;
        //        return false;
        //    }
        //}

        private static void CloneSettings(RequestBuilder rb)
        {
            LocalSettings l = SpellStaller.LS;
            GlobalSettings g = SpellStaller.GS;
            //l.perColorCount = g.PartsPerColor;
        }
    }
}
