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
using MonoMod.Cil;

namespace LongstandingSolitudeFix
{
    public class FreeUnlockHolderBehavior : CharacterBody.ItemBehavior
    {
        private void Awake()
        {
            //GameModeCatalog.availability.CallWhenAvailable(new Action(PostLoad));

            enabled = false;
        }

        //public void PostLoad()
        //{
        //}

        private void OnEnable()
        {
            if (!(bool)body)
            {
                return;
            }

            body.AddBuff(DLC2Content.Buffs.FreeUnlocks);
        }
        private void OnDisable()
        {
            if (!(bool)body)
            {
                return;
            }

            if (body.HasBuff(DLC2Content.Buffs.FreeUnlocks))
            {
                body.RemoveBuff(DLC2Content.Buffs.FreeUnlocks);
            }
        }
    }
}
