// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Dispel - Magic
    /// </summary>
    public class DispelMagic : BaseEntityEffect
    {
        public static readonly string EffectKey = "Dispel-Magic";

        List<LiveEffectBundle> validSpells = new List<LiveEffectBundle>();
        DaggerfallListPickerWindow spellPicker = null;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(6, 0);
            properties.SupportChance = true;
            properties.ShowSpellIcon = false;
            properties.ChanceFunction = ChanceFunction.Custom;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Self;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Mysticism;
            properties.ChanceCosts = MakeEffectCosts(120, 180);
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("dispel");
        public override string SubGroupName => TextManager.Instance.GetLocalizedText("magic");
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1516);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1216);

        public override void MagicRound()
        {
            base.MagicRound();

            // Confirmed in classic that Dispel Magic spell point cost is applied when casting even if player cancels popup
            // So not refunding spell points at cast time here like Identify

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Target must be player - no effect on other entities
            if (entityBehaviour != GameManager.Instance.PlayerEntityBehaviour)
                return;

            // Launch spell picker
            spellPicker = new DaggerfallListPickerWindow(DaggerfallUI.Instance.UserInterfaceManager);
            spellPicker.ParentPanel.BackgroundColor = Color.clear;
            spellPicker.AllowCancel = true;
            spellPicker.OnItemPicked += SpellPicker_OnItemPicked;
            spellPicker.OnClose += SpellPicker_OnClose;
            PopulateSpellPicker();
            DaggerfallUI.Instance.UserInterfaceManager.PushWindow(spellPicker);
        }

        void PopulateSpellPicker()
        {
            spellPicker.ListBox.ClearItems();
            validSpells.Clear();

            // Get all effect bundles currently operating on player
            EntityEffectManager playerEffectManager = GameManager.Instance.PlayerEffectManager;
            LiveEffectBundle[] effectBundles = playerEffectManager.EffectBundles;
            if (effectBundles == null || effectBundles.Length == 0)
                return;

            // Add to list view
            foreach (LiveEffectBundle bundle in effectBundles)
            {
                // Must be a spell bundle and have an icon
                // Confirmed classic allows player to dispel effects from items
                // Item effects will be reapplied on next recast or when item is unequipped and equipped again
                if ((bundle.bundleType == BundleTypes.Spell || bundle.bundleType == BundleTypes.HeldMagicItem) && ShowIcon(bundle))
                {
                    spellPicker.ListBox.AddItem(bundle.name);
                    validSpells.Add(bundle);
                }
            }
        }

        bool ShowIcon(LiveEffectBundle bundle)
        {
            // At least one effect with remaining rounds must want to show an icon, or be from an equipped item
            foreach (IEntityEffect effect in bundle.liveEffects)
            {
                if (effect.Properties.ShowSpellIcon && (effect.RoundsRemaining >= 0 || bundle.fromEquippedItem != null))
                {
                    return true;
                }
            }

            return false;
        }

        private void SpellPicker_OnItemPicked(int index, string itemString)
        {
            // Check index range
            if (index < 0 || index > validSpells.Count)
                return;

            // Get selected spell bundle
            LiveEffectBundle bundle = validSpells[index];

            // Dispel selected bundle
            // Player self-cast spells are always dispelled, otherwise use Chance roll
            if (bundle.caster.EntityType == EntityTypes.Player || RollChance())
            {
                manager.RemoveBundle(bundle);
                DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("dispelMagicSuccess"));
                //Debug.LogFormat("Dispelling {0}", validSpells[index].name);
            }
            else
            {
                DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("dispelMagicFailed"));
            }

            spellPicker.CloseWindow();
        }

        private void SpellPicker_OnClose()
        {
            spellPicker.OnItemPicked -= SpellPicker_OnItemPicked;
            spellPicker.OnClose -= SpellPicker_OnClose;
        }
    }
}
