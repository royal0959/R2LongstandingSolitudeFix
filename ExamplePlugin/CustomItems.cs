using BepInEx;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace LongstandingSolitudeFix
{
    internal class CustomItems
    {
        public static ItemDef FreeUnlockHolder;

        public static void Init()
        {
            CreateFreeUnlockHolder();

            //AddLanguageTokens();
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
        }

        private static void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            if (NetworkServer.active)
            {
                self.AddItemBehavior<LongstandingSolitude>(self.inventory.GetItemCount(DLC2Content.Items.OnLevelUpFreeUnlock));
                self.AddItemBehavior<FreeUnlockHolderBehavior>(self.inventory.GetItemCount(FreeUnlockHolder));

            }
            orig(self);
        }

        private static void CreateFreeUnlockHolder()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            FreeUnlockHolder = new ItemDef
            {
                name = "FreeUnlockHolder",
                //tier = ItemTier.NoTier,
                //_itemTierDef = Addressables.LoadAssetAsync<ItemTierDef>("RoR2/Base/Common/Tier3Def.asset").WaitForCompletion(),
                _itemTierDef = null,
                deprecatedTier = ItemTier.NoTier,
                pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion(),
                pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion(),
                nameToken = "ITEM_FREEUNLOCKHOLDER_NAME",
                pickupToken = "ITEM_FREEUNLOCKHOLDER_PICKUP",
                descriptionToken = "ITEM_FREEUNLOCKHOLDER_DESC",
                loreToken = "ITEM_FREEUNLOCKHOLDER_LORE",
                tags = new[]
                {
                    ItemTag.WorldUnique,
                    ItemTag.CannotCopy,
                    ItemTag.CannotDuplicate,
                    ItemTag.BrotherBlacklist,
                    ItemTag.CannotSteal,
                },

                canRemove = false,
                //hidden = true
                hidden = false,
            };
#pragma warning restore CS0618 // Type or member is obsolete

            var displayRules = new ItemDisplayRuleDict(null);

            var itemIndex = new CustomItem(FreeUnlockHolder, displayRules);
            ItemAPI.Add(itemIndex);
        }

        //private static void AddLanguageTokens()
        //{
        //    LanguageAPI.Add("ITEM_BOOSTALLSTATS_PICKUP", "Summon a <style=cIsUtility>Guardian Wisp</style>. All <style=cIsUtility>organic</style> allies are <style=cIsDamage>stronger</style> and <style=cIsDamage>Elite</style>.");
        //    LanguageAPI.Add("ITEM_BOOSTALLSTATS_DESC",
        //        "Gain an allied <style=cIsUtility>Guardian Wisp</style> that respawns every 30 seconds. All <style=cIsUtility>ORGANIC</style> allies will gain <style=cIsDamage>+200%</style> <style=cStack>(+200% per stack)</style> damage and <style=cIsUtility>+150%</style> <style=cStack>(+150% per stack)</style> health and a random <style=cIsDamage>Elite</style> status.\r\n");
        //}
    }
}
