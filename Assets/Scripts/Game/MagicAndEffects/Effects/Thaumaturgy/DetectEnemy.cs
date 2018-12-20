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

using DaggerfallConnect;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Detect Enemy
    /// </summary>
    public class DetectEnemy : DetectEffect
    {
        public static readonly string EffectKey = "Detect-Enemy";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(39, 1);
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "detect");
            properties.SubGroupName = TextManager.Instance.GetText("ClassicEffects", "enemy");
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1597);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1297);
            properties.SupportDuration = true;
            properties.AllowedTargets = TargetTypes.CasterOnly;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Thaumaturgy;
            properties.DurationCosts = MakeEffectCosts(20, 8, 200);
        }

        protected override bool IsLikeKind(IncumbentEffect other)
        {
            return (other is DetectEnemy);
        }

        protected override void AddState(IncumbentEffect incumbent)
        {
            // Stack my rounds onto incumbent
            incumbent.RoundsRemaining += RoundsRemaining;
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);
            DaggerfallUI.Instance.DaggerfallHUD.HUDCompass.RegisterDetector(this);
        }

        public override void End()
        {
            base.End();
            DaggerfallUI.Instance.DaggerfallHUD.HUDCompass.DeregisterDetector(this);
        }

        public override void MagicRound()
        {
            base.MagicRound();

            DetectedObjects = GameManager.Instance.PlayerGPS.GetNearbyObjects(PlayerGPS.NearbyObjectFlags.Enemy);
        }
    }
}
