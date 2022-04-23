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
using UnityEditor;

using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop
{
    [CustomEditor(typeof(DaggerfallLoot))]
    [CanEditMultipleObjects]
    public class DaggerfallLootEditor : Editor
    {
        const string isFoldoutEnabledKey = nameof(DaggerfallLootEditor) + "::" + nameof(isFoldoutEnabledKey);
        DaggerfallLoot component;

        void OnEnable()
        {
            component = target as DaggerfallLoot;
        }

        public override VisualElement CreateInspectorGUI()
        {
            var ROOT = new VisualElement();
            {
                ROOT.Add(new IMGUIContainer(base.OnInspectorGUI));

                int numItems = component.Items.Count;

                var FOLDOUT = new Foldout() { text = $"[{numItems}] {nameof(DaggerfallLoot.Items)}", value = EditorPrefs.GetBool(isFoldoutEnabledKey, false) };
                FOLDOUT.RegisterValueChangedCallback((evt) => EditorPrefs.SetBool(isFoldoutEnabledKey, evt.newValue));
                if (numItems != 0)// @TODO: remove this line once component.Items is a IList (so the ListView is always created)
                {
                    // @TODO: component.Items is no IList so can't reference it as itemsSource directly without making a copy
                    //        (direct reference is highly prefferable so remove this copy in the future)
                    var itemsAsArray = new DaggerfallUnityItem[numItems];
                    for (int i = 0; i < numItems; i++)
                        itemsAsArray[i] = component.Items.GetItem(i);

                    int itemHeight = (int)EditorGUIUtility.singleLineHeight;

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
                    FOLDOUT.Add(LISTVIEW);
                }
                ROOT.Add(FOLDOUT);
            }
            return ROOT;
        }

    }
}
