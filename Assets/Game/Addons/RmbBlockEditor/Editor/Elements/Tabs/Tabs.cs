// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor.Elements
{
    public class Tabs : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<Tabs, UxmlTraits>
        {
        }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlIntAttributeDescription selection =
                new UxmlIntAttributeDescription { name = "selection", defaultValue = 0 };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var ate = ve as Tabs;

                ate.selection = selection.GetValueFromBag(bag, cc);
            }
        }

        public int selection { get; set; }

        public Tabs()
        {
            var stylesheet =
                AssetDatabase.LoadAssetAtPath<StyleSheet>(
                    "Assets/Game/Addons/RmbBlockEditor/Editor/Elements/Tabs/Style.uss");
            styleSheets.Add(stylesheet);

            InitializeUI();
        }

        private void InitializeUI()
        {
            var container = new VisualElement { name = "tab-container" };
            container.AddToClassList("row");
            Add(container);
            CheckForTabs();
            RegisterCallback<GeometryChangedEvent>(HandleContentChanged);
            RegisterCallback<AttachToPanelEvent>(HandleAttachedToPanel);
        }

        private void HandleAttachedToPanel(AttachToPanelEvent evt)
        {
            CheckForTabs();
        }

        private void HandleContentChanged(GeometryChangedEvent evt)
        {
            CheckForTabs();
        }

        private void HandleTabClicked(MouseUpEvent evt)
        {
            var button = (Button)evt.currentTarget;
            selection = int.Parse(button.name);
            CheckForTabs();
        }

        private void CheckForTabs()
        {
            var container = this.Query<VisualElement>("tab-container").First();
            container.Clear();

            for (int i = 0; i < contentContainer.childCount; i++)
            {
                VisualElement curr = contentContainer[i];

                if (curr is Tab t)
                {
                    var currTab = (Tab)curr;
                    var tabButton = new Button();
                    tabButton.text = currTab.label;
                    tabButton.name = currTab.value.ToString();
                    tabButton.RegisterCallback<MouseUpEvent>(HandleTabClicked);
                    tabButton.AddToClassList("tab");
                    if (selection == currTab.value)
                    {
                        tabButton.AddToClassList("selected");
                        currTab.RemoveFromClassList("hidden");
                    }
                    else
                    {
                        tabButton.RemoveFromClassList("selected");
                        currTab.AddToClassList("hidden");
                    }

                    container.Add(tabButton);
                }
            }
        }
    }
}