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

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Renders an image in place of a text label in book reader UI.
    /// </summary>
    public class ImageLabel : TextLabel
    {
        Texture2D image;
        float imageWidth;
        float imageHeight;
        float scaleFactor;

        public Texture2D Image
        {
            get { return image; }
            set { image = value; RefreshLayout(); }
        }

        public override void Draw()
        {
            if (image == null || image.width == 0 || image.height == 0)
                return;

            // Image position is always centred to page
            RefreshLayout();
            Rect totalRect = Rectangle;
            Rect rect = new Rect(totalRect.x + imageWidth / 2, totalRect.y, imageWidth, imageHeight);
            Size = new Vector2(imageWidth, imageHeight);
            DaggerfallUI.DrawTexture(rect, image, ScaleMode.StretchToFill);
        }

        public override void RefreshLayout()
        {
            if (image == null || image.width == 0 || image.height == 0)
                return;

            // Image size is always half width of page area
            base.RefreshLayout();
            imageWidth = (float)MaxWidth * LocalScale.x / 2f;
            scaleFactor = (float)MaxWidth / (float)image.width;
            imageHeight = (float)image.height * scaleFactor * LocalScale.y / 2f;
            Size = new Vector2(imageWidth, imageHeight);
        }
    }
}