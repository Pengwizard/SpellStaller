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
    internal class Dummy : MonoBehaviour { }

    // Charm that makes VS/Shade Souls into solid platforms.
    // Takes priority over the Spell Stabalizer charm in the speed of the fireball.
    public class SoulShuttle
    {
        public static bool ShuttleMode()
        {
            return SpellStaller.Instance.LightBridgeMode();
        }

        public static float GetWaitModifider()
        {
            if (ShuttleMode())
            {
                return 10f;
            }

            return 1f;
        }

        public static float GetShuttleSpeed()
        {
            return 13f;
        }

        public static Vector3 LightbridgeSpawnPosition(GameObject fireball)
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
            return HeroController.instance.transform.position + new Vector3(xOffset, -(1f + SpellStabalizer.GetFireballLevel()));
        }

        public static void UpdateFireballObjectForShuttle(GameObject gameObj)
        {
            // Makes the fireball a solid terrain.
            try
            {
                if (ShuttleMode())
                {
                    gameObj.layer = LayerMask.NameToLayer("Terrain");
                    gameObj.transform.position = LightbridgeSpawnPosition(gameObj);
                    // gameObj.AddComponent<Dummy>().StartCoroutine(SpawnLightBridge(gameObj));
                }
                else
                {
                    gameObj.layer = LayerMask.NameToLayer("Attack");
                    if (gameObj.GetComponent<Dummy>() != null)
                    {
                        gameObj.GetComponent<Dummy>().StopAllCoroutines();
                        Component.Destroy(gameObj.GetComponent<Dummy>());
                    }

                }
                gameObj.GetComponent<BoxCollider2D>().isTrigger = !ShuttleMode();
                gameObj.GetComponent<Rigidbody2D>().isKinematic = ShuttleMode();
            }
            catch (Exception err)
            {
                SpellStaller.Instance.LogError(err.ToString());
            }
        }


        public static void ApplySoulShuttle(On.HutongGames.PlayMaker.Actions.Wait.orig_OnEnter orig, Wait self)
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
                SpellStaller.Instance.LogError(err.ToString());
            }
            orig(self);
        }
    }
}
