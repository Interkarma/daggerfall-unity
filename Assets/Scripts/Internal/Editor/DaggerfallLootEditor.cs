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
using UnityEditor;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Items;
using DaggerfallConnect;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Utility;
using DaggerfallWorkshop.Game.MagicAndEffects;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallConnect.FallExe;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop
{
    [CustomEditor(typeof(DaggerfallLoot))]
    [CanEditMultipleObjects]
    public class DaggerfallLootEditor : Editor
    {
        DaggerfallLoot component;
        static bool isFoldoutEnabled = false;
        const string isFoldoutEnabledKey = nameof(DaggerfallLootEditor) + "::" + nameof(isFoldoutEnabledKey);

        void OnEnable()
        {
            component = target as DaggerfallLoot;
            isFoldoutEnabled = EditorPrefs.GetBool(isFoldoutEnabledKey, false);
        }
        void OnDisable() => EditorPrefs.SetBool(isFoldoutEnabledKey, isFoldoutEnabled);

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var items = component.Items;
            isFoldoutEnabled = EditorGUILayout.BeginFoldoutHeaderGroup(isFoldoutEnabled, $"[{items.Count}] Items");
            EditorGUI.indentLevel++;
            for (int i = 0; isFoldoutEnabled && i < items.Count; i++)
            {
                var next = items.GetItem(i);
                EditorGUILayout.LabelField($"{next.ItemName}{(next.stackCount == 1 ? "" : $" x {next.stackCount}")}");
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndFoldoutHeaderGroup();
        }
    }
}
