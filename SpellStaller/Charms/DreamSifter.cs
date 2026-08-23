using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Internal;
using KorzUtils.Helper;
using SFCore;
using System;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellStaller.Charms
{

    public class SoulOrbSpawner : MonoBehaviour
    {
        public static void SpawnSoul(Transform transform)
        {
            var prefab = ObjectCache.SoulOrb;
            Destroy(prefab.Spawn());
            prefab.SetActive(true);

            // Give 34 soul per usage.
            // double if dream wielder is equipped
            // max if can dream gate.
            int soulToGive = 17;
            int masksToHeal = 1;

            if (PlayerData.instance.dreamNailUpgraded)
            {
                soulToGive *= 9;
                masksToHeal = 11;
            }
            else if (PlayerData.instance.hasDreamGate)
            {
                soulToGive *= 4;
                masksToHeal = 4;
            }
            else if (PlayerData.instance.equippedCharm_30)
            {
                soulToGive *= 2;
                masksToHeal = 2;
            }

            FlingUtils.Config config = new()
            {
                Prefab = prefab,
                AmountMin = soulToGive,
                AmountMax = soulToGive,
                SpeedMin = 10,
                SpeedMax = 20,
                AngleMin = 0,
                AngleMax = 360,
            };
            var objs = FlingUtils.SpawnAndFling(config, transform, Vector3.zero);

            // Heal on parry.
            HeroController.instance.AddHealth(masksToHeal);

            IEnumerator Routine()
            {
                yield return new WaitForSeconds(10f);
                foreach (var o in objs)
                {
                    Destroy(o);
                }
            }

            GameManager.instance.gameObject.GetOrAddComponent<SoulOrbSpawner>().StartCoroutine(Routine());
            Destroy(prefab);
        }
    }

    // Charm that allows collecting soul from thin air with dream nail.
    public class DreamSifter : EasyCharm
    {
        private static readonly FsmID dreamNailId = new("Knight", "Dream Nail");

        private static string charmSprite = "DreamSifter";
        public static string charmName = "Dream Sifter";
        // TODO: After release make this description better?
        private static string charmDescription = "Made from a circle of shell-like material, with a fine dream-thread mesh inside.\r\n\r\nAllows a user to collect scattered Soul from the air of Hallownest with a Dream Nail.\r\n\r\nDespite this being a charm, Salubra doesn't recognize it as such.";
        private static int defaultCost = 0;

        public DreamSifter() : base()
        {
            //ApplyHooks();
            On.HutongGames.PlayMaker.Actions.ActivateGameObject.OnEnter += OnEnterDreamNailEnd;
        }

        private void OnEnterDreamNailEnd(On.HutongGames.PlayMaker.Actions.ActivateGameObject.orig_OnEnter orig, HutongGames.PlayMaker.Actions.ActivateGameObject self)
        {
            orig(self);

            try
            {
                if (self.Fsm.FsmComponent.gameObject.name.Contains("Knight") &&
                    self.Fsm.Name == "Dream Nail" &&
                    self.State.Name == "Slash")
                {
                    AddSoul();
                }
            }
            catch (Exception err)
            {
                SpellStaller.Instance.LogError(err.ToString());
            }
        }

        public static List<string> overrideRooms = new() {"dream_nailcollection", "dream_abyss"};

        public bool CheckDreammail()
        {
            if (GameManager.instance.GetCurrentMapZone() != "DREAM_WORLD")
            {
                return true;
            }

            return overrideRooms.Contains(GameManager.instance.sceneName.ToLower());
        }

        public bool AllowSifting()
        {
            return IsEquipped && CheckDreammail();
        }

        private void AddSoul()
        {
            if (IsEquipped)
            {
                if (AllowSifting())
                {
                    SoulOrbSpawner.SpawnSoul(HeroController.instance.transform);
                }
                else
                {
                    GameHelper.DisplayMessage("The air here doesn't have any scattered Soul...");
                }
            }
        }

        protected override Sprite GetSpriteInternal()
        {
            return new EmbeddedSprite(DreamSifter.charmSprite).Value;
        }

        protected override string GetName()
        {
            return DreamSifter.charmName;
        }

        protected override string GetDescription()
        {
            return DreamSifter.charmDescription;
        }

        protected override int GetCharmCost()
        {
            return DreamSifter.defaultCost;
        }
    }
}
