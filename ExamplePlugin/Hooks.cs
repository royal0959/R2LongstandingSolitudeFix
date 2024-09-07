﻿using R2API;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;

namespace LongstandingSolitudeFix
{
    public class Hooks
    {
        // TODO: fix interacting with free item consuming a buff
        private static Dictionary<PlayerCharacterMasterController, int> playersLastBuffCount = [];

        internal static void Init()
        {
            On.RoR2.SceneExitController.SetState += SceneExitController_SetState;
            On.RoR2.PlayerCharacterMasterController.OnBodyStart += PlayerCharacterMasterController_OnBodyStart;
            GlobalEventManager.onCharacterLevelUp += GlobalEventManager_onCharacterLevelUp;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;

            Run.onRunStartGlobal += Run_onRunStartGlobal;
        }

        // turn into pearl at full level
        private static void ReplaceLSWithPearl(CharacterBody body, Inventory inventory)
        {
            if (body.level < TeamManager.naturalLevelCap)
            {
                return;
            }

            int itemsCount = inventory.GetItemCount(DLC2Content.Items.OnLevelUpFreeUnlock);

            Log.Info($"itemsCount {itemsCount}");

            if (itemsCount < 1)
            {
                return;
            }

            inventory.RemoveItem(DLC2Content.Items.OnLevelUpFreeUnlock, itemsCount);
            inventory.GiveItem(RoR2Content.Items.Pearl, itemsCount);
            CharacterMasterNotificationQueue.SendTransformNotification(body.master, DLC2Content.Items.OnLevelUpFreeUnlock.itemIndex, RoR2Content.Items.Pearl.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
        }

        private static void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);

            if (NetworkServer.active)
            {
                Log.Info($"le count {self.inventory.GetItemCount(DLC2Content.Items.OnLevelUpFreeUnlock)}");

                if (self.inventory.GetItemCount(DLC2Content.Items.OnLevelUpFreeUnlock) >= 1)
                {
                    ReplaceLSWithPearl(self.GetBody(), self.inventory);
                }

            }
        }

        private static void GlobalEventManager_onCharacterLevelUp(CharacterBody body)
        {
            ReplaceLSWithPearl(body, body.inventory);
        }

        // reset every run start
        private static void Run_onRunStartGlobal(Run obj)
        {
            playersLastBuffCount.Clear();
        }

        // give back buff every scene change
        private static void PlayerCharacterMasterController_OnBodyStart(On.RoR2.PlayerCharacterMasterController.orig_OnBodyStart orig, PlayerCharacterMasterController self)
        {
            orig(self);

            foreach (KeyValuePair<PlayerCharacterMasterController, int> entry in playersLastBuffCount)
            {
                CharacterMaster master = entry.Key.master;

                if (master == null)
                {
                    return;
                }

                CharacterBody body = master.GetBody();

                if (body == null)
                {
                    return;
                }

                for (int i = 0; i < entry.Value; i++)
                {
                    body.AddBuff(DLC2Content.Buffs.FreeUnlocks);
                }
            }

            playersLastBuffCount.Clear();
        }

        private static void SceneExitController_SetState(On.RoR2.SceneExitController.orig_SetState orig, SceneExitController self, SceneExitController.ExitState newState)
        {
            if (newState == SceneExitController.ExitState.TeleportOut)
            {
                var instances = PlayerCharacterMasterController.instances;
                foreach (var playerCharacterMaster in PlayerCharacterMasterController.instances)
                {
                    CharacterMaster master = playerCharacterMaster.master;
                    int buffsCount = master.GetBody().GetBuffCount(DLC2Content.Buffs.FreeUnlocks);

                    playersLastBuffCount[playerCharacterMaster] = buffsCount;
                }
            }

            orig(self, newState);
        }
    }
}