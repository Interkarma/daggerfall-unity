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
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallWorkshop.Game.UserInterfaceWindows;

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
            properties.GroupName = TextManager.Instance.GetText("ClassicEffects", "identify");
            properties.SubGroupName = string.Empty;
            properties.SpellMakerDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1599);
            properties.SpellBookDescription = DaggerfallUnity.Instance.TextProvider.GetRSCTokens(1299);
            //properties.SupportChance = true;
            properties.AllowedTargets = TargetTypes.CasterOnly;
            properties.AllowedElements = ElementTypes.Magic;
            properties.AllowedCraftingStations = MagicCraftingStations.SpellMaker;
            properties.MagicSkill = DFCareer.MagicSkills.Thaumaturgy;
            //properties.DurationCosts = MakeEffectCosts(40, 100, 28);
        }

        public override void Start(EntityEffectManager manager, DaggerfallEntityBehaviour caster = null)
        {
            base.Start(manager, caster);

            UserInterfaceManager uiManager = DaggerfallUI.UIManager as UserInterfaceManager;
            DaggerfallTradeWindow tradeWindow = new DaggerfallTradeWindow(uiManager, DaggerfallTradeWindow.WindowModes.Identify);
            tradeWindow.UsingIdentifySpell = true;
            uiManager.PushWindow(tradeWindow);
        }
    }
}
