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

using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Open
    /// </summary>
    public class Open : IncumbentEffect
    {
        public static readonly string EffectKey = "Open";

        protected int forcedRoundsRemaining = 1;
        protected bool awakeAlert = true;
        protected bool castByItem = false;
        protected bool castBySkeletonKey = false;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(17, 255);
            properties.ShowSpellIcon = false;
            properties.SupportChance = true;
            properties.ChanceFunction = ChanceFunction.Custom;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Self;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Mysticism;
            properties.ChanceCosts = MakeEffectCosts(20, 100);
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("open");
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1565);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1265);

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            CheckCastByItem();
            StartWaitingForDoor();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            CheckCastByItem();
            StartWaitingForDoor();
        }

        protected override int RemoveRound()
        {
            return forcedRoundsRemaining;
        }

        public override int RoundsRemaining
        {
            get { return forcedRoundsRemaining; }
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return other is Open;
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
        }

        protected virtual void StartWaitingForDoor()
        {
            // Do nothing if spell chance fails
            // Always succeeds chance roll when cast by item but still subject to level vs. door requirement
            if (!castByItem && !castBySkeletonKey && !RollChance())
            {
                DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("spellEffectFailed"));
                CancelEffect();
                return;
            }

            // Output "Ready to open." if the host manager is player
            if (awakeAlert && manager.EntityBehaviour == GameManager.Instance.PlayerEntityBehaviour)
            {
                DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("readyToOpen"), 1.5f);
                awakeAlert = false;
            }
        }

        /// <summary>
        /// Called by entity holding Open incumbent when they activate a door.
        /// For player this is called by PlayerActivate when opening/closing a door.
        /// Enemies cannot use Lock/Open effects at this time.
        /// This effect will automatically open door if closed when spell triggered.
        /// </summary>
        /// <param name="actionDoor">DaggerfallActionDoor activated by entity.</param>
        public virtual void TriggerOpenEffect(DaggerfallActionDoor actionDoor)
        {
            if (forcedRoundsRemaining == 0)
                return;

            bool activatedByPlayer = (manager.EntityBehaviour == GameManager.Instance.PlayerEntityBehaviour);

            if (actionDoor.IsLocked)
            {
                // Unlocks door to level of entity - from spell description "Unlocks chest or door to lock-level of caster."
                // Skeleton's Key can open even magical locks
                if (castBySkeletonKey || actionDoor.CurrentLockValue <= manager.EntityBehaviour.Entity.Level)
                {
                    actionDoor.CurrentLockValue = 0;
                }
                else if (activatedByPlayer)
                {
                    DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("openFailed"), 1.5f);
                }
            }

            if (!actionDoor.IsLocked && actionDoor.IsClosed)
            {
                // Automatically open door if closed and unlocked
                actionDoor.ToggleDoor(activatedByPlayer);
            }

            // Expire effect once door activated
            CancelEffect();
        }

        /// <summary>
        /// Called by entity holding Open incumbent when they activate an exterior door.
        /// For player this is called by PlayerActivate when opening/closing a door.
        /// Enemies cannot use Lock/Open effects at this time.
        /// For the classic effect, the player's level is always checked, even for the Skeleton Key
        /// </summary>
        /// <param name="actionDoor">DaggerfallActionDoor activated by entity.</param>
        /// <returns>True if the door was opened, falsed otherwise</returns>
        public virtual bool TriggerExteriorOpenEffect(int buildingLockValue)
        {
            bool success = true;
            
            // Player level must meet or exceed lock level for success
            if (GameManager.Instance.PlayerEntity.Level < buildingLockValue)
            {
                DaggerfallUI.AddHUDText(TextManager.Instance.GetLocalizedText("openFailed"), 1.5f);
                success = false;
            }

            // Cancel effect
            CancelEffect();

            return success;
        }

        /// <summary>
        /// Cancel effect.
        /// </summary>
        public virtual void CancelEffect()
        {
            forcedRoundsRemaining = 0;
            ResignAsIncumbent();
        }

        protected virtual void CheckCastByItem()
        {
            castByItem = ParentBundle.castByItem != null;

            castBySkeletonKey = 
                ParentBundle.castByItem != null &&
                ParentBundle.castByItem.IsArtifact &&
                ParentBundle.castByItem.WorldTextureArchive == 432 &&
                ParentBundle.castByItem.WorldTextureRecord == 20;

            UnityEngine.Debug.LogFormat("Open.castBySkeletonKey={0}", castBySkeletonKey);
        }
    }
}
