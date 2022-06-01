// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: TheLacus
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A multi-control with text boxes for entering multiple values in the same line.
    /// Can also be used to draw text boxes in a row/column table.
    /// </summary>
    public class MultiTextBox : Panel
    {
        #region Fields

        float horizontalSpace = 0.05f;
        float verticalSpace = 0.05f;
        int maxItemsPerRow = -1;
        bool enableOutline = true;

        int rows;
        int columns;

        #endregion

        #region Properties

        /// <summary>
        /// All the texboxes used by this control.
        /// </summary>
        public TextBox[] TextBoxes { get; private set; }

        /// <summary>
        /// Set properties to all textboxes.
        /// </summary>
        public Action<TextBox> OnAddTextBoxCallback { private get; set; }

        /// <summary>
        /// Full rect of control.
        /// </summary>
        public Rect Rect
        {
            get { return new Rect(Position, Size); }
            set { Position = value.position; Size = value.size; }
        }
        
        /// <summary>
        /// Space between textboxes based on total length (range 0-1).
        /// </summary>
        public float HorizontalSpace
        {
            get { return horizontalSpace; }
            set { horizontalSpace = Mathf.Clamp01(value); }
        }

        /// <summary>
        /// Space between lines based on total height (range 0-1).
        /// </summary>
        public float VerticalSpace
        {
            get { return verticalSpace; }
            set { verticalSpace = Mathf.Clamp01(value); }
        }

        /// <summary>
        /// Go to next row when this amount is exceeded. Set to -1 to use a single line.
        /// </summary>
        public int MaxItemsPerRow
        {
            get { return maxItemsPerRow; }
            set { maxItemsPerRow = Mathf.Max(-1, value); }
        }

        /// <summary>
        /// Enable outline on all textboxes.
        /// </summary>
        public bool EnableOutline
        {
            get { return enableOutline; }
            set { enableOutline = value; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Make a multi-control with text boxes with default settings.
        /// </summary>
        public MultiTextBox()
            : base()
        {
        }

        /// <summary>
        /// Make a multi-control with text boxes with a callback to init individual textboxes.
        /// </summary>
        public MultiTextBox(Action<TextBox> onAddTextBoxCallback)
            : base()
        {
            this.OnAddTextBoxCallback = onAddTextBoxCallback;
        }

        #endregion

        #region Public Instance Methods

        /// <summary>
        /// Setup a multi-control with empty text boxes.
        /// </summary>
        /// <param name="size">Number of textboxes.</param>
        /// <param name="isNumeric">Accept only numeric chars.</param>
        public void DoLayout(int size, bool isNumeric = false, NumericMode numericMode = NumericMode.Natural)
        {
            DoLayout(Enumerable.Range(0, size).Select(x => string.Empty).ToArray(), isNumeric, numericMode);
        }

        /// <summary>
        /// Setup a multi-control with text boxes for entering multiple strings.
        /// </summary>
        /// <param name="defaultText">Default strings in the requested order.</param>
        public void DoLayout(params string[] defaultText)
        {
            DoLayout(defaultText, false);
        }

        /// <summary>
        /// Setup a multi-control with text boxes for entering multiple int values.
        /// </summary>
        /// <param name="defaultValues">Default values in the requested order.</param>
        public void DoLayout(params int[] defaultValues)
        {
            DoLayout(defaultValues.Select(x => x.ToString()).ToArray(), true, NumericMode.Integer);
        }

        /// <summary>
        /// Setup a multi-control with text boxes for entering multiple float values.
        /// </summary>
        /// <param name="defaultValues">Default values in the requested order.</param>
        public void DoLayout(params float[] defaultValues)
        {
            DoLayout(defaultValues.Select(x => x.ToString()).ToArray(), true, NumericMode.Float);
        }

        /// <summary>
        /// Gets all result strings in the layout order.
        /// </summary>
        public IEnumerable<string> GetInput()
        {
            return TextBoxes.Select(x => x.ResultText);
        }

        /// <summary>
        /// Gets all result values in the layout order.
        /// </summary>
        public IEnumerable<int> GetIntValues()
        {
            return GetInput().Select(x => int.Parse(x));
        }

        /// <summary>
        /// Gets result value at given index.
        /// </summary>
        public int GetIntValue(int index)
        {
            return int.Parse(TextBoxes[index].ResultText);
        }

        /// <summary>
        /// Gets result value at given layout position.
        /// </summary>
        public int GetIntValue(int row, int column)
        {
            return GetIntValue(row * columns + column);
        }

        /// <summary>
        /// Gets result values from a row of size 2.
        /// </summary>
        public DaggerfallWorkshop.Utility.Tuple<int, int> GetIntTuple(int row = 0)
        {
            return DaggerfallWorkshop.Utility.Tuple<int, int>.Make(GetIntValue(row, 0), GetIntValue(row, 1));
        }

        /// <summary>
        /// Gets all result values in the layout order.
        /// </summary>
        public IEnumerable<float> GetFloatValues()
        {
            return GetInput().Select(x => float.Parse(x));
        }

        /// <summary>
        /// Gets result value at given index.
        /// </summary>
        public float GetFloatValue(int index)
        {
            return float.Parse(TextBoxes[index].ResultText);
        }

        /// <summary>
        /// Gets result value at given layout position.
        /// </summary>
        public float GetFloatValue(int row, int column)
        {
            return GetFloatValue(row * columns + column);
        }

        /// <summary>
        /// Gets result values from a row of size 2.
        /// </summary>
        public DaggerfallWorkshop.Utility.Tuple<float, float> GetFloatTuple(int row = 0)
        {
            return DaggerfallWorkshop.Utility.Tuple<float, float>.Make(GetFloatValue(row, 0), GetFloatValue(row, 1));
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Helper which makes a multi-control with text boxes using callbacks.
        /// </summary>
        /// <param name="rect">Full rect of control.</param>
        /// <param name="doLayout">Call an overload of DoLayout() to set default values and make the layout.</param>
        /// <param name="maxItemsPerRow">Number of colums, or -1 to use a single row.</param>
        /// <param name="onAddTextBoxCallback">Set properties to all textboxes.</param>
        /// <returns>A new instance of MultiTextBox</returns>
        public static MultiTextBox Make(Rect rect, Action<MultiTextBox> doLayout, int maxItemsPerRow = -1, Action<TextBox> onAddTextBoxCallback = null)
        {
            var multiTextBox = new MultiTextBox(onAddTextBoxCallback);
            multiTextBox.Rect = rect;
            multiTextBox.maxItemsPerRow = maxItemsPerRow;
            doLayout(multiTextBox);
            return multiTextBox;
        }

        #endregion

        #region Private Methods

        private void DoLayout(string[] defaultText, bool isNumeric, NumericMode numericMode = NumericMode.Natural)
        {
            if (Components.Count > 0)
                Components.Clear();

            columns = maxItemsPerRow == -1 ? defaultText.Length : Mathf.Min(defaultText.Length, maxItemsPerRow);
            float totalWidth = Size.x / columns;   
            float hSpace = totalWidth * horizontalSpace;
            float contentWidth = columns == 1 ? totalWidth : totalWidth - hSpace;

            rows = maxItemsPerRow == -1 ? 1 : Mathf.CeilToInt(defaultText.Length / maxItemsPerRow);
            float totalHeight = Size.y / rows;
            float vSpace = totalHeight * verticalSpace;
            float contentheight = rows == 1 ? totalHeight : totalHeight - vSpace;
            
            TextBoxes = new TextBox[defaultText.Length];
            Vector2 size = new Vector2(contentWidth, contentheight);

            int row = 0;
            int column = 0;
            for (int i = 0; i < defaultText.Length; i++)
            {
                float x;
                if (column == 0)
                {
                    x = 0;
                }
                else if (column < columns)
                {
                    x = totalWidth * column + hSpace / 2;
                }
                else
                {
                    x = Size.x - size.x;
                }

                float y;
                if (row == 0)
                {
                    y = 0;
                }
                else if (row < rows)
                {
                    y = totalHeight * row + vSpace / 2;
                }
                else
                {
                    y = Size.y - size.y;
                }

                TextBoxes[i] = DaggerfallUI.AddTextBoxWithFocus(new Rect(new Vector2(x, y), size), defaultText[i], this);
                TextBoxes[i].Numeric = isNumeric;
                TextBoxes[i].NumericMode = numericMode;
                TextBoxes[i].UseFocus = true;
                TextBoxes[i].Outline.Enabled = enableOutline;
                if (OnAddTextBoxCallback != null)
                    OnAddTextBoxCallback(TextBoxes[i]);

                if (++column == columns)
                {
                    row++;
                    column = 0;
                }
            }
        }

        #endregion
    }
}
