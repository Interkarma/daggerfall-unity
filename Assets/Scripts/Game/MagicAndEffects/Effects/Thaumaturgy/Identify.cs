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
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;
using DaggerfallWorkshop.Game.Formulas;

namespace DaggerfallWorkshop.Game.MagicAndEffects.MagicEffects
{
    /// <summary>
    /// Identify - currently just opens a free identify window
    /// </summary>
    public class Identify : BaseEntityEffect
    {
        public static readonly string EffectKey = "Identify";

        public override void SetProperties()
        {
            properties.Key = EffectKey;
            properties.ClassicKey = MakeClassicKey(40, 255);
            properties.SupportChance = true;
            properties.AllowedTargets = TargetTypes.CasterOnly;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Thaumaturgy;
            properties.ChanceCosts = MakeEffectCosts(40, 100, 28);
            properties.ChanceFunction = ChanceFunction.Custom;
        }

        public override string GroupName => TextManager.Instance.GetLocalizedText("identify");
        public override TextFile.Token[] SpellMakerDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1599);
        public override TextFile.Token[] SpellBookDescription => DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1299);

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);

            // Refund spell point cost for this effect before opening trade window
            // Actual spell point cost is applied when player clicks Identify button in trade window
            // Any other effects bundled with identify on spell will not have their spell point cost refunded
            FormulaHelper.SpellCost cost = FormulaHelper.CalculateEffectCosts(this, settings, caster.Entity);
            if (cost.spellPointCost < 5)
                cost.spellPointCost = 5;
            caster.Entity.IncreaseMagicka(cost.spellPointCost);

            // Get peered entity gameobject
            DaggerfallEntityBehaviour entityBehaviour = GetPeeredEntityBehaviour(manager);
            if (!entityBehaviour)
                return;

            // Target must be player - no effect on other entities
            if (entityBehaviour != GameManager.Instance.PlayerEntityBehaviour)
                return;

            // Open identify trade window in spell mode
            UserInterfaceManager uiManager = DaggerfallUI.UIManager as UserInterfaceManager;
            DaggerfallTradeWindow tradeWindow = (DaggerfallTradeWindow)UIWindowFactory.GetInstanceWithArgs(UIWindowType.Trade, new object[] { uiManager, null, DaggerfallTradeWindow.WindowModes.Identify, null });
            tradeWindow.UsingIdentifySpell = true;
            tradeWindow.IdentifySpellChance = ChanceValue();
            tradeWindow.IdentifySpellCost = cost.spellPointCost;
            uiManager.PushWindow(tradeWindow);
        }
    }
}
