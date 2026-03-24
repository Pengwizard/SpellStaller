using ItemChanger.Internal;
using Modding;
using SFCore;
using SpellStaller.Charms;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SpellStaller
{
    public class SaveSettings
    {
        public Dictionary<string, EasyCharmState> Charms;
    }

    public partial class SpellStaller : Mod, IGlobalSettings<GlobalSettings>, ILocalSettings<SaveSettings>
    {
        private static SpellStaller _instance;
        public static GlobalSettings GS { get; set; } = new GlobalSettings();

        internal SaveSettings localSettings = new SaveSettings();

        public Dictionary<string, EasyCharm> Charms = new Dictionary<string, EasyCharm>{
            {SpellStabilizer.charmName, new SpellStabilizer()},
            {DreamSifter.charmName, new DreamSifter()},
            {SoulShuttle.charmName, new SoulShuttle()}
        };

        public EasyCharm GetCharmByName(string charmName)
        {
            return Charms[charmName];
        }

        internal static SpellStaller Instance
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

            On.HeroController.Update += HeroController_Update;

            bool rando = ModHooks.GetMod("Randomizer 4") is Mod;
            bool ic = ModHooks.GetMod("ItemChangerMod") is Mod;

            if (rando && ic)
            {
                RandoItem.InitRandoConnection();
            }

            Log("Initialized");
        }


        // Global Settings Stuff
        public void OnLoadGlobal(GlobalSettings s) => GS = s;

        public GlobalSettings OnSaveGlobal() => GS;

        public bool isStallEnabled()
        {
            return GS.Enabled;
        }

        public bool isDamageEnabled()
        {
            return GS.ShouldDamage;
        }

        public bool LightBridgeMode()
        {
            return GS.LightBridge && isStallEnabled();
        }

        public void GiveCharm(string charmName)
        {
            Charms[charmName].GiveCharm(true);
        }


        private void HeroController_Update(On.HeroController.orig_Update orig, HeroController self)
        {
            orig(self);
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                GiveCharm(SpellStabilizer.charmName);
                GiveCharm(DreamSifter.charmName);
                GiveCharm(SoulShuttle.charmName);
            }


            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                GiveCharm(DreamSifter.charmName);
            }
        }


        public void OnLoadLocal(SaveSettings s)
        {
            localSettings = s;
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

        public SaveSettings OnSaveLocal()
        {
            localSettings.Charms = new Dictionary<string, EasyCharmState>();
            foreach (var kvp in Charms)
            {
                if (Charms.TryGetValue(kvp.Key, out EasyCharm m))
                {
                    localSettings.Charms[kvp.Key] = m.GetCharmState();
                }
            }
            return localSettings;
        }
    }
}
