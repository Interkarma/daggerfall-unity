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

namespace DaggerfallWorkshop.UIToolkit
{
    [UnityEngine.Scripting.Preserve]
    public class ItemCollectionField : Foldout
    {

        const string isFoldoutEnabledKey = nameof(ItemCollectionField) + "::" + nameof(isFoldoutEnabledKey);

        public ItemCollectionField(ItemCollection obj)
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

                const int itemHeight = 16;

                var LISTVIEW = new ListView(
                    itemsSource: itemsAsArray,
                    itemHeight: itemHeight,
                    makeItem: () => new Label(),
                    bindItem: (ve, i) => ((Label)ve).text = $"{itemsAsArray[i].ItemName}{(itemsAsArray[i].stackCount == 1 ? "" : $" x {itemsAsArray[i].stackCount}")}"
                );
                {
                    var style = LISTVIEW.style;
                    style.flexGrow = 1;
                    style.minHeight = itemHeight * Mathf.Min(numItems, 6);
                    style.maxHeight = itemHeight * Mathf.Min(numItems, 12);
                }
                this.Add(LISTVIEW);
            }
        }

    }
}
