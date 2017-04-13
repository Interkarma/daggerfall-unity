// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2016 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// Popup text for notifications, etc.
    /// </summary>
    public class PopupText : Panel
    {
        const float textSpacing = 1;
        const float popDelay = 1.0f;
        const int maxRows = 7;

        LinkedList<TextLabel> textRows = new LinkedList<TextLabel>();
        float timer = 0;
        float nextPopDelay = popDelay;

        public PopupText()
            : base()
        {
            //TextLabel label = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, Vector2.zero, "This is some test text", this);
            //label.HorizontalAlignment = HorizontalAlignment.Center;
        }

        public override void Update()
        {
            base.Update();

            // Remove item from front of list
            timer += Time.deltaTime;
            if (timer > nextPopDelay)
            {
                timer = 0;
                if (textRows.Count > 0)
                    textRows.RemoveFirst();

                // Reset pop delay to default
                nextPopDelay = popDelay;
            }
        }

        public override void Draw()
        {
            base.Draw();

            // Draw text
            int count = 0;
            float y = 4;
            int maxCount = (textRows.Count > maxRows) ? maxRows : textRows.Count;
            IEnumerator enumerator = textRows.GetEnumerator();
            while (enumerator.MoveNext())
            {
                TextLabel label = enumerator.Current as TextLabel;
                if (label != null)
                {
                    label.Position = new Vector2(0, y);
                    label.Draw();
                    y += label.TextHeight + textSpacing;
                }
                if (++count > maxCount)
                    break;
            }
        }

        public void AddText(string text)
        {
            TextLabel label = DaggerfallUI.AddTextLabel(DaggerfallUI.DefaultFont, Vector2.zero, text);
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.Parent = Parent;
            textRows.AddLast(label);
            timer = 0;
        }

        /// <summary>
        /// Adds text with custom delay.
        /// Delay affects this item only. Subsequent text items can override delay.
        /// Delay will return to default after time elapsed.
        /// </summary>
        /// <param name="text">Text to display.</param>
        /// <param name="delayInSeconds">Time in seconds before removing text.</param>
        public void AddText(string text, float delayInSeconds)
        {
            AddText(text);
            nextPopDelay = delayInSeconds;
        }
    }
}
