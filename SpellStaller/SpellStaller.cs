using System;
using System.Collections.Generic;
using System.Linq;
using DebugMod;
using ItemChanger.Internal;
using Modding;
using SFCore;
using SpellStaller.Charms;
using SpellStaller.Rando;

namespace SpellStaller
{
    public class SaveSettings
    {
        public Dictionary<string, EasyCharmState> Charms;
    }

    public partial class SpellStaller : Mod, IGlobalSettings<Rando.GlobalSettings>, ILocalSettings<Rando.LocalSettings>
    {
        private static SpellStaller _instance;
        public static Rando.GlobalSettings GS { get; set; } = new Rando.GlobalSettings();
        public static Rando.LocalSettings LS { get; set; } = new();


        public Dictionary<string, EasyCharm> Charms = new Dictionary<string, EasyCharm>{
            {SpellStabilizer.charmName, new SpellStabilizer()},
            {DreamSifter.charmName, new DreamSifter()},
            {GorbsMemento.charmName, new GorbsMemento()}
        };

        public EasyCharm GetCharmByName(string charmName)
        {
            return Charms[charmName];
        }

        private void CountOurCharms(On.PlayerData.orig_CountCharms orig, PlayerData self)
        {
            orig(self);
            self.SetInt("charmsOwned", self.GetInt("charmsOwned") + Charms.Count(c => c.Value.GotCharm));
        }

        public static SpellStaller Instance
        {
            get
            {
                if (_instance == null)
                {
                    throw new InvalidOperationException($"An instance of {nameof(SpellStaller)} was never constructed");
                }
                return _instance;
            }
        }

        public override string GetVersion() => GetType().Assembly.GetName().Version.ToString();

        public SpellStaller() : base("SpellStaller")
        {
            _instance = this;
        }

        public override void Initialize()
        {
            Log("Initializing");

            // put additional initialization logic here

            //Charms 

            //On.HeroController.Update += HeroController_Update;

            bool rando = ModHooks.GetMod("Randomizer 4") is Mod;
            bool ic = ModHooks.GetMod("ItemChangerMod") is Mod;

            RandoInterop.Hook();

            On.PlayerData.CountCharms += CountOurCharms;

            if (ModHooks.GetMod("DebugMod") != null)
            {
                try
                {
                    BindableFunctions.OnGiveAllCharms += () => {
                        foreach (var c in Charms)
                        {
                            c.Value.GiveCharm();
                        }

                        PlayerData.instance.CountCharms();
                    };
                    BindableFunctions.OnRemoveAllCharms += () => {
                        foreach (var c in Charms)
                        {
                            c.Value.TakeCharm();
                        }

                        PlayerData.instance.CountCharms();
                    };
                }
                catch (MissingMethodException)
                {
                    LogWarn("DebugMod hooks for giving and removing charms are not available; those commands will not work on Transcendence charms. Updating DebugMod may fix this.");
                }
            }

            Log("Initialized");
        }


        public Rando.GlobalSettings OnSaveGlobal() => GS;
        public void OnLoadGlobal(Rando.GlobalSettings s) => GS = s;

        //public void OnLoadLocal(Rando.LocalSettings s) => LS = s;
        //public Rando.LocalSettings OnSaveLocal() => LS;

        public void GiveCharm(string charmName)
        {
            Charms[charmName].GiveCharm(true);
        }

        public void OnLoadLocal(Rando.LocalSettings s)
        {
            LS = s;
            if (s.Charms != null)
            {
                foreach (var kvp in s.Charms)
                {
                    if (Charms.TryGetValue(kvp.Key, out EasyCharm m))
                    {
                        m.RestoreCharmState(kvp.Value);
                    }
                }
            }
        }

        public Rando.LocalSettings OnSaveLocal()
        {
            LS.Charms = new Dictionary<string, EasyCharmState>();
            foreach (var kvp in Charms)
            {
                if (Charms.TryGetValue(kvp.Key, out EasyCharm m))
                {
                    LS.Charms[kvp.Key] = m.GetCharmState();
                }
            }
            return LS;
        }
    }
}
