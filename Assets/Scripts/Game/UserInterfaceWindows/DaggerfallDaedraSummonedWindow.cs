// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Hazelnut
// Contributors:
//
// Notes:
//

using UnityEngine;
using DaggerfallWorkshop.Game.UserInterface;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterfaceWindows
{
    /// <summary>
    /// Class for handling Daedra summoning.
    /// </summary>
    public class DaggerfallDaedraSummonedWindow : DaggerfallBaseWindow, IMacroContextProvider
    {

        Panel messagePanel = new Panel();
        MultiFormatTextLabel label = new MultiFormatTextLabel();

        DaggerfallQuestPopupWindow.DaedraData daedraSummoned;

        public DaggerfallDaedraSummonedWindow(IUserInterfaceManager uiManager, DaggerfallQuestPopupWindow.DaedraData daedraSummoned)
            : base(uiManager)
        {
            ParentPanel.BackgroundColor = Color.clear;
            this.daedraSummoned = daedraSummoned;
        }

        #region Setup Methods

        protected override void Setup()
        {
            if (IsSetup)
                return;

            messagePanel.HorizontalAlignment = HorizontalAlignment.Center;
            messagePanel.VerticalAlignment = VerticalAlignment.Bottom;
            NativePanel.Components.Add(messagePanel);
            messagePanel.BackgroundColor = new Color(1, 0.5f, 0.5f);

            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Middle;
            messagePanel.Components.Add(label);
        }

        #endregion

        #region Service Handling: Daedra Summoning


        #endregion

        #region Macro handling

        public virtual MacroDataSource GetMacroDataSource()
        {
            return new DaedraSummonedMacroDataSource(this);
        }

        /// <summary>
        /// MacroDataSource context sensitive methods for quest popup windows.
        /// </summary>
        protected class DaedraSummonedMacroDataSource : MacroDataSource
        {
            private DaggerfallDaedraSummonedWindow parent;
            public DaedraSummonedMacroDataSource(DaggerfallDaedraSummonedWindow daedraSummoning)
            {
                this.parent = daedraSummoning;
            }

            public override string Daedra()
            {
                FactionFile.FactionData factionData;
                if (GameManager.Instance.PlayerEntity.FactionData.GetFactionData(parent.daedraSummoned.factionId, out factionData))
                    return factionData.name;
                else
                    return "%dae[error]";
            }
        }

        #endregion
    }
}