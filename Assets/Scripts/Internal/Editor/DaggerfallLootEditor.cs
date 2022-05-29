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

namespace DaggerfallWorkshop.EditorOnly
{
    [CustomEditor(typeof(DaggerfallLoot))]
    [CanEditMultipleObjects]
    public class DaggerfallLootEditor : Editor
    {

        DaggerfallLoot component;

        void OnEnable() => component = target as DaggerfallLoot;

        public override VisualElement CreateInspectorGUI()
        {
            var ROOT = new VisualElement();
            {
                ROOT.Add(new IMGUIContainer(base.OnInspectorGUI));
                ROOT.Add(new UIToolkit.ItemCollectionField(obj: component.Items, itemHeight: 32));
            }
            return ROOT;
        }

    }
}
