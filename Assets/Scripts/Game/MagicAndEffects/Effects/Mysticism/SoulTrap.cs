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

using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Soul Trap
    /// </summary>
    public class SoulTrap : IncumbentEffect
    {
        public static readonly string EffectKey = "SoulTrap";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(12, 255);
            properties.ShowSpellIcon = false;
            properties.SupportDuration = true;
            properties.SupportChance = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Other;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_All;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Mysticism;
            properties.DurationCosts = MakeEffectCosts(60, 68);
            properties.ChanceCosts = MakeEffectCosts(40, 68);
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("soulTrap");
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1603);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1303);

        public override bool ChanceSuccess
        {
            // Always return true so that effect is always attached to entity
            // Chance will be re-rolled using RollTrapChance() when entity is slain to see if soul is trapped
            get { return true; }
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
        }

        protected override void BecomeIncumbent()
        {
            base.BecomeIncumbent();

            // Output trap start message
            string messageID = string.Empty;
            switch (manager.EntityBehaviour.EntityType)
            {
                case EntityTypes.CivilianNPC:
                case EntityTypes.EnemyClass:
                    messageID = "trapHumanoid";
                    ResignAsIncumbent();
                    End();
                    break;
                case EntityTypes.EnemyMonster:
                    messageID = "trapActive";
                    break;
                default:
                    ResignAsIncumbent();
                    End();
                    return;
            }
            DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText(messageID));
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return other is SoulTrap;
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        public override void ConstantEffect()
        {
            base.ConstantEffect();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Raise soul trap active flag
            (entityBehaviour.Entity as EnemyEntity).SoulTrapActive = true;
        }

        public bool RollTrapChance()
        {
            return RollChance();
        }

        public static bool FillEmptyTrapItem(MobileTypes soulType, bool azurasStarOnly = false)
        {
            // In classic, the player's items are iterated through and the first instance found of an empty soul trap or Azura's Star is used.
            // Whichever is chosen first would depend on the order of the list of items, which would probably be the order in which the items were added to the inventory.
            // For here, fill Azura's Star first, then soul traps, as this is probably the behavior players would expect.

            DaggerfallUnityItem emptyTrap = null;

            // Get empty Azura's Star
            List<DaggerfallUnityItem> amulets = GameManager.Instance.PlayerEntity.Items.SearchItems(ItemGroups.Jewellery, (int)Jewellery.Amulet);
            foreach (DaggerfallUnityItem amulet in amulets)
            {
                if (amulet.ContainsEnchantment(EnchantmentTypes.SpecialArtifactEffect, (short)ArtifactsSubTypes.Azuras_Star) && amulet.TrappedSoulType == MobileTypes.None)
                {
                    emptyTrap = amulet;
                    break;
                }
            }

            // Exit if trapping to Azura's Star and it was not found or already full
            if (emptyTrap == null && azurasStarOnly)
                return false;

            // Get another trap
            if (emptyTrap == null)
            {
                // Get empty soul trap
                List<DaggerfallUnityItem> traps = GameManager.Instance.PlayerEntity.Items.SearchItems(ItemGroups.MiscItems, (int)MiscItems.Soul_trap);
                foreach (DaggerfallUnityItem trap in traps)
                {
                    if (trap.TrappedSoulType == MobileTypes.None)
                    {
                        emptyTrap = trap;
                        break;
                    }
                }
            }

            // Fill the empty trap
            if (emptyTrap != null)
            {
                emptyTrap.TrappedSoulType = soulType;
                return true;
            }
            
            return false;
        }
    }
}
