using MenuChanger;
using MenuChanger.Extensions;
using MenuChanger.MenuElements;
using MenuChanger.MenuPanels;
using RandomizerMod.Menu;
using static RandomizerMod.Localization;

namespace SpellStaller.Rando
{
    public class RandoMenuPage
    {
        internal MenuPage SpellStallerRandoPage;
        internal MenuElementFactory<GlobalSettings> spellStallerMEF;
        internal VerticalItemPanel spellStallerVIP;

        internal SmallButton JumpToSpellStallerButton;

        internal static RandoMenuPage Instance { get; private set; }

        public static void OnExitMenu()
        {
            Instance = null;
        }

        public static void Hook()
        {
            RandomizerMenuAPI.AddMenuPage(ConstructMenu, HandleButton);
            MenuChangerMod.OnExitMainMenu += OnExitMenu;
        }

        private static bool HandleButton(MenuPage landingPage, out SmallButton button)
        {
            button = Instance.JumpToSpellStallerButton;
            return true;
        }

        private void SetTopLevelButtonColor()
        {
            if (JumpToSpellStallerButton != null)
            {
                JumpToSpellStallerButton.Text.color = SpellStaller.GS.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
            }
        }

        private static void ConstructMenu(MenuPage landingPage)
        {
            Instance = new(landingPage);
        }

        private RandoMenuPage(MenuPage landingPage)
        {
            SpellStallerRandoPage = new MenuPage(Localize("Spell Staller"), landingPage);
            spellStallerMEF = new(SpellStallerRandoPage, SpellStaller.GS);
            spellStallerVIP = new(SpellStallerRandoPage, new(0, 300), 75f, true, spellStallerMEF.Elements);
            Localize(spellStallerMEF);
            foreach (IValueElement e in spellStallerMEF.Elements)
            {
                e.SelfChanged += obj => SetTopLevelButtonColor();
            }

            JumpToSpellStallerButton = new(landingPage, Localize("Spell Staller"));
            JumpToSpellStallerButton.AddHideAndShowEvent(landingPage, SpellStallerRandoPage);
            SetTopLevelButtonColor();
        }
    }
}