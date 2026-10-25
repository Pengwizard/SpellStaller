using SFCore;
using UnityEngine;

namespace SpellStaller.Charms
{

    public class GorbsMemento : EasyCharm
    {
        private static string charmSprite = "Gorb'sMemento";
        public static string charmName = "Gorb's Memento";
        // TODO: After release make this description better?
        private static string charmDescription = "A circlet inscribed with wisdoms from The Great Mind.\r\n\r\n\"If one aims to fully ascend, they must first remove DIVE as an option.\"- The Great Mind";
        private static int defaultCost = 0;

        public GorbsMemento() : base()
        {
            ApplyHooks();
        }

        public void ApplyHooks()
        {
            On.PlayerData.GetInt += CheckForGorbWisdom;
        }

        private int CheckForGorbWisdom(On.PlayerData.orig_GetInt orig, PlayerData self, string intName)
        {
            if (intName == "quakeLevel" && IsEquipped)
            {
                return 0;
            }

            return orig(self, intName);
        }

        protected override Sprite GetSpriteInternal()
        {
            return new EmbeddedSprite(GorbsMemento.charmSprite).Value;
        }

        protected override string GetName()
        {
            return GorbsMemento.charmName;
        }

        protected override string GetDescription()
        {
            return GorbsMemento.charmDescription;
        }

        protected override int GetCharmCost()
        {
            return GorbsMemento.defaultCost;
        }
    }
}
