using R2API;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LongstandingSolitudeFix
{
    public class Hooks
    {
        // TODO: make this reset every run start
        private static Dictionary<PlayerCharacterMasterController, int> playersLastBuffCount = [];

        internal static void Init()
        {
            //On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;

            On.RoR2.SceneExitController.SetState += SceneExitController_SetState;
            //On.RoR2.SceneDirector.Start += SceneDirector_Start;

            On.RoR2.PlayerCharacterMasterController.OnBodyStart += PlayerCharacterMasterController_OnBodyStart;
        }

        // give back buff every scene change
        private static void PlayerCharacterMasterController_OnBodyStart(On.RoR2.PlayerCharacterMasterController.orig_OnBodyStart orig, PlayerCharacterMasterController self)
        {
            orig(self);
            Log.Info($"BODYSTART");

            foreach (KeyValuePair<PlayerCharacterMasterController, int> entry in playersLastBuffCount)
            {
                CharacterMaster master = entry.Key.master;
                Log.Info($"PlayerCharacterMasterController {entry.Key} naster {master} body {master.GetBody()} amount {entry.Value}");

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
                    //You can get the master via playerCharacterMaster.master
                    //and the body via playerCharacterMaster.master.GetBody()

                    CharacterMaster master = playerCharacterMaster.master;
                    int buffsCount = master.GetBody().GetBuffCount(DLC2Content.Buffs.FreeUnlocks);

                    Log.Info($"buffsCount {buffsCount}");

                    playersLastBuffCount[playerCharacterMaster] = buffsCount;
                }
            }

            orig(self, newState);
        }

        //private static void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        //{
            //Log.Info("SCENE DIRECTOR STARTED!!!");
            //orig(self);

            //foreach (KeyValuePair<CharacterMaster, int> entry in playersLastBuffCount)
            //{
            //    Log.Info($"master {entry.Key} body {entry.Key.GetBody()} amount {entry.Value}");

            //    for (int i = 0; i < entry.Value; i++)
            //    {
            //        entry.Key.GetBody().AddBuff(DLC2Content.Buffs.FreeUnlocks);
            //    }
            //}

            //foreach (CharacterMaster readOnlyInstances3 in CharacterMaster.readOnlyInstancesList)
            //{
            //    Log.Info($"masterrrrr {readOnlyInstances3} body {readOnlyInstances3.GetBody()}");
            //}
        //}


        //private static void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        //{
        //    if (stack <= 0)
        //    {
        //        orig(self, activator);
        //        return;
        //    }

        //    int oldBuffsCount = this.body.GetBuffCount(DLC2Content.Buffs.FreeUnlocks);
        //    orig(self, activator);
        //    int newBuffsCount = this.body.GetBuffCount(DLC2Content.Buffs.FreeUnlocks);

        //    Log.Info($"oldBuffsCount {oldBuffsCount} newBuffsCount {newBuffsCount}");


        //    Inventory inventory = this.body.inventory;
        //    int itemCount = inventory.GetItemCount(CustomItems.FreeUnlockHolder);

        //    // TODO: move this sync check to FreeUnlockHolderBehavior
        //    // if buff is higher, that means we got more freeunlock buffs from external sources
        //    // don't sync yet
        //    if (oldBuffsCount > itemCount)
        //    {
        //        return;
        //    }

        //    if (newBuffsCount < oldBuffsCount)
        //    {
        //        int count = (newBuffsCount - itemCount);

        //        inventory.GiveItem(CustomItems.FreeUnlockHolder, count);
        //        Log.Info($"Buff stack: {inventory.GetItemCount(CustomItems.FreeUnlockHolder)}");
        //    }
        //    else if (newBuffsCount > oldBuffsCount)
        //    {
        //        int count = (itemCount - newBuffsCount);

        //        inventory.GiveItem(CustomItems.FreeUnlockHolder, count);
        //        Log.Info($"Buff stack: {inventory.GetItemCount(CustomItems.FreeUnlockHolder)}");
        //    }
        //}
    }
}
