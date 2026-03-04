using Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpellStaller
{
    public partial class SpellStaller : IMenuMod
    {
        public bool ToggleButtonInsideMenu => false;

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? toggleButtonEntry)
        {
            List<IMenuMod.MenuEntry> entries = new List<IMenuMod.MenuEntry>();

            entries.Add(new IMenuMod.MenuEntry
            {
                Name = "Stall Ball",
                Description = "Toggle whether to make fireballs not move",
                Values = new string[] { "True", "False" },
                Saver = (i) => GS.Enabled = i == 0,
                Loader = () => GS.Enabled ? 0 : 1
            });
            entries.Add(new IMenuMod.MenuEntry
            {
                Name = "Enable ball damage",
                Description = "If set, fireballs do damage",
                Values = new string[] { "True", "False" },
                Saver = (i) => GS.ShouldDamage = i == 0,
                Loader = () => GS.ShouldDamage ? 0 : 1
            });
            entries.Add(new IMenuMod.MenuEntry
            {
                Name = "Light Bridge",
                Description = "If true, fireball is actually a light bridge",
                Values = new string[] { "True", "False" },
                Saver = (i) => GS.LightBridge = i == 0,
                Loader = () => GS.LightBridge ? 0 : 1
            });
            return entries;
        }
    }
}
