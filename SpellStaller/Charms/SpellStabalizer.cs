using System;
using HutongGames.PlayMaker.Actions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItemChanger.Extensions;
using KorzUtils.Helper;
using SFCore.Utils;


namespace SpellStaller.Charms
{
    public class SpellStabalizer
    {
        private const float Slowdown = 4f;
        // The zero index is there bc im lazy :pout:
        private static float[] WaitTimes = { 0f, 0.4f, 0.475f };
        private static float[] SlowSpeeds = { 0f, 1f, 0.1f };
        private static float[] DefaultSpeeds = { 0f, 40f, 45f };

        private static bool ShouldStall()
        {
            // TODO: Check if charm is equiped?
            return SpellStaller.Instance.isStallEnabled();
        }

        public static int GetFireballLevel()
        {
            return HeroController.instance.playerData.fireballLevel;
        }

        private static int GetScreamLevel()
        {
            return HeroController.instance.playerData.screamLevel;
        }

        public static float GetFireballWaitTime(int spellLevel)
        {
            var slowdown = ShouldStall() ? Slowdown : 1;
            return WaitTimes[spellLevel] * slowdown * SoulShuttle.GetWaitModifider();
        }

        public static float GetFireballSpeed(int spellLevel)
        {
            // Shuttle speed takes priority
            if (SoulShuttle.ShuttleMode())
            {
                return SoulShuttle.GetShuttleSpeed();
            }

            return ShouldStall() ? SlowSpeeds[spellLevel] : DefaultSpeeds[spellLevel];
        }


        public static void ExtendFireballDuration(On.HutongGames.PlayMaker.Actions.Wait.orig_OnEnter orig, Wait self)
        {
            try
            {
                if (self.Fsm.FsmComponent.gameObject.name.Contains("Fireball") &&
                    self.Fsm.Name == "Fireball Control" &&
                    self.State.Name == "Idle")
                {
                    self.time.Value = GetFireballWaitTime(GetFireballLevel());
                    SpellStaller.Instance.Log($"New duration is: {self.time.Value}");
                }
            }
            catch (Exception err)
            {
                SpellStaller.Instance.LogError(err.ToString());
            }
            orig(self);
        }

        public static void SlowVengefulSpirit(On.HutongGames.PlayMaker.Actions.SetVelocityAsAngle.orig_OnEnter orig, SetVelocityAsAngle self)
        {
            try
            {
                if (self.Fsm.FsmComponent.gameObject.name.Contains("Fireball") &&
                    self.Fsm.FsmComponent.gameObject.name.Contains("Top") &&
                    self.Fsm.Name == "Fireball Cast")
                {
                    self.speed.Value = GetFireballSpeed(GetFireballLevel());
                }
            }
            catch (Exception err)
            {
                SpellStaller.Instance.LogError(err.ToString());
            }

            orig(self);
        }

        // Scream extender code

        // We keep a reference of the scream objects to spawn them in later.
        private static GameObject _wraith;
        private static GameObject _shriek;
        public static GameObject Scream
        {
            get
            {
                if (_wraith == null)
                {
                    _wraith = GameObject.Instantiate(HeroController.instance.transform.Find("Spells/Scr Heads").gameObject);
                    GameObject.DontDestroyOnLoad(_wraith);
                }
                return _wraith;
            }
        }

        public static GameObject Shriek
        {
            get
            {
                if (_shriek == null)
                {
                    _shriek = GameObject.Instantiate(HeroController.instance.transform.Find("Spells/Scr Heads 2").gameObject);
                    GameObject.DontDestroyOnLoad(_shriek);
                }
                return _shriek;
            }
        }

        public static Vector3 ScreamSpawnPosition()
        {
            return HeroController.instance.transform.position + new Vector3(0f, 2f + GetScreamLevel());
        }

        private static IEnumerator SpawnScreams(GameObject holder, Vector3 position)
        {
            yield return new WaitForSeconds(0f);
            GameObject scream = GameObject.Instantiate(GetScreamLevel() == 1 ? Scream : Shriek);
            scream.transform.SetParent(holder.transform);
            scream.transform.position = position;
            scream.LocateMyFSM("FSM").FsmVariables.FindFsmBool("Unparent").Value = false;
            scream.LocateMyFSM("FSM").FsmVariables.FindFsmBool("Reposition").Value = false;
            var waitState = scream.LocateMyFSM("FSM").GetState("Wait");
            // We start the scream at a slightly later frame to make it blink less.
            waitState.GetFirstActionOfType<Tk2dPlayFrame>().frame.Value = 3;
            Component.Destroy(scream.LocateMyFSM("Deactivate on Hit"));
            int i = 0;
            do
            {
                scream.SetActive(false);
                scream.SetActive(true);
                yield return new WaitForSeconds(0.35f);
                i++;
            } while (i < (10 * GetScreamLevel()));
            GameObject.Destroy(holder);
        }


        public static void EditScream(
            On.HutongGames.PlayMaker.Actions.Tk2dPlayAnimationWithEvents.orig_OnEnter orig,
            HutongGames.PlayMaker.Actions.Tk2dPlayAnimationWithEvents self)
        {
            if (self.IsCorrectContext("Spell Control", "Knight", null) &&
                (self.State.Name == "Scream End" || self.State.Name == "Scream End 2"))
            {
                GameObject screamHolder = new("Persistent Scream");
                screamHolder.SetActive(true);
                screamHolder.AddComponent<Dummy>().StartCoroutine(SpawnScreams(screamHolder, ScreamSpawnPosition()));
            }
            orig(self);
        }
    }
}
