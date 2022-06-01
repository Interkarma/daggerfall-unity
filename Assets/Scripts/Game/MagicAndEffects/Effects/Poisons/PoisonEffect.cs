// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net), Allofich
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallConnect;
using FullSerializer;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    public class PoisonEffect : IncumbentEffect
    {
        #region Fields

        public const int startValue = 128;
        protected const int totalVariants = 12;

        protected readonly VariantProperties[] variantProperties = new VariantProperties[totalVariants];

        protected uint lastMinute;
        protected int minutesToStart;
        protected int minutesRemaining;
        protected PoisonStates currentState;
        protected int forcedRoundsRemaining = 1;
        protected bool positiveStatsRemoved = false;

        #endregion

        #region Structs & Enums

        public enum PoisonStates
        {
            Waiting,
            Active,
            Complete,
        }

        protected struct VariantProperties
        {
            public Poisons poisonType;
            public EffectProperties effectProperties;
        }

        #endregion

        #region Properties

        public override EffectProperties Properties
        {
            get { return variantProperties[currentVariant].effectProperties; }
        }

        // No external code should rely on a specific "classic poison type"
        // If a mod wants to override PoisonEffect to add new poison types, then
        // it will need to stop relying on this enum
        protected virtual Poisons PoisonType
        {
            get { return variantProperties[currentVariant].poisonType; }
        }

        public bool IsDrug
        {
            get { return IsDrugType(); }
        }

        public PoisonStates CurrentState
        {
            get { return currentState; }
        }

        #endregion

        #region Overrides

        public override void SetProperties()
        {
            variantCount = totalVariants;
            SetVariantProperties(Poisons.Nux_Vomica);
            SetVariantProperties(Poisons.Arsenic);
            SetVariantProperties(Poisons.Moonseed);
            SetVariantProperties(Poisons.Drothweed);
            SetVariantProperties(Poisons.Somnalius);
            SetVariantProperties(Poisons.Pyrrhic_Acid);
            SetVariantProperties(Poisons.Magebane);
            SetVariantProperties(Poisons.Thyrwort);
            SetVariantProperties(Poisons.Indulcet);
            SetVariantProperties(Poisons.Sursum);
            SetVariantProperties(Poisons.Quaesto_Vil);
            SetVariantProperties(Poisons.Aegrotat);
        }

        // Poison effects manage their own lifecycle
        protected override int RemoveRound()
        {
            return forcedRoundsRemaining;
        }

        // Always present at least one round remaining so effect system does not remove
        // Set forcedRoundsRemaining to 0 to allow effect to expire once poison has run course or cured externally
        public override int RoundsRemaining
        {
            get { return forcedRoundsRemaining; }
        }

        public override void MagicRound()
        {
            base.MagicRound();

            if (currentState != PoisonStates.Complete)
                UpdatePoison();
            else
                CompletePoison();
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);

            if (PoisonType == Poisons.None)
                return;

            // Store first minute of infection - poisons operate in 1-minute ticks
            lastMinute = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();

                                         // Poison types. 0-7 are weapon poisons. 8-11 are drugs
                                         // 0     1    2     3     4     5    6    7     8    9   10   11
            ushort[] MinMinutesToPoison = { 4,   10,   0,    5,    0,    0,   2,   0,    2,   1,   2,   0 };
            ushort[] MaxMinutesToPoison = { 4,   10,   0,   10,    0,    0,   2,   0,   12,   4,  12,   0 };
            ushort[] MinRoundsOfPoison = {  3,   20,   1,    5,    2,    1,   5,   1,    2,   2,   1,   5 };
            ushort[] MaxRoundsOfPoison = { 10, 1000,   4,   30,   10,    2,  20,   3,    6,   2,   4,  20 };

            int index = (int)PoisonType - startValue;
            minutesToStart = Random.Range(MinMinutesToPoison[index], MaxMinutesToPoison[index] + 1);
            minutesRemaining = Random.Range(MinRoundsOfPoison[index], MaxRoundsOfPoison[index] + 1);

            DaggerfallEntityBehaviour host = GetPeeredEntityBehaviour(manager);
            Debug.Log(host.Entity.Name + " afflicted with " + PoisonType + ", starting in " + minutesToStart + " minutes, lasting for " + minutesRemaining + " minutes.");
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Nothing to do here - poisonType does not stack rounds with self
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            // Comparing keys should be enough for like-kind test
            // Child classes can override test if they need to
            return (other.Key == Key);
        }

        #endregion

        #region Public Methods

        public static string GetClassicPoisonEffectKey(Poisons poisonType)
        {
            return string.Format("Poison-{0}", poisonType.ToString());
        }

        public virtual void CurePoison()
        {
            forcedRoundsRemaining = 0;
            minutesRemaining = 0;
            currentState = PoisonStates.Complete;
            ResignAsIncumbent();
        }

        #endregion

        #region Protected Methods

        protected virtual void UpdatePoison()
        {
            // Do nothing until poison set
            if (PoisonType == Poisons.None)
                return;

            // Get current minute and number of minutes that have passed (e.g. fast travel can progress time several days)
            uint currentMinute = DaggerfallUnity.Instance.WorldTime.DaggerfallDateTime.ToClassicDaggerfallTime();
            int minutesPassed = (int)(currentMinute - lastMinute);

            // Increment poison effect for each game minute passed or until poison is complete
            while (minutesPassed-- > 0 && minutesRemaining > 0 && currentState != PoisonStates.Complete)
            {
                IncrementPoisonEffects();
            }

            // Update minute tracking
            lastMinute = currentMinute;
        }

        protected virtual void CompletePoison()
        {
            // All positive attribute effects from drugs are removed when poison complete
            if (IsDrug && !positiveStatsRemoved)
                RemovePositiveStats();

            // Attribute damage from poisons will persist until player heals attributes or cures poison
            // If poison has completed and all attribute damage is healed then outcome is identical to just curing poison directly
            if (AllAttributesHealed())
                CurePoison();
        }

        protected virtual void IncrementPoisonEffects()
        {
            // Count down to poison start
            if (currentState == PoisonStates.Waiting)
            {
                if (--minutesToStart > 0)
                    return;
                else
                    currentState = PoisonStates.Active;
            }

            // Tick poison effect
            DaggerfallEntityBehaviour host = GetPeeredEntityBehaviour(manager);
            switch (PoisonType)
            {
                case Poisons.Nux_Vomica:
                    host.Entity.DecreaseHealth(Random.Range(2, 12));
                    break;

                case Poisons.Arsenic:
                    host.Entity.DecreaseHealth(2);
                    ChangeStatMod(DFCareer.Stats.Endurance, -1);
                    break;

                case Poisons.Moonseed:
                    host.Entity.DecreaseHealth(Random.Range(1, 10));
                    break;

                case Poisons.Drothweed:
                    ChangeStatMod(DFCareer.Stats.Strength, -Random.Range(5, 10));
                    ChangeStatMod(DFCareer.Stats.Agility, -Random.Range(1, 5));
                    ChangeStatMod(DFCareer.Stats.Speed, -Random.Range(1, 5));
                    break;

                case Poisons.Somnalius:
                    host.Entity.DecreaseFatigue(Random.Range(10, 100), true);
                    break;

                case Poisons.Pyrrhic_Acid:
                    host.Entity.DecreaseHealth(Random.Range(1, 30));
                    break;

                case Poisons.Magebane:
                    ChangeStatMod(DFCareer.Stats.Willpower, -Random.Range(1, 5));
                    host.Entity.DecreaseMagicka(Random.Range(5, 15));
                    break;

                case Poisons.Thyrwort:
                    ChangeStatMod(DFCareer.Stats.Willpower, -Random.Range(5, 20));
                    ChangeStatMod(DFCareer.Stats.Personality, -Random.Range(10, 20));
                    break;

                case Poisons.Indulcet:
                    host.Entity.DecreaseFatigue(Random.Range(10, 100), true);
                    ChangeStatMod(DFCareer.Stats.Luck, Random.Range(4, 10));
                    break;

                case Poisons.Sursum:
                    ChangeStatMod(DFCareer.Stats.Intelligence, -Random.Range(10, 30));
                    ChangeStatMod(DFCareer.Stats.Strength, Random.Range(5, 20));
                    break;

                case Poisons.Quaesto_Vil:
                    ChangeStatMod(DFCareer.Stats.Willpower, -Random.Range(1, 4));
                    host.Entity.IncreaseFatigue(Random.Range(5, 10), true);
                    break;

                case Poisons.Aegrotat:
                    ChangeStatMod(DFCareer.Stats.Endurance, -Random.Range(1, 5));
                    host.Entity.IncreaseMagicka(Random.Range(5, 10));
                    break;
            }

            if (host.Entity == GameManager.Instance.PlayerEntity)
                DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("youFeelSomewhatBad"));

            if (--minutesRemaining <= 0)
                currentState = PoisonStates.Complete;
        }

        protected virtual void SetVariantProperties(Poisons poisonType)
        {
            int variant = (int)poisonType - startValue;
            VariantProperties vp = new VariantProperties();
            vp.effectProperties = properties;
            vp.effectProperties.Key = GetClassicPoisonEffectKey(poisonType);
            vp.effectProperties.ShowSpellIcon = false;
            vp.poisonType = poisonType;
            variantProperties[variant] = vp;
        }

        protected virtual void RemovePositiveStats()
        {
            for (int i = 0; i < StatMods.Length; i++)
            {
                if (StatMods[i] > 0)
                    StatMods[i] = 0;
            }
            positiveStatsRemoved = true;
        }

        protected virtual bool IsDrugType()
        {
            switch(PoisonType)
            {
                case Poisons.Indulcet:
                case Poisons.Sursum:
                case Poisons.Quaesto_Vil:
                case Poisons.Aegrotat:
                    return true;
                default:
                    return false;
            }
        }

        #endregion

        #region Serialization

        [fsObject("v1")]
        public struct SaveData_v1
        {
            public uint lastMinute;
            public int minutesToStart;
            public int minutesRemaining;
            public PoisonStates currentState;
            public int forcedRoundsRemaining;
            public bool positiveStatsRemoved;
        }

        public override object GetSaveData()
        {
            SaveData_v1 data = new SaveData_v1();
            data.lastMinute = lastMinute;
            data.minutesToStart = minutesToStart;
            data.minutesRemaining = minutesRemaining;
            data.currentState = currentState;
            data.forcedRoundsRemaining = forcedRoundsRemaining;
            data.positiveStatsRemoved = positiveStatsRemoved;

            return data;
        }

        public override void RestoreSaveData(object dataIn)
        {
            if (dataIn == null)
                return;

            SaveData_v1 data = (SaveData_v1)dataIn;
            lastMinute = data.lastMinute;
            minutesToStart = data.minutesToStart;
            minutesRemaining = data.minutesRemaining;
            currentState = data.currentState;
            forcedRoundsRemaining = data.forcedRoundsRemaining;
            positiveStatsRemoved = data.positiveStatsRemoved;
        }

        #endregion

    }
}
