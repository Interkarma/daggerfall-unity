using UnityEngine;
using System;
using System.Collections;
using DaggerfallWorkshop.Demo.UserInterface;

namespace DaggerfallWorkshop.Demo.UserInterfaceWindows
{
    /// <summary>
    /// Implements the Daggerfall Load Saved Game interface.
    /// </summary>
    public class DaggerfallLoadSavedGameWindow : DaggerfallBaseWindow
    {
        const string nativeImgName = "LOAD00I0.IMG";

        Texture2D nativeTexture;

        public DaggerfallLoadSavedGameWindow(IUserInterfaceManager uiManager)
            : base(uiManager)
        {
        }

        protected override void Setup()
        {
            // Load native texture
            nativeTexture = GetTextureFromImg(nativeImgName);
            if (!nativeTexture)
                throw new Exception("DaggerfallLoadSavedGameWindow: Could not load native texture.");

            // Setup native panel background
            NativePanel.BackgroundTexture = nativeTexture;

            // Set game select buttons
            AddButton(new Vector2(40, 4), new Vector2(80, 50), DaggerfallUIMessages.dfuiSelectSaveGame0, DaggerfallUIMessages.dfuiOpenSaveGame0);
            AddButton(new Vector2(40, 69), new Vector2(80, 50), DaggerfallUIMessages.dfuiSelectSaveGame1, DaggerfallUIMessages.dfuiOpenSaveGame1);
            AddButton(new Vector2(40, 134), new Vector2(80, 50), DaggerfallUIMessages.dfuiSelectSaveGame2, DaggerfallUIMessages.dfuiOpenSaveGame2);
            AddButton(new Vector2(200, 4), new Vector2(80, 50), DaggerfallUIMessages.dfuiSelectSaveGame3, DaggerfallUIMessages.dfuiOpenSaveGame3);
            AddButton(new Vector2(200, 69), new Vector2(80, 50), DaggerfallUIMessages.dfuiSelectSaveGame4, DaggerfallUIMessages.dfuiOpenSaveGame4);
            AddButton(new Vector2(200, 134), new Vector2(80, 50), DaggerfallUIMessages.dfuiSelectSaveGame5, DaggerfallUIMessages.dfuiOpenSaveGame5);

            // Setup load and exit buttons
            AddButton(new Vector2(126, 5), new Vector2(68, 11), DaggerfallUIMessages.dfuiOpenSelectedSaveGame);
            AddButton(new Vector2(133, 150), new Vector2(56, 19), WindowMessages.wmCloseWindow);

            IsSetup = true;
        }
    }
}