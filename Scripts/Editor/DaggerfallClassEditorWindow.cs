// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// View the class template data shared by CLASS*.CFG and ENEMY*.CFG.
    /// </summary>
    public class DaggerfallClassEditor : EditorWindow
    {
        const string windowTitle = "Class Viewer";
        const string menuPath = "Daggerfall Tools/Class Viewer";

        DaggerfallUnity dfUnity;
        DFClass selectedClass;
        Vector2 scrollPos;

        [SerializeField]
        ClassSource classSource = ClassSource.Classes;
        [SerializeField]
        int selectedTemplate = 0;

        [SerializeField]
        DFClass[] classTemplates;
        [SerializeField]
        DFClass[] monsterTemplates;

        [SerializeField]
        GUIContent[] classNames;
        [SerializeField]
        GUIContent[] monsterNames;

        [SerializeField]
        bool showAttributesFoldout = true;
        [SerializeField]
        bool showAdvancementFoldout = true;
        [SerializeField]
        bool showSkillsFoldout = true;
        [SerializeField]
        bool showTolerancesFoldout = true;
        [SerializeField]
        bool showForbiddenMaterialFoldout = true;
        [SerializeField]
        bool showForbiddenShieldsFoldout = true;
        [SerializeField]
        bool showForbiddenArmorFoldout = true;
        [SerializeField]
        bool showProficienciesFoldout = true;
        [SerializeField]
        bool showMagickaFoldout = true;
        [SerializeField]
        bool showMiscellaneousFoldout = true;
        [SerializeField]
        bool showUnknownFoldout = true;

        enum ClassSource
        {
            Classes,
            Monsters,
        }

        [MenuItem(menuPath)]
        static void Init()
        {
            DaggerfallClassEditor window = (DaggerfallClassEditor)EditorWindow.GetWindow(typeof(DaggerfallClassEditor));
#if UNITY_5_0
            window.title = windowTitle;
#elif UNITY_5_1
            window.titleContent = new GUIContent(windowTitle);
#endif
        }

        void OnGUI()
        {
            if (!IsReady())
            {
                EditorGUILayout.HelpBox("DaggerfallUnity instance not ready. Have you set your Arena2 path?", MessageType.Info);
                return;
            }

            // Select class source
            EditorGUILayout.Space();
            classSource = (ClassSource)EditorGUILayout.EnumPopup(new GUIContent("Source"), (ClassSource)classSource);

            // Select class from specified source
            selectedClass = null;
            if (classSource == ClassSource.Classes && classNames != null && classNames.Length > 0)
            {
                if (selectedTemplate > classNames.Length)
                    selectedTemplate = 0;
                selectedTemplate = EditorGUILayout.Popup(new GUIContent("Class"), selectedTemplate, classNames);
                selectedClass = classTemplates[selectedTemplate];
            }
            else if (classSource == ClassSource.Monsters && monsterNames != null && monsterNames.Length > 0)
            {
                if (selectedTemplate > monsterNames.Length)
                    selectedTemplate = 0;
                selectedTemplate = EditorGUILayout.Popup(new GUIContent("Class"), selectedTemplate, monsterNames);
                selectedClass = monsterTemplates[selectedTemplate];
            }
            else
            {
                return;
            }

            // Show foldouts
            if (selectedClass != null)
            {
                scrollPos = GUILayoutHelper.ScrollView(scrollPos, () =>
                {
                    ShowAdvancementGUI();
                    ShowAttributesGUI();
                    ShowSkillsGUI();
                    ShowTolerancesGUI();
                    ShowProficienciesGUI();
                    ShowForbiddenMaterialsGUI();
                    ShowForbiddenArmorGUI();
                    ShowForbiddenShieldsGUI();
                    ShowMagickaGUI();
                    ShowMiscellaneousGUI();
                    //ShowUnknownGUI();
                });
            }
        }

        void ShowAdvancementGUI()
        {
            EditorGUILayout.Space();
            showAdvancementFoldout = GUILayoutHelper.Foldout(showAdvancementFoldout, new GUIContent("Advancement"), () =>
            {
                EditorGUILayout.Space();
                GUILayoutHelper.Indent(() =>
                {
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Hit Points Per Level");
                        EditorGUILayout.SelectableLabel(selectedClass.HitPointsPerLevel.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Difficulty Multiplier");
                        EditorGUILayout.SelectableLabel(selectedClass.AdvancementMultiplier.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                });
            });
        }

        void ShowAttributesGUI()
        {
            EditorGUILayout.Space();
            showAttributesFoldout = GUILayoutHelper.Foldout(showAttributesFoldout, new GUIContent("Attributes"), () =>
            {
                GUILayoutHelper.Indent(() =>
                {
                    EditorGUILayout.Space();
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Strength");
                        EditorGUILayout.SelectableLabel(selectedClass.Strength.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Intelligence");
                        EditorGUILayout.SelectableLabel(selectedClass.Intelligence.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Willpower");
                        EditorGUILayout.SelectableLabel(selectedClass.Willpower.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Agility");
                        EditorGUILayout.SelectableLabel(selectedClass.Agility.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Endurance");
                        EditorGUILayout.SelectableLabel(selectedClass.Endurance.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Personality");
                        EditorGUILayout.SelectableLabel(selectedClass.Personality.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Speed");
                        EditorGUILayout.SelectableLabel(selectedClass.Speed.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Luck");
                        EditorGUILayout.SelectableLabel(selectedClass.Luck.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                });
            });
        }

        void ShowSkillsGUI()
        {
            EditorGUILayout.Space();
            showSkillsFoldout = GUILayoutHelper.Foldout(showSkillsFoldout, new GUIContent("Skills"), () =>
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Primary");
                GUILayoutHelper.Indent(() =>
                {
                    EditorGUILayout.SelectableLabel(selectedClass.PrimarySkill1.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedClass.PrimarySkill2.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedClass.PrimarySkill3.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                });

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Major");
                GUILayoutHelper.Indent(() =>
                {
                    EditorGUILayout.SelectableLabel(selectedClass.MajorSkill1.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedClass.MajorSkill2.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedClass.MajorSkill3.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                });

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Minor");
                GUILayoutHelper.Indent(() =>
                {
                    EditorGUILayout.SelectableLabel(selectedClass.MinorSkill1.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedClass.MinorSkill2.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedClass.MinorSkill3.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedClass.MinorSkill4.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedClass.MinorSkill5.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedClass.MinorSkill6.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                });
            });
        }

        void ShowTolerancesGUI()
        {
            EditorGUILayout.Space();
            showTolerancesFoldout = GUILayoutHelper.Foldout(showTolerancesFoldout, new GUIContent("Tolerances"), () =>
            {
                EditorGUILayout.Space();
                GUILayoutHelper.Indent(() =>
                {
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Paralysis");
                        EditorGUILayout.SelectableLabel(selectedClass.Paralysis.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Magic");
                        EditorGUILayout.SelectableLabel(selectedClass.Magic.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Poison");
                        EditorGUILayout.SelectableLabel(selectedClass.Poison.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Fire");
                        EditorGUILayout.SelectableLabel(selectedClass.Fire.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Frost");
                        EditorGUILayout.SelectableLabel(selectedClass.Frost.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Shock");
                        EditorGUILayout.SelectableLabel(selectedClass.Shock.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Disease");
                        EditorGUILayout.SelectableLabel(selectedClass.Disease.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                });
            });
        }

        void ShowForbiddenMaterialsGUI()
        {
            EditorGUILayout.Space();
            showForbiddenMaterialFoldout = GUILayoutHelper.Foldout(showForbiddenMaterialFoldout, new GUIContent("Forbidden Materials"), () =>
            {
                EditorGUILayout.Space();
                GUILayoutHelper.Indent(() =>
                {
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Iron");
                        EditorGUILayout.SelectableLabel(selectedClass.IsMaterialForbidden(DFClass.MaterialFlags.Iron).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Steel");
                        EditorGUILayout.SelectableLabel(selectedClass.IsMaterialForbidden(DFClass.MaterialFlags.Steel).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Silver");
                        EditorGUILayout.SelectableLabel(selectedClass.IsMaterialForbidden(DFClass.MaterialFlags.Silver).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Elven");
                        EditorGUILayout.SelectableLabel(selectedClass.IsMaterialForbidden(DFClass.MaterialFlags.Elven).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Dwarven");
                        EditorGUILayout.SelectableLabel(selectedClass.IsMaterialForbidden(DFClass.MaterialFlags.Dwarven).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Mithril");
                        EditorGUILayout.SelectableLabel(selectedClass.IsMaterialForbidden(DFClass.MaterialFlags.Mithril).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Adamantium");
                        EditorGUILayout.SelectableLabel(selectedClass.IsMaterialForbidden(DFClass.MaterialFlags.Adamantium).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Ebony");
                        EditorGUILayout.SelectableLabel(selectedClass.IsMaterialForbidden(DFClass.MaterialFlags.Ebony).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Orcish");
                        EditorGUILayout.SelectableLabel(selectedClass.IsMaterialForbidden(DFClass.MaterialFlags.Orcish).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Daedric");
                        EditorGUILayout.SelectableLabel(selectedClass.IsMaterialForbidden(DFClass.MaterialFlags.Daedric).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                });
            });
        }

        void ShowForbiddenArmorGUI()
        {
            EditorGUILayout.Space();
            showForbiddenArmorFoldout = GUILayoutHelper.Foldout(showForbiddenArmorFoldout, new GUIContent("Forbidden Armors"), () =>
            {
                EditorGUILayout.Space();
                GUILayoutHelper.Indent(() =>
                {
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Leather");
                        EditorGUILayout.SelectableLabel(selectedClass.IsArmorForbidden(DFClass.ArmorFlags.Leather).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Chain");
                        EditorGUILayout.SelectableLabel(selectedClass.IsArmorForbidden(DFClass.ArmorFlags.Chain).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Plate");
                        EditorGUILayout.SelectableLabel(selectedClass.IsArmorForbidden(DFClass.ArmorFlags.Plate).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                });
            });
        }

        void ShowForbiddenShieldsGUI()
        {
            EditorGUILayout.Space();
            showForbiddenShieldsFoldout = GUILayoutHelper.Foldout(showForbiddenShieldsFoldout, new GUIContent("Forbidden Shields"), () =>
            {
                EditorGUILayout.Space();
                GUILayoutHelper.Indent(() =>
                {
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Buckler");
                        EditorGUILayout.SelectableLabel(selectedClass.IsShieldForbidden(DFClass.ShieldFlags.Buckler).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Round Shield");
                        EditorGUILayout.SelectableLabel(selectedClass.IsShieldForbidden(DFClass.ShieldFlags.RoundShield).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Kite Shield");
                        EditorGUILayout.SelectableLabel(selectedClass.IsShieldForbidden(DFClass.ShieldFlags.KiteShield).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Tower Shield");
                        EditorGUILayout.SelectableLabel(selectedClass.IsShieldForbidden(DFClass.ShieldFlags.TowerShield).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                });
            });
        }

        void ShowProficienciesGUI()
        {
            EditorGUILayout.Space();
            showProficienciesFoldout = GUILayoutHelper.Foldout(showProficienciesFoldout, new GUIContent("Proficiencies"), () =>
            {
                EditorGUILayout.Space();
                GUILayoutHelper.Indent(() =>
                {
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Short Blades");
                        EditorGUILayout.SelectableLabel(selectedClass.ShortBlades.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Long Blades");
                        EditorGUILayout.SelectableLabel(selectedClass.LongBlades.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Hand To Hand");
                        EditorGUILayout.SelectableLabel(selectedClass.HandToHand.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Axes");
                        EditorGUILayout.SelectableLabel(selectedClass.Axes.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Blunt Weapons");
                        EditorGUILayout.SelectableLabel(selectedClass.BluntWeapons.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Missile Weapons");
                        EditorGUILayout.SelectableLabel(selectedClass.MissileWeapons.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                });
            });
        }

        void ShowMagickaGUI()
        {
            EditorGUILayout.Space();
            showMagickaFoldout = GUILayoutHelper.Foldout(showMagickaFoldout, new GUIContent("Magicka"), () =>
            {
                EditorGUILayout.Space();
                GUILayoutHelper.Indent(() =>
                {
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Increased Magery");
                        EditorGUILayout.SelectableLabel(selectedClass.SpellPointMultiplierValue.ToString("0.00 * INT"), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Darkness Powered Magery");
                        EditorGUILayout.SelectableLabel(selectedClass.DarknessPoweredMagery.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Light Powered Magery");
                        EditorGUILayout.SelectableLabel(selectedClass.LightPoweredMagery.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Spell Absorption");
                        EditorGUILayout.SelectableLabel(selectedClass.SpellAbsorption.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Unable to regenerate spell points");
                        EditorGUILayout.SelectableLabel(selectedClass.NoRegenSpellPoints.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                });
            });
        }

        void ShowMiscellaneousGUI()
        {
            EditorGUILayout.Space();
            showMiscellaneousFoldout = GUILayoutHelper.Foldout(showMiscellaneousFoldout, new GUIContent("Miscellaneous"), () =>
            {
                EditorGUILayout.Space();
                GUILayoutHelper.Indent(() =>
                {
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Acute Hearing");
                        EditorGUILayout.SelectableLabel(selectedClass.AcuteHearing.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Athleticism");
                        EditorGUILayout.SelectableLabel(selectedClass.Athleticism.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Adrenaline Rush");
                        EditorGUILayout.SelectableLabel(selectedClass.AdrenalineRush.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });

                    EditorGUILayout.Space();
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Regeneration");
                        EditorGUILayout.SelectableLabel(selectedClass.Regeneration.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Rapid Healing");
                        EditorGUILayout.SelectableLabel(selectedClass.RapidHealing.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });

                    EditorGUILayout.Space();
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Damage From Sunlight");
                        EditorGUILayout.SelectableLabel(selectedClass.DamageFromSunlight.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Damage From Holy Places");
                        EditorGUILayout.SelectableLabel(selectedClass.DamageFromHolyPlaces.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });

                    EditorGUILayout.Space();
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Undead Attack Modifier");
                        EditorGUILayout.SelectableLabel(selectedClass.UndeadAttackModifier.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Daedra Attack Modifier");
                        EditorGUILayout.SelectableLabel(selectedClass.DaedraAttackModifier.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Humanoid Attack Modifier");
                        EditorGUILayout.SelectableLabel(selectedClass.HumanoidAttackModifier.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Animals Attack Modifier");
                        EditorGUILayout.SelectableLabel(selectedClass.AnimalsAttackModifier.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                });
            });
        }

        void ShowUnknownGUI()
        {
            EditorGUILayout.Space();
            showUnknownFoldout = GUILayoutHelper.Foldout(showUnknownFoldout, new GUIContent("Unknown"), () =>
            {
                EditorGUILayout.Space();
                GUILayoutHelper.Indent(() =>
                {
                    EditorGUILayout.LabelField("Unknown1 [1 Bytes]");
                    EditorGUILayout.SelectableLabel(selectedClass.RawData.Unknown1.ToString("X2"), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                });
                GUILayoutHelper.Indent(() =>
                {
                    EditorGUILayout.LabelField("Unknown2 [8 Bytes]");
                    string valuesString = string.Empty;
                    for (int i = 0; i < selectedClass.RawData.Unknown2.Length; i++)
                    {
                        valuesString += selectedClass.RawData.Unknown2[i].ToString("X2") + " ";
                    }
                    EditorGUILayout.SelectableLabel(valuesString, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                });
            });
        }

        bool IsReady()
        {
            dfUnity = DaggerfallUnity.Instance;

            if (!dfUnity.IsReady || string.IsNullOrEmpty(dfUnity.Arena2Path))
                return false;

            // Read all CLASS*.CFG files
            if (classTemplates == null)
            {
                string[] files = Directory.GetFiles(dfUnity.Arena2Path, "class*.cfg");
                if (files != null && files.Length > 0)
                {
                    classTemplates = new DFClass[files.Length];
                    classNames = new GUIContent[files.Length];
                    for (int i = 0; i < files.Length; i++)
                    {
                        ClassFile classFile = new ClassFile(files[i]);
                        classTemplates[i] = classFile.DFClass;
                        classNames[i] = new GUIContent(classTemplates[i].Name);
                    }
                }
            }

            // Read all ENEMY*.CFG files
            if (monsterTemplates == null)
            {
                MonsterFile monsterFile = new MonsterFile();
                if (monsterFile.Load(Path.Combine(dfUnity.Arena2Path, MonsterFile.Filename), FileUsage.UseMemory, true))
                {
                    // First pass locates CFG record indices
                    List<int> cfgIndices = new List<int>();
                    for (int i = 0; i < monsterFile.Count; i++)
                    {
                        string recordName = monsterFile.GetRecordName(i);
                        if (recordName.EndsWith(".cfg", StringComparison.InvariantCultureIgnoreCase))
                            cfgIndices.Add(i);
                    }

                    // Second pass populates arrays
                    monsterTemplates = new DFClass[cfgIndices.Count];
                    monsterNames = new GUIContent[cfgIndices.Count];
                    for (int i = 0; i < cfgIndices.Count; i++)
                    {
                        // Read ENEMY.CFG class file from stream
                        ClassFile classFile = new ClassFile();
                        byte[] data = monsterFile.GetRecordBytes(cfgIndices[i]);
                        MemoryStream stream = new MemoryStream(data);
                        BinaryReader reader = new BinaryReader(stream);
                        classFile.Load(reader);
                        reader.Close();

                        // Add to arrays
                        monsterTemplates[i] = classFile.DFClass;
                        monsterNames[i] = new GUIContent(monsterTemplates[i].Name);
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}