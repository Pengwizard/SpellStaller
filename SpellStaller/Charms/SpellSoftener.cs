using System;
using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpellStaller.Charms
{
    // Removes damage from all spells. Doesn't depend on other charms in any way.
    // TODO: Add filter? Do I even *want* this code?
    public class SpellSoftener
    {
        // Removes damage from *all* spells
        public static void RemoveDamage(On.HutongGames.PlayMaker.Actions.SetFsmInt.orig_OnEnter orig, SetFsmInt self)
        {
            orig(self);
            if (SpellStaller.Instance.isDamageEnabled())
            {
                return;
            }
            try
            {
                self.Fsm.GameObject.LocateMyFSM("damages_enemy").FsmVariables.FindFsmInt("damageDealt").Value = 0;
            }
            catch (Exception ex)
            {
                SpellStaller.Instance.Log("Failed to set spell damage.");
                SpellStaller.Instance.Log(ex);
            }
        }
    }
}
