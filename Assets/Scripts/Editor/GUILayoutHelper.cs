// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    TheLacus
// 
// Notes:
//

using UnityEngine;
using UnityEditor;
using System;

namespace DaggerfallWorkshop
{
    public static class GUILayoutHelper
    {
        public delegate void VoidDelegate();

        public static bool Foldout(bool toggle, GUIContent label, VoidDelegate callback)
        {
            var rect = GUILayoutUtility.GetRect(new GUIContent("\t" + label.text), GUIStyle.none);
            bool result = EditorGUI.Foldout(rect, toggle, label, true);
            if (result)
                callback();
            return result;
        }

        public static void EnableGroup(bool enabled, VoidDelegate callback)
        {
            EditorGUI.BeginDisabledGroup(!enabled);
            callback();
            EditorGUI.EndDisabledGroup();
        }

        public static void Indent(VoidDelegate callback)
        {
            EditorGUI.indentLevel++;
            callback();
            EditorGUI.indentLevel--;
        }

        public static void Indent(int levels, VoidDelegate callback)
        {
            EditorGUI.indentLevel += levels;
            callback();
            EditorGUI.indentLevel -= levels;
        }

        public static void Horizontal(VoidDelegate callback)
        {
            EditorGUILayout.BeginHorizontal();
            callback();
            EditorGUILayout.EndHorizontal();
        }

        public static void Horizontal(Rect parent, string prefixLabel, params Action<Rect>[] rects)
        {
            const float spaceRel = 0.9f;

            if (prefixLabel != null)
                parent = EditorGUI.PrefixLabel(parent, new GUIContent(prefixLabel));

            float fullWidth = parent.width / rects.Length;
            float contentWidth = fullWidth * spaceRel;
            float space = (fullWidth - contentWidth) / 2;

            for (int i = 0; i < rects.Length; i++)
                rects[i](new Rect(parent.x + fullWidth * i + (i == 0 ? 0 : (i == rects.Length - 1 ? space * 2 : space)),
                    parent.y, rects.Length == 1 ? fullWidth : contentWidth, EditorGUIUtility.singleLineHeight));
        }

        public static void Vertical(VoidDelegate callback)
        {
            EditorGUILayout.BeginVertical();
            callback();
            EditorGUILayout.EndVertical();
        }

        public static void Vertical(Rect parent, int linesPerItem, params Action<Rect>[] rects)
        {
            float fullHeight = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * linesPerItem;
            float contentHeight = EditorGUIUtility.singleLineHeight * linesPerItem;
            float space = (fullHeight - contentHeight) / 2;
            for (int i = 0; i < rects.Length; i++)
                rects[i](new Rect(parent.x, parent.y + fullHeight * i + space, parent.width, contentHeight));
        }

        public static Vector2 ScrollView(Vector2 scrollPosition, VoidDelegate callback)
        {
            var newScrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            callback();
            EditorGUILayout.EndScrollView();
            return newScrollPosition;
        }

        // Note: Area cannot be nested
        public static void Area(Rect rect, VoidDelegate callback)
        {
            GUILayout.BeginArea(rect);
            callback();
            GUILayout.EndArea();
        }
    }
}
