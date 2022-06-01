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

using System;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;
using FullSerializer;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Shield
    /// </summary>
    public class Shield : IncumbentEffect
    {
        public static readonly string EffectKey = "Shield";

        int startingShield;
        int shieldRemaining;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(35, 255);
            properties.SupportDuration = true;
            properties.SupportMagnitude = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_All;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Alteration;
            properties.DurationCosts = MakeEffectCosts(28, 8);
            properties.MagnitudeCosts = MakeEffectCosts(80, 60);
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("shield");
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1590);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1290);

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return other is Shield;
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;

            // Top up shield amount no more than starting value
            Shield incumbentShield = incumbent as Shield;
            incumbentShield.shieldRemaining += GetMagnitude(GetPeeredEntityBehaviour(manager));
            if (incumbentShield.shieldRemaining > incumbentShield.startingShield)
                incumbentShield.shieldRemaining = incumbentShield.startingShield;
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);

            // Set initial shield amount
            startingShield = shieldRemaining = GetMagnitude(GetPeeredEntityBehaviour(manager));
        }

        /// <summary>
        /// Apply damage to shield.
        /// </summary>
        /// <param name="amount">Amount of damage to apply.</param>
        /// <returns>Damaged passed through after removing shield amount. Will be 0 if damage amount less than remaining shield amount.</returns>
        public int DamageShield(int amount)
        {
            if (shieldRemaining > 0)
            {
                shieldRemaining -= amount;
                if (shieldRemaining <= 0)
                {
                    // Shield busted - immediately end effect and return shield overflow amount
                    ResignAsIncumbent();
                    RoundsRemaining = 0;
                    manager.UpdateHUDSpellIcons();
                    return Math.Abs(shieldRemaining);
                }

                return 0;
            }
            else
            {
                return amount;
            }
        }

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public int startingShield;
            public int shieldRemaining;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.startingShield = startingShield;
            data.shieldRemaining = shieldRemaining;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            startingShield = data.startingShield;
            shieldRemaining = data.shieldRemaining;
        }

        #endregion
    }
}
