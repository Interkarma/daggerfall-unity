// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2021 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    Allofich, Hazelnut
// 
// Notes:
//

using UnityEngine;
using UnityEngine.UIElements;

using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.EditorOnly.UIToolkit
{
    public class ItemCollectionField : Foldout
    {

        const string isFoldoutEnabledKey = nameof(ItemCollectionField) + "::" + nameof(isFoldoutEnabledKey);
        const string classItemName = "item-name";
        const string classInventoryImage = "inventory-image";

        public ItemCollectionField(ItemCollection obj, int itemHeight = 64)
        {
            int numItems = obj.Count;

            this.value = PlayerPrefs.GetInt(isFoldoutEnabledKey, 0) != 0;
            this.text = $"[{numItems}] {nameof(DaggerfallLoot.Items)}";

            this.RegisterValueChangedCallback((evt) => PlayerPrefs.SetInt(isFoldoutEnabledKey, evt.newValue ? 1 : 0));
            if (numItems != 0)// @TODO: remove this line once component.Items is a IList (so the ListView is always created)
            {
                // @TODO: component.Items is no IList so can't reference it as itemsSource directly without making a copy
                //        (direct reference is highly prefferable so remove this copy in the future)
                var itemsAsArray = new DaggerfallUnityItem[numItems];
                for (int i = 0; i < numItems; i++)
                    itemsAsArray[i] = obj.GetItem(i);

                var LISTVIEW = new ListView(itemsAsArray, itemHeight, makeItem, bindItem);
                {
                    var style = LISTVIEW.style;
                    style.flexGrow = 1;
                    style.minHeight = itemHeight * Mathf.Min(numItems, 6);
                    style.maxHeight = itemHeight * Mathf.Min(numItems, 12);
                }
                this.Add(LISTVIEW);

                VisualElement makeItem()
                {
                    var ve = new VisualElement();
                    {
                        var style = ve.style;
                        style.flexDirection = FlexDirection.Row;
                        style.alignItems = Align.Center;
                    }
                    {
                        var img = new Image() { name = classInventoryImage, scaleMode = ScaleMode.ScaleToFit };
                        {
                            var style = img.style;
                            style.width = style.height = itemHeight;
                            style.flexShrink = 0;
                        }
                        ve.Add(img);

                        var label = new Label() { name = classItemName };
                        {
                            var style = label.style;
                            style.flexWrap = Wrap.Wrap;
                            style.flexShrink = 1;
                        }
                        ve.Add(label);
                    }

                    return ve;
                }
                void bindItem(VisualElement ve, int i)
                {
                    DaggerfallUnityItem item = itemsAsArray[i];
                    ve.Q<Image>(classInventoryImage).image = DaggerfallUnity.Instance.ItemHelper.GetInventoryImage(item).texture;
                    ve.Q<Label>(classItemName).text = $"{item.LongName}{(item.stackCount == 1 ? "" : $" x {item.stackCount}")}";
                }
            }
        }

    }
}
