using HutongGames.PlayMaker.Actions;
using SFCore;
using System;
using UnityEngine;

namespace SpellStaller.Charms
{

    // Charm that makes VS/Shade Souls into solid platforms.
    // Takes priority over the Spell Stabalizer charm in the speed of the fireball.
    public class SoulShuttle : EasyCharm
    {
        private static string charmSprite = "SoulShuttle";
        public static string charmName = "Soul Shuttle";
        // TODO: After release make this description better?
        private static string charmDescription = "Condenses the user's soul projection's further, to the point of being a solid object.";
        private static int defaultCost = 0;

        public SoulShuttle() : base()
        {
             ApplyHooks();
        }

        public bool ShuttleMode()
        {
            return IsEquipped;
        }

        public float GetWaitModifider()
        {
            if (ShuttleMode())
            {
                return 10f;
            }

            return 1f;
        }

        public float GetShuttleSpeed()
        {
            return 13f;
        }

        public void ApplyHooks()
        {
            On.HutongGames.PlayMaker.Actions.Wait.OnEnter += ApplySoulShuttle;
        }


        public Vector3 LightbridgeSpawnPosition(GameObject fireball)
        {
            var heroPosition = HeroController.instance.transform.position;
            float xOffset = 0;
            if (fireball.transform.position.x > heroPosition.x)
            {
                xOffset = -5f;
            }
            else
            {
                xOffset = 5f;
            }
            return HeroController.instance.transform.position + new Vector3(xOffset, -(1f + SpellStabilizer.GetFireballLevel()));
        }

        public void UpdateFireballObjectForShuttle(GameObject gameObj)
        {
            // Makes the fireball a solid terrain.
            try
            {
                if (ShuttleMode())
                {
                    gameObj.layer = LayerMask.NameToLayer("Terrain");
                    // gameObj.AddComponent<DamageHero>();
                    gameObj.transform.position = LightbridgeSpawnPosition(gameObj);
                    // gameObj.AddComponent<Dummy>().StartCoroutine(SpawnLightBridge(gameObj));

                    gameObj.GetComponent<BoxCollider2D>().isTrigger = false;
                    gameObj.GetComponent<Rigidbody2D>().isKinematic = true;
                }
                else
                {
                    gameObj.layer = LayerMask.NameToLayer("Attack");
                    if (gameObj.GetComponent<Dummy>() != null)
                    {
                        gameObj.GetComponent<Dummy>().StopAllCoroutines();
                        Component.Destroy(gameObj.GetComponent<Dummy>());
                    }

                    gameObj.GetComponent<BoxCollider2D>().isTrigger = true;
                    gameObj.GetComponent<Rigidbody2D>().isKinematic = false;
                }
            }
            catch (Exception err)
            {
                // SpellStaller.Instance.LogError(err.ToString());
            }
        }


        public void ApplySoulShuttle(On.HutongGames.PlayMaker.Actions.Wait.orig_OnEnter orig, Wait self)
        {
            try
            {
                if (self.Fsm.FsmComponent.gameObject.name.Contains("Fireball"))
                {
                    UpdateFireballObjectForShuttle(self.Fsm.FsmComponent.gameObject);
                }
            }
            catch (Exception err)
            {
                // SpellStaller.Instance.LogError(err.ToString());
            }
            orig(self);
        }


        protected override Sprite GetSpriteInternal()
        {
            return new EmbeddedSprite(SoulShuttle.charmSprite).Value;
        }

        protected override string GetName()
        {
            return SoulShuttle.charmName;
        }

        protected override string GetDescription()
        {
            return SoulShuttle.charmDescription;
        }

        protected override int GetCharmCost()
        {
            return SoulShuttle.defaultCost;
        }
    }
}
