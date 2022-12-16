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
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Atlas editor window for quickly finding locations.
    /// </summary>
    public class AtlasEditorWindow : EditorWindow
    {
        const string windowTitle = "Atlas";
        const string menuPath = "Daggerfall Tools/Atlas";

        [SerializeField]
        int selectedRegion = 0;
        [SerializeField]
        string[] regionNames = new string[0];

        DaggerfallUnity dfUnity;
        DFRegion regionData;
        int lastSelectedRegion = -1;
        Vector2 scrollPos;
        List<string> locationNames = new List<string>();
        string searchString = string.Empty;

        enum SearchPatterns
        {
            All,
            Cities,
            Dungeons,
            Graveyards,
            Homes,
            Taverns,
            Temples,
        }

        [MenuItem(menuPath)]
        static void Init()
        {
            AtlasEditorWindow window = (AtlasEditorWindow)EditorWindow.GetWindow(typeof(AtlasEditorWindow));
            window.titleContent = new GUIContent(windowTitle);
        }

        void OnGUI()
        {
            if (!IsReady())
            {
                EditorGUILayout.HelpBox("DaggerfallUnity instance not ready. Have you set your Arena2 path?", MessageType.Info);
                return;
            }

            EditorGUILayout.Space();
            selectedRegion = EditorGUILayout.Popup(selectedRegion, regionNames);
            if (selectedRegion != lastSelectedRegion)
            {
                locationNames.Clear();
                ReloadCurrentRegion();
            }

            // Search bar
            GUILayout.BeginHorizontal(GUI.skin.FindStyle("Toolbar"));
            searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
            if (GUILayout.Button("", GUI.skin.FindStyle("ToolbarSeachCancelButton")))
            {
                // Remove focus if cleared
                searchString = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            bool search = false;
            if (string.IsNullOrEmpty(searchString))
            {
                if (locationNames.Count == regionData.LocationCount)
                    locationNames.Clear();

                GUILayoutHelper.Horizontal(() =>
                {
                    if (GUILayout.Button("Cities")) EnumerateLocationsByType(SearchPatterns.Cities);
                    if (GUILayout.Button("Dungeons")) EnumerateLocationsByType(SearchPatterns.Dungeons);
                    if (GUILayout.Button("Graveyards")) EnumerateLocationsByType(SearchPatterns.Graveyards);
                    if (GUILayout.Button("Homes")) EnumerateLocationsByType(SearchPatterns.Homes);
                    if (GUILayout.Button("Taverns")) EnumerateLocationsByType(SearchPatterns.Taverns);
                    if (GUILayout.Button("Temples")) EnumerateLocationsByType(SearchPatterns.Temples);
                });
            }
            else
            {
                if (locationNames.Count != regionData.LocationCount)
                    EnumerateLocationsByType(SearchPatterns.All);
                search = true;
            }

            int totalLocations = 0;
            string regionSlash = regionNames[selectedRegion] + "/";
            scrollPos = GUILayoutHelper.ScrollView(scrollPos, () =>
            {
                for (int i = 0; i < locationNames.Count; i++)
                {
                    string multiName = regionSlash + locationNames[i];
                    if (search)
                    {
                        int index = locationNames[i].IndexOf(searchString, StringComparison.InvariantCultureIgnoreCase);
                        if (index != -1)
                        {
                            ShowLocationItem(multiName);
                            totalLocations++;
                        }
                    }
                    else
                    {
                        ShowLocationItem(multiName);
                        totalLocations++;
                    }
                }
            });

            EditorGUILayout.LabelField("Total locations found: " + totalLocations);
        }

        bool IsReady()
        {
            if (!dfUnity)
                dfUnity = DaggerfallUnity.Instance;

            if (!dfUnity.IsReady || string.IsNullOrEmpty(dfUnity.Arena2Path))
                return false;

            if (regionNames.Length == 0)
            {
                regionNames = (string[])MapsFile.RegionNames.Clone(); // Using non-localized name in atlas editor
                System.Array.Sort(regionNames);
            }

            return true;
        }

        void ShowLocationItem(string multiName)
        {
            EditorGUILayout.SelectableLabel(multiName, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        }

        void ReloadCurrentRegion()
        {
            regionData = dfUnity.ContentReader.MapFileReader.GetRegion(regionNames[selectedRegion]);
            lastSelectedRegion = selectedRegion;
        }

        void EnumerateLocationsByType(SearchPatterns pattern)
        {
            locationNames.Clear();

            if (regionData.LocationCount == 0)
                ReloadCurrentRegion();

            for (int i = 0; i < regionData.LocationCount; i++)
            {
                bool addName = false;
                DFRegion.LocationTypes type = regionData.MapTable[i].LocationType;
                switch (pattern)
                {
                    case SearchPatterns.All:
                        addName = true;
                        break;
                    case SearchPatterns.Cities:
                        if (type == DFRegion.LocationTypes.TownCity ||
                            type == DFRegion.LocationTypes.TownHamlet ||
                            type == DFRegion.LocationTypes.TownVillage)
                        {
                            addName = true;
                        }
                        break;
                    case SearchPatterns.Dungeons:
                        if (type == DFRegion.LocationTypes.DungeonKeep ||
                            type == DFRegion.LocationTypes.DungeonLabyrinth ||
                            type == DFRegion.LocationTypes.DungeonRuin ||
                            type == DFRegion.LocationTypes.Coven)
                        {
                            addName = true;
                        }
                        break;
                    case SearchPatterns.Graveyards:
                        if (type == DFRegion.LocationTypes.Graveyard)
                        {
                            addName = true;
                        }
                        break;
                    case SearchPatterns.Homes:
                        if (type == DFRegion.LocationTypes.HomeFarms ||
                            type == DFRegion.LocationTypes.HomePoor ||
                            type == DFRegion.LocationTypes.HomeWealthy ||
                            type == DFRegion.LocationTypes.HomeYourShips)
                        {
                            addName = true;
                        }
                        break;
                    case SearchPatterns.Taverns:
                        if (type == DFRegion.LocationTypes.Tavern)
                        {
                            addName = true;
                        }
                        break;
                    case SearchPatterns.Temples:
                        if (type == DFRegion.LocationTypes.ReligionCult ||
                            type == DFRegion.LocationTypes.ReligionTemple)
                        {
                            addName = true;
                        }
                        break;
                }
                if (addName)
                    locationNames.Add(regionData.MapNames[i]);
            }

            locationNames.Sort();
        }
    }
}