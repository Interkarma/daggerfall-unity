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
using FullSerializer;
using DaggerfallConnect;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Used by Mace of Molag Bal to transfer spell points or strength from target to wielder.
    /// Can temporarily increase spell points and strength of wielder over their usual maximum value.
    /// </summary>
    public class MaceOfMolagBalEffect : IncumbentEffect
    {
        public static readonly string EffectKey = ArtifactsSubTypes.Mace_of_Molag_Bal.ToString();

        const int maxIncreaseRounds = 12;

        uint lastStrikeTime;
        int currentMaxMagickaIncrease;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ShowSpellIcon = false;
            properties.EnchantmentPayloadFlags = EnchantmentPayloadFlags.Strikes | EnchantmentPayloadFlags.Held;
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return other is MaceOfMolagBalEffect;
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            return;
        }

        #region Payloads

        public override void ConstantEffect()
        {
            base.ConstantEffect();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Assign current max spell point modifier
            entityBehaviour.Entity.ChangeMaxMagickaModifier(currentMaxMagickaIncrease);
        }

        public override void MagicRound()
        {
            base.MagicRound();

            // Temporary increase to spell points or strength lasts for 12 game minutes
            // Wipe bonuses if wielder does not strike another target in this time
            if (DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime() > lastStrikeTime + maxIncreaseRounds)
            {
                currentMaxMagickaIncrease = 0;
                SetStatMaxMod(DFCareer.Stats.Strength, 0);
                SetStatMod(DFCareer.Stats.Strength, 0);
            }
        }

        public override PayloadCallbackResults? EnchantmentPayloadCallback(EnchantmentPayloadFlags context, EnchantmentParam? param = null, DaggerfallEntityBehaviour sourceEntity = null, DaggerfallEntityBehaviour targetEntity = null, DaggerfallUnityItem sourceItem = null, int sourceDamage = 0)
        {
            base.EnchantmentPayloadCallback(context, param, sourceEntity, targetEntity, sourceItem, sourceDamage);

            // Validate
            if (context != EnchantmentPayloadFlags.Strikes || sourceEntity == null || targetEntity == null || sourceItem == null || sourceDamage == 0)
                return null;

            // Target can saves vs magic
            if (FormulaHelper.SavingThrow(DFCareer.Elements.Magic, DFCareer.EffectFlags.Magic, targetEntity.Entity, 0) == 0)
                return null;

            // Find live effect as EnchantmentPayloadCallback is only called on template and we need to change live data
            // Log error and allow effect to continue - but it will not operate fully
            MaceOfMolagBalEffect liveEffect = FindLiveEffect(sourceEntity);
            if (liveEffect == null)
                Debug.LogError("MaceOfMolagBalEffect.EnchantmentPayloadCallback could not find live effect instance on source entity.");

            // Seed random
            Random.InitState(Time.frameCount);

            // "The Mace of Molag Bal drains its victim's spell points and gives them to the bearer.
            // "If the victim has no spell points, he is drained of strength, which is also transferred to the wielder."
            // "Using the Mace of Molag Bal can actually give its bearer more spell points or more strength than he would have fully rested."
            // After considerable testing in classic unable to actually reproduce the first part of this effect (transfer of spell points)
            // Could be down to casters in classic dumping their spell point pool almost immediately, but even backstabs failed to transfer any spell points to wielder
            // Implementing spell point drain as per description rather than based on observation in classic
            // Assuming spell points drained are equal to damage
            // Testing in classic shows that strength increase is always 1-6
            if (targetEntity.Entity.CurrentMagicka > 0)
            {
                // First drain spell points from target
                // Limit drain to available spell points on target
                int spellPointsDrained = targetEntity.Entity.CurrentMagicka - targetEntity.Entity.DecreaseMagicka(sourceDamage);

                // Then raise spell points on source equal to amount drained
                // If this will increase over usual spell point pool amount then increase max to by overflow amount
                int overflow = sourceEntity.Entity.CurrentMagicka + spellPointsDrained - sourceEntity.Entity.MaxMagicka;
                if (overflow > 0)
                {
                    // Immediately set increase to spell point maximum to absorb all spell points drained
                    // This also needs to be set each tick so we accumulate this overflow amount to use in live effect
                    sourceEntity.Entity.ChangeMaxMagickaModifier(overflow);
                    if (liveEffect != null)
                        liveEffect.currentMaxMagickaIncrease += overflow;
                }
                sourceEntity.Entity.IncreaseMagicka(spellPointsDrained);
            }
            else
            {
                // If target is out of spell points then drain 1-6 strength from target
                int strengthDrained = Random.Range(1, 7);
                DrainTargetStrength(targetEntity, strengthDrained);

                // Accumulate drain amount as a strength buff in live effect
                // These modifiers are automatically serialized/deserialized as part of effect framework
                if (liveEffect != null)
                {
                    liveEffect.ChangeStatMaxMod(DFCareer.Stats.Strength, strengthDrained);
                    liveEffect.ChangeStatMod(DFCareer.Stats.Strength, strengthDrained);
                }
            }

            // Record last strike time
            if (liveEffect != null)
                liveEffect.lastStrikeTime = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();

            // Durability loss is equal to damage caused
            return new PayloadCallbackResults()
            {
                durabilityLoss = sourceDamage,
            };
        }

        MaceOfMolagBalEffect FindLiveEffect(DaggerfallEntityBehaviour entity)
        {
            if (entity == null)
                return null;

            EntityEffectManager targetManager = entity.GetComponent<EntityEffectManager>();
            if (targetManager == null)
                return null;

            return targetManager.FindIncumbentEffect<MaceOfMolagBalEffect>() as MaceOfMolagBalEffect;
        }

        void DrainTargetStrength(DaggerfallEntityBehaviour entity, int amount)
        {
            if (entity == null)
                return;

            EntityEffectManager targetManager = entity.GetComponent<EntityEffectManager>();
            if (targetManager == null)
                return;

            // Find incumbent strength drain effect on target or assign new drain
            DrainStrength drain = targetManager.FindIncumbentEffect<DrainStrength>() as DrainStrength;
            if (drain == null)
            {
                // Create and assign bundle
                // We bypass saving throws as target already had a chance at start of payload delivery
                EntityEffectBundle bundle = targetManager.CreateSpellBundle(DrainStrength.EffectKey);
                targetManager.AssignBundle(bundle, AssignBundleFlags.BypassSavingThrows);

                // Find new bundle now its assigned
                drain = targetManager.FindIncumbentEffect<DrainStrength>() as DrainStrength;
            }

            // Increment incumbent drain magnitude on target
            if (drain != null)
            {
                drain.IncreaseMagnitude(amount);
            }
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct CustomSaveData_v1
        {
            public uint lastStrikeTime;
            public int currentMaxMagickaIncrease;
            public int currentMaxStrengthIncrease;
        }

        public override object GetSaveData()
        {
            CustomSaveData_v1 data = new CustomSaveData_v1();
            data.lastStrikeTime = lastStrikeTime;
            data.currentMaxMagickaIncrease = currentMaxMagickaIncrease;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            CustomSaveData_v1 data = (CustomSaveData_v1)dataIn;
            lastStrikeTime = data.lastStrikeTime;
            currentMaxMagickaIncrease = data.currentMaxMagickaIncrease;

            // Immediately assign current max spell point modifier
            // This ensures saved spell points at time of save can be restored if greater than normal
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (entityBehaviour)
                entityBehaviour.Entity.ChangeMaxMagickaModifier(currentMaxMagickaIncrease);
        }

        #endregion
    }
}
