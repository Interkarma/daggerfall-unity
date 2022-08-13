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
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Charm
    /// NOTES:
    ///  -Spellbook and spellmaker descriptions for charm effect are incorrect in TEXT.RSC.
    ///  -Charm actually operates just like Pacify Humanoid, but only on enemy classes.
    ///  -Enemy class will remain pacified permanently until player attacks them (confirmed in classic).
    ///  -As Duration has no effect in classic, it is intentionally disabled here. This also slightly lowers base cost.
    ///  -Can enable Duration again later by uncommenting related lines in properties.
    /// </summary>
    public class CharmEffect : BaseEntityEffect
    {
        public static readonly string EffectKey = "Charm";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(34, 255);
            //properties.SupportDuration = true;
            properties.SupportChance = true;
            properties.AllowedTargets = EntityEffectBroker.TargetFlags_Other;
            properties.AllowedElements = EntityEffectBroker.ElementFlags_All;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Thaumaturgy;
            //properties.DurationCosts = MakeEffectCosts(20, 8);
            properties.ChanceCosts = MakeEffectCosts(40, 60);
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("charm");
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1589);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1289);

        public override void MagicRound()
        {
            base.MagicRound();

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Set target non-hostile
            if (IsEnemyClass(entityBehaviour.Entity))
            {
                EnemyMotor enemyMotor = entityBehaviour.GetComponent<EnemyMotor>();
                if (enemyMotor)
                {
                    enemyMotor.IsHostile = false;
                    Debug.LogFormat("Enemy {0} is now non-hostile by Charm", enemyMotor.name);
                }
            }
        }

        bool IsEnemyClass(DaggerfallEntity entity)
        {
            // Validate entity
            if (entity == null || !(entity is EnemyEntity))
                return false;

            // Check target match
            // Note: Charm only works on enemy classes (not monstrous humanoids)
            return DaggerfallEntity.IsClassEnemyId((entity as EnemyEntity).MobileEnemy.ID);
        }
    }
}
