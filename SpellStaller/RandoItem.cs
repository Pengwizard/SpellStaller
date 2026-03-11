using ItemChanger;
using ItemChanger.Locations;
using ItemChanger.UIDefs;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod.RC;
using RandomizerMod.Settings;
using SFCore;
using SpellStaller.Charms;
using UnityEngine;

namespace SpellStaller
{
    public class RandoItem
    {
        // call this in Initialize of your mod after checking if ItemChanger is available
        public static void InitRandoConnection()
        {
            var charms = SpellStaller.Instance.Charms;
            foreach (var entry in charms) {
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

            // Add item to rando (and define how it impacts logic) & the logic required to consider the location "reachable"
            RCData.RuntimeLogicOverride.Subscribe(50f, AddItemAndLogic);

            // Add the item & location to RequestBuilder the thing that makes it do the randomizing
            RequestBuilder.OnUpdate.Subscribe(0.3f, AddLocationAndItemToRandomization);
        }

        private static void AddLocationAndItemToRandomization(RequestBuilder rb)
        {
            var charms = SpellStaller.Instance.Charms;
            foreach (var entry in charms)
            {
                rb.AddItemByName(entry.Key.Replace(" ", "_"));
            }

        }

        private static void AddItemAndLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            var charms = SpellStaller.Instance.Charms;
            foreach (var entry in charms)
            {
                lmb.AddItem(new EmptyItem(entry.Key.Replace(" ", "_")));
            }
        }
    }
}
