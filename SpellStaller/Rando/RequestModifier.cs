using System;
using System.Collections.Generic;
using RandomizerMod.RC;
using static Mono.Security.X509.X509Stores;

namespace SpellStaller.Rando
{
    public class RequestModifier
    {
        public static void Hook()
        {
            //RequestBuilder.OnUpdate.Subscribe(-499, SetupItems);
            //RequestBuilder.OnUpdate.Subscribe(-499, DefinePools);
            RequestBuilder.OnUpdate.Subscribe(0, CloneSettings);
            RequestBuilder.OnUpdate.Subscribe(-498f, AddLocationAndItemToRandomization);
            RequestBuilder.OnUpdate.Subscribe(-499f, SetupItems);
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

        private static void SetupItems(RequestBuilder rb)
        {
            if (!SpellStaller.GS.Enabled)
            {
                return;
            }

            var names = new HashSet<string>();
            var charms = SpellStaller.Instance.Charms;
            foreach (var charm in charms)
            {
                var name = charm.Key.Replace(" ", "_");
                names.Add(name);
                rb.EditItemRequest(name, info =>
                {
                    info.getItemDef = () => new()
                    {
                        Name = name,
                        Pool = "Charm",
                        MajorItem = false,
                        PriceCap = 666
                    };
                });
            }

            rb.OnGetGroupFor.Subscribe(0f, (RequestBuilder rb, string item, RequestBuilder.ElementType type, out GroupBuilder gb) => {
                if (names.Contains(item) && (type == RequestBuilder.ElementType.Unknown || type == RequestBuilder.ElementType.Item))
                {
                    gb = rb.GetGroupFor("Shaman_Stone");
                    SpellStaller.Instance.Log($"Found a charm {item} and setting group {gb}");
                    return true;
                }
                gb = default;
                return false;
            });
        }

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
