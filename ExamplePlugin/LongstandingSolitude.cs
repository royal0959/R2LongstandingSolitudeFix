using RoR2;
using RoR2.Items;
using BepInEx;
using R2API;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Random = System.Random;
using UnityEngine.AddressableAssets;
using HarmonyLib;
using System.Linq;
using UnityEngine.Events;
using System.ComponentModel;

namespace LongstandingSolitudeFix
{
    public class LongstandingSolitude : CharacterBody.ItemBehavior
    {
        //int lastStack = 0;

        private void Awake()
        {
            enabled = false;
        }

        

        //private void OnEnable()
        //{

        //}

        //private void OnDisable()
        //{
        //    RemoveAllFreeUnlockHolders();
        //}

        //private void FixedUpdate()
        //{
        //    if (lastStack != stack)
        //    {
        //        UpdateFreeUnlockHolders(stack);
        //    }
        //}

        private void RemoveAllFreeUnlockHolders()
        {
            this.body.inventory.ResetItem(CustomItems.FreeUnlockHolder);

        }

        //private void UpdateFreeUnlockHolders()
        //{
            //Inventory inventory = this.body.inventory;
            //int itemCount = inventory.GetItemCount(CustomItems.FreeUnlockHolder);

            //if (newStack < lastStack)
            //{
            //    int count = (newStack - itemCount);

            //    inventory.GiveItem(CustomItems.FreeUnlockHolder, count);
            //    Log.Info($"Buff stack: {inventory.GetItemCount(CustomItems.FreeUnlockHolder)}");
            //}
            //else if (newStack > lastStack)
            //{
            //    int count = (itemCount - newStack);

            //    inventory.GiveItem(CustomItems.FreeUnlockHolder, count);
            //    Log.Info($"Buff stack: {inventory.GetItemCount(CustomItems.FreeUnlockHolder)}");
            //}
        //}
    }
}
