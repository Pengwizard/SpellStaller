using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Internal;
using SFCore;
using System;
using UnityEngine;
using System;
using System.Collections;

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

            if (PlayerData.instance.hasDreamGate)
            {
                soulToGive *= 9;
                masksToHeal = 11;
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
                yield return new WaitForSeconds(30f);
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

        private static string charmSprite = "SoulShuttle";
        public static string charmName = "Dream Sifter";
        // TODO: After release make this description better?
        private static string charmDescription = "A circle of fine dream-thread mesh, with specks of Soul stuck to it.<br><br>Allows a dream-nail wielder to gather Soul from thin air.";
        private static int defaultCost = 0;

        public DreamSifter() : base()
        {
            ApplyHooks();
        }

        public bool AllowSifting()
        {
            return IsEquipped;
        }

        private void HookDreamSifter(PlayMakerFSM fsm)
        {
            fsm.GetState("End").AddFirstAction(new Lambda(AddSoul));
        }

        private void AddSoul()
        {
            if (AllowSifting())
            {
                SoulOrbSpawner.SpawnSoul(HeroController.instance.transform);
            }
        }

        public void ApplyHooks()
        {
            Events.AddFsmEdit(dreamNailId, HookDreamSifter);
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
