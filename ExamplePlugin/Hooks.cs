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
        internal static void Init()
        {
            GlobalEventManager.onCharacterLevelUp += GlobalEventManager_onCharacterLevelUp;
            //On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
        }

        private static void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            if (itemIndex != DLC2Content.Items.OnLevelUpFreeUnlock.itemIndex)
            {
                orig(self, itemIndex, count);
                return;
            }

            if (Configs.BreakIntoPearl.Value != true)
            {
                orig(self, itemIndex, count);
                return;
            }

            CharacterMaster master = self.gameObject.GetComponent<CharacterMaster>();

            if (master == null)
            {
                orig(self, itemIndex, count);
                return;
            }

            CharacterBody body = master.GetBody();

            if (body == null)
            {
                orig(self, itemIndex, count);
                return;
            }

            if (body.level < TeamManager.naturalLevelCap)
            {
                orig(self, itemIndex, count);
                return;
            }

            orig(self, RoR2Content.Items.Pearl.itemIndex, count);
            CharacterMasterNotificationQueue.SendTransformNotification(body.master, DLC2Content.Items.OnLevelUpFreeUnlock.itemIndex, RoR2Content.Items.Pearl.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);

            //ReplaceLSWithPearl(body, self);
        }

        private static void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            int lastBuffsCount = 0;

            CharacterBody body = activator.GetComponent<CharacterBody>();

            if (body != null)
            {
                lastBuffsCount = body.GetBuffCount(DLC2Content.Buffs.FreeUnlocks);
            }

            orig(self, activator);

            if (lastBuffsCount == 0)
            {
                return;
            }

            if (body == null)
            {
                return;
            }

            if (self.cost > 0)
            {
                return;
            }

            // if item was free: regain the buffs we lost
            // as FreeUnlock buff is still removed after purchasing free interactables (such as a hacked shrine)

            int buffsAfter = body.GetBuffCount(DLC2Content.Buffs.FreeUnlocks);

            if ((lastBuffsCount - buffsAfter) <= 0)
            {
                return;
            }

            for (int i = 0; i < (lastBuffsCount - buffsAfter); i++)
            {
                body.AddBuff(DLC2Content.Buffs.FreeUnlocks);
            }
        }

        // turn into pearl at full level
        private static void ReplaceLSWithPearl(CharacterBody body, Inventory inventory)
        {
            if (Configs.BreakIntoPearl.Value != true)
            {
                return;
            }

            if (body.level < TeamManager.naturalLevelCap)
            {
                return;
            }

            int itemsCount = inventory.GetItemCount(DLC2Content.Items.OnLevelUpFreeUnlock);

            if (itemsCount < 1)
            {
                return;
            }

            inventory.RemoveItem(DLC2Content.Items.OnLevelUpFreeUnlock, itemsCount);
            inventory.GiveItem(RoR2Content.Items.Pearl, itemsCount);
            CharacterMasterNotificationQueue.SendTransformNotification(body.master, DLC2Content.Items.OnLevelUpFreeUnlock.itemIndex, RoR2Content.Items.Pearl.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
        }

        private static void GlobalEventManager_onCharacterLevelUp(CharacterBody body)
        {
            ReplaceLSWithPearl(body, body.inventory);
        }
    }
}
