// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2019 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using DaggerfallConnect;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Entity;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Open
    /// </summary>
    public class Open : IncumbentEffect
    {
        public static readonly string EffectKey = "Open";

        int forcedRoundsRemaining = 1;
        bool awakeAlert = true;
        bool castBySkeletonKey = false;

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(17, 255);
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "open");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1565);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1265);
            properties.ShowSpellIcon = false;
            properties.SupportChance = true;
            properties.ChanceFunction = ChanceFunction.Custom;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Self;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_MagicOnly;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Mysticism;
            properties.ChanceCosts = MakeEffectCosts(20, 100);
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            CheckCastBySkeletonKey();
            StartWaitingForDoor();
        }

        public override void Resume(EntityEffectManager.EffectSaveData_v1 effectData, EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Resume(effectData, manager, caster);
            CheckCastBySkeletonKey();
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

        void StartWaitingForDoor()
        {
            // Do nothing if failed - Skeleton's Key always works
            if (!castBySkeletonKey && !RollChance())
            {
                DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "spellEffectFailed"));
                CancelEffect();
                return;
            }

            // Output "Ready to open." if the host manager is player
            if (awakeAlert && manager.EntityBehaviour == GameManager.Instance.PlayerEntityBehaviour)
            {
                DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "readyToOpen"), 1.5f);
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
        public void TriggerOpenEffect(DaggerfallActionDoor actionDoor)
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
                    DaggerfallUI.AddHUDText(TextManager.Instance.GetText(textDatabase, "openFailed"), 1.5f);
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
        /// Cancel effect.
        /// </summary>
        public void CancelEffect()
        {
            forcedRoundsRemaining = 0;
            ResignAsIncumbent();
        }

        void CheckCastBySkeletonKey()
        {
            castBySkeletonKey = 
                ParentBundle.castByItem != null &&
                ParentBundle.castByItem.IsArtifact &&
                ParentBundle.castByItem.WorldTextureArchive == 432 &&
                ParentBundle.castByItem.WorldTextureRecord == 20;

            UnityEngine.Debug.LogFormat("Open.castBySkeletonKey={0}", castBySkeletonKey);
        }
    }
}
