// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
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
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "soulTrap");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1603);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1303);
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
            DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, messageID));
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

        public bool FillEmptyTrapItem(MobileTypes soulType)
        {
            // Get all soul traps and find first empty
            DaggerfallUnityItem emptyTrap = null;
            List<DaggerfallUnityItem> traps = GameManager.Instance.PlayerEntity.Items.SearchItems(ItemGroups.MiscItems, (int)MiscItems.Soul_trap);
            foreach(DaggerfallUnityItem trap in traps)
            {
                if (trap.TrappedSoulType == MobileTypes.None)
                {
                    emptyTrap = trap;
                    break;
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
