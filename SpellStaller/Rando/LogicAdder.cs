using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerCore.StringItems;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace SpellStaller.Rando
{
    public static class LogicAdder
    {
        public static void Hook()
        {
            RCData.RuntimeLogicOverride.Subscribe(50, AddItemAndLogic);
        }

        private static void AddItemAndLogic(GenerationSettings gs, LogicManagerBuilder lmb)
        {
            if (!SpellStaller.GS.Enabled)
            {
                return;
            }

            var charms = SpellStaller.Instance.Charms;
            foreach (var entry in charms)
            {
                var sk = entry.Key.Replace(" ", "_");
                Term term = lmb.GetOrAddTerm(sk, TermType.Int);
                lmb.AddItem(new StringItemTemplate(sk, $"`{sk}<1` => CHARMS++ >> {sk}++"));
            }
        }
    }
}
