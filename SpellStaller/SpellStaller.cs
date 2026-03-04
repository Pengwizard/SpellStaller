using Modding;
using SpellStaller.Charms;
using System;

namespace SpellStaller
{
    public partial class SpellStaller : Mod, IGlobalSettings<GlobalSettings>
    {
        private static SpellStaller _instance;
        public static GlobalSettings GS { get; set; } = new GlobalSettings();


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


            On.HutongGames.PlayMaker.Actions.Wait.OnEnter += SpellStabalizer.ExtendFireballDuration;
            On.HutongGames.PlayMaker.Actions.SetVelocityAsAngle.OnEnter += SpellStabalizer.SlowVengefulSpirit;
            On.HutongGames.PlayMaker.Actions.Wait.OnEnter += SoulShuttle.ApplySoulShuttle;
            On.HutongGames.PlayMaker.Actions.Tk2dPlayAnimationWithEvents.OnEnter += SpellStabalizer.EditScream;

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

    }
}
