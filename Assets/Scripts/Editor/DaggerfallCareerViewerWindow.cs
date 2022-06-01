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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Game.Formulas;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// View the class template data shared by CLASS*.CFG and ENEMY*.CFG.
    /// </summary>
    public class DaggerfallClassEditor : EditorWindow
    {
        const string windowTitle = "Career Viewer";
        const string menuPath = "Daggerfall Tools/Career Viewer";

        DaggerfallUnity dfUnity;
        DFCareer selectedCareer;
        Vector2 scrollPos;

        [SerializeField]
        CareerSource careerSource = CareerSource.PlayerClasses;
        [SerializeField]
        int selectedTemplate = 0;

        [SerializeField]
        DFCareer[] classTemplates;
        [SerializeField]
        DFCareer[] monsterTemplates;

        [SerializeField]
        GUIContent[] classNames;
        [SerializeField]
        GUIContent[] monsterNames;

        [SerializeField]
        bool showAttributesFoldout = true;
        [SerializeField]
        bool showSecondaryAttributesFoldout = true;
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
        //[SerializeField]
        //bool showUnknownFoldout = true;

        enum CareerSource
        {
            PlayerClasses,
            Monsters,
        }

        [MenuItem(menuPath)]
        static void Init()
        {
            DaggerfallClassEditor window = (DaggerfallClassEditor)EditorWindow.GetWindow(typeof(DaggerfallClassEditor));
            window.titleContent = new GUIContent(windowTitle);
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
            careerSource = (CareerSource)EditorGUILayout.EnumPopup(new GUIContent("Source"), (CareerSource)careerSource);

            // Select class from specified source
            selectedCareer = null;
            if (careerSource == CareerSource.PlayerClasses && classNames != null && classNames.Length > 0)
            {
                if (selectedTemplate > classNames.Length)
                    selectedTemplate = 0;
                selectedTemplate = EditorGUILayout.Popup(new GUIContent("Class"), selectedTemplate, classNames);
                selectedCareer = classTemplates[selectedTemplate];
            }
            else if (careerSource == CareerSource.Monsters && monsterNames != null && monsterNames.Length > 0)
            {
                if (selectedTemplate > monsterNames.Length)
                    selectedTemplate = 0;
                selectedTemplate = EditorGUILayout.Popup(new GUIContent("Class"), selectedTemplate, monsterNames);
                selectedCareer = monsterTemplates[selectedTemplate];
            }
            else
            {
                return;
            }

            // Show foldouts
            if (selectedCareer != null)
            {
                scrollPos = GUILayoutHelper.ScrollView(scrollPos, () =>
                {
                    ShowAdvancementGUI();
                    ShowAttributesGUI();
                    ShowSecondaryAttributesGUI();
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
                        EditorGUILayout.LabelField("Max HP Per Class Level / Number of Monster HP Die Rolls");
                        EditorGUILayout.SelectableLabel(selectedCareer.HitPointsPerLevel.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Difficulty Multiplier");
                        EditorGUILayout.SelectableLabel(selectedCareer.AdvancementMultiplier.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
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
                        EditorGUILayout.SelectableLabel(selectedCareer.Strength.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Intelligence");
                        EditorGUILayout.SelectableLabel(selectedCareer.Intelligence.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Willpower");
                        EditorGUILayout.SelectableLabel(selectedCareer.Willpower.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Agility");
                        EditorGUILayout.SelectableLabel(selectedCareer.Agility.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Endurance");
                        EditorGUILayout.SelectableLabel(selectedCareer.Endurance.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Personality");
                        EditorGUILayout.SelectableLabel(selectedCareer.Personality.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Speed");
                        EditorGUILayout.SelectableLabel(selectedCareer.Speed.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Luck");
                        EditorGUILayout.SelectableLabel(selectedCareer.Luck.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                });
            });
        }

        void ShowSecondaryAttributesGUI()
        {
            EditorGUILayout.Space();
            showSecondaryAttributesFoldout = GUILayoutHelper.Foldout(showSecondaryAttributesFoldout, new GUIContent("Secondary Attributes"), () =>
            {
                GUILayoutHelper.Indent(() =>
                {
                    EditorGUILayout.Space();
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Damage Modifier");
                        EditorGUILayout.SelectableLabel(FormulaHelper.DamageModifier(selectedCareer.Strength).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Max Encumbrance");
                        EditorGUILayout.SelectableLabel(FormulaHelper.MaxEncumbrance(selectedCareer.Strength).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Spell Points");
                        EditorGUILayout.SelectableLabel(FormulaHelper.SpellPoints(selectedCareer.Intelligence, selectedCareer.SpellPointMultiplierValue).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Magic Resist");
                        EditorGUILayout.SelectableLabel(FormulaHelper.MagicResist(selectedCareer.Willpower).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("To Hit Modifier");
                        EditorGUILayout.SelectableLabel(FormulaHelper.ToHitModifier(selectedCareer.Agility).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Hit Points Modifier");
                        EditorGUILayout.SelectableLabel(FormulaHelper.HitPointsModifier(selectedCareer.Endurance).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Healing Rate Modifier");
                        EditorGUILayout.SelectableLabel(FormulaHelper.HealingRateModifier(selectedCareer.Endurance).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    if (careerSource == CareerSource.Monsters)
                    {
                        MobileEnemy enemy;
                        if (EnemyBasics.GetEnemy(selectedCareer.Name, out enemy))
                        {
                            GUILayoutHelper.Horizontal(() =>
                            {
                                string monsterHealth = string.Format("{0}-{1}", enemy.MinHealth, enemy.MaxHealth);
                                EditorGUILayout.LabelField("Monster Health");
                                EditorGUILayout.SelectableLabel(monsterHealth, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                            });
                            GUILayoutHelper.Horizontal(() =>
                            {
                                string monsterDamage = string.Format("{0}-{1}", enemy.MinDamage, enemy.MaxDamage);
                                EditorGUILayout.LabelField("Monster Damage 1");
                                EditorGUILayout.SelectableLabel(monsterDamage, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                            });
                            GUILayoutHelper.Horizontal(() =>
                            {
                                string monsterDamage2 = string.Format("{0}-{1}", enemy.MinDamage2, enemy.MaxDamage2);
                                EditorGUILayout.LabelField("Monster Damage 2");
                                EditorGUILayout.SelectableLabel(monsterDamage2, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                            });
                            GUILayoutHelper.Horizontal(() =>
                            {
                                string monsterDamage3 = string.Format("{0}-{1}", enemy.MinDamage3, enemy.MaxDamage3);
                                EditorGUILayout.LabelField("Monster Damage 3");
                                EditorGUILayout.SelectableLabel(monsterDamage3, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                            });
                            GUILayoutHelper.Horizontal(() =>
                            {
                                EditorGUILayout.LabelField("Required Metal");
                                EditorGUILayout.SelectableLabel(enemy.MinMetalToHit.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                            });
                            GUILayoutHelper.Horizontal(() =>
                            {
                                string level = string.Format("{0}", enemy.Level);
                                EditorGUILayout.LabelField("Level");
                                EditorGUILayout.SelectableLabel(level, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                            });
                            GUILayoutHelper.Horizontal(() =>
                            {
                                string armorValue = string.Format("{0}", enemy.ArmorValue);
                                EditorGUILayout.LabelField("Armor Value");
                                EditorGUILayout.SelectableLabel(armorValue, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                            });
                        }
                    }
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
                    EditorGUILayout.SelectableLabel(selectedCareer.PrimarySkill1.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedCareer.PrimarySkill2.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedCareer.PrimarySkill3.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                });

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Major");
                GUILayoutHelper.Indent(() =>
                {
                    EditorGUILayout.SelectableLabel(selectedCareer.MajorSkill1.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedCareer.MajorSkill2.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedCareer.MajorSkill3.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                });

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Minor");
                GUILayoutHelper.Indent(() =>
                {
                    EditorGUILayout.SelectableLabel(selectedCareer.MinorSkill1.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedCareer.MinorSkill2.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedCareer.MinorSkill3.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedCareer.MinorSkill4.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedCareer.MinorSkill5.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    EditorGUILayout.SelectableLabel(selectedCareer.MinorSkill6.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
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
                        EditorGUILayout.SelectableLabel(selectedCareer.Paralysis.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Magic");
                        EditorGUILayout.SelectableLabel(selectedCareer.Magic.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Poison");
                        EditorGUILayout.SelectableLabel(selectedCareer.Poison.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Fire");
                        EditorGUILayout.SelectableLabel(selectedCareer.Fire.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Frost");
                        EditorGUILayout.SelectableLabel(selectedCareer.Frost.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Shock");
                        EditorGUILayout.SelectableLabel(selectedCareer.Shock.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Disease");
                        EditorGUILayout.SelectableLabel(selectedCareer.Disease.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
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
                        EditorGUILayout.SelectableLabel(selectedCareer.IsMaterialForbidden(DFCareer.MaterialFlags.Iron).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Steel");
                        EditorGUILayout.SelectableLabel(selectedCareer.IsMaterialForbidden(DFCareer.MaterialFlags.Steel).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Silver");
                        EditorGUILayout.SelectableLabel(selectedCareer.IsMaterialForbidden(DFCareer.MaterialFlags.Silver).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Elven");
                        EditorGUILayout.SelectableLabel(selectedCareer.IsMaterialForbidden(DFCareer.MaterialFlags.Elven).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Dwarven");
                        EditorGUILayout.SelectableLabel(selectedCareer.IsMaterialForbidden(DFCareer.MaterialFlags.Dwarven).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Mithril");
                        EditorGUILayout.SelectableLabel(selectedCareer.IsMaterialForbidden(DFCareer.MaterialFlags.Mithril).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Adamantium");
                        EditorGUILayout.SelectableLabel(selectedCareer.IsMaterialForbidden(DFCareer.MaterialFlags.Adamantium).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Ebony");
                        EditorGUILayout.SelectableLabel(selectedCareer.IsMaterialForbidden(DFCareer.MaterialFlags.Ebony).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Orcish");
                        EditorGUILayout.SelectableLabel(selectedCareer.IsMaterialForbidden(DFCareer.MaterialFlags.Orcish).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Daedric");
                        EditorGUILayout.SelectableLabel(selectedCareer.IsMaterialForbidden(DFCareer.MaterialFlags.Daedric).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
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
                        EditorGUILayout.SelectableLabel(selectedCareer.IsArmorForbidden(DFCareer.ArmorFlags.Leather).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Chain");
                        EditorGUILayout.SelectableLabel(selectedCareer.IsArmorForbidden(DFCareer.ArmorFlags.Chain).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Plate");
                        EditorGUILayout.SelectableLabel(selectedCareer.IsArmorForbidden(DFCareer.ArmorFlags.Plate).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
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
                        EditorGUILayout.SelectableLabel(selectedCareer.IsShieldForbidden(DFCareer.ShieldFlags.Buckler).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Round Shield");
                        EditorGUILayout.SelectableLabel(selectedCareer.IsShieldForbidden(DFCareer.ShieldFlags.RoundShield).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Kite Shield");
                        EditorGUILayout.SelectableLabel(selectedCareer.IsShieldForbidden(DFCareer.ShieldFlags.KiteShield).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Tower Shield");
                        EditorGUILayout.SelectableLabel(selectedCareer.IsShieldForbidden(DFCareer.ShieldFlags.TowerShield).ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
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
                        EditorGUILayout.SelectableLabel(selectedCareer.ShortBlades.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Long Blades");
                        EditorGUILayout.SelectableLabel(selectedCareer.LongBlades.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Hand To Hand");
                        EditorGUILayout.SelectableLabel(selectedCareer.HandToHand.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Axes");
                        EditorGUILayout.SelectableLabel(selectedCareer.Axes.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Blunt Weapons");
                        EditorGUILayout.SelectableLabel(selectedCareer.BluntWeapons.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Missile Weapons");
                        EditorGUILayout.SelectableLabel(selectedCareer.MissileWeapons.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
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
                        EditorGUILayout.SelectableLabel(selectedCareer.SpellPointMultiplierValue.ToString("0.00 * INT"), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Darkness Powered Magery");
                        EditorGUILayout.SelectableLabel(selectedCareer.DarknessPoweredMagery.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Light Powered Magery");
                        EditorGUILayout.SelectableLabel(selectedCareer.LightPoweredMagery.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Spell Absorption");
                        EditorGUILayout.SelectableLabel(selectedCareer.SpellAbsorption.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Unable to regenerate spell points");
                        EditorGUILayout.SelectableLabel(selectedCareer.NoRegenSpellPoints.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
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
                        EditorGUILayout.SelectableLabel(selectedCareer.AcuteHearing.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Athleticism");
                        EditorGUILayout.SelectableLabel(selectedCareer.Athleticism.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Adrenaline Rush");
                        EditorGUILayout.SelectableLabel(selectedCareer.AdrenalineRush.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });

                    EditorGUILayout.Space();
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Regeneration");
                        EditorGUILayout.SelectableLabel(selectedCareer.Regeneration.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Rapid Healing");
                        EditorGUILayout.SelectableLabel(selectedCareer.RapidHealing.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });

                    EditorGUILayout.Space();
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Damage From Sunlight");
                        EditorGUILayout.SelectableLabel(selectedCareer.DamageFromSunlight.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Damage From Holy Places");
                        EditorGUILayout.SelectableLabel(selectedCareer.DamageFromHolyPlaces.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });

                    EditorGUILayout.Space();
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Undead Attack Modifier");
                        EditorGUILayout.SelectableLabel(selectedCareer.UndeadAttackModifier.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Daedra Attack Modifier");
                        EditorGUILayout.SelectableLabel(selectedCareer.DaedraAttackModifier.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Humanoid Attack Modifier");
                        EditorGUILayout.SelectableLabel(selectedCareer.HumanoidAttackModifier.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                    GUILayoutHelper.Horizontal(() =>
                    {
                        EditorGUILayout.LabelField("Animals Attack Modifier");
                        EditorGUILayout.SelectableLabel(selectedCareer.AnimalsAttackModifier.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                    });
                });
            });
        }

        //void ShowUnknownGUI()
        //{
        //    EditorGUILayout.Space();
        //    showUnknownFoldout = GUILayoutHelper.Foldout(showUnknownFoldout, new GUIContent("Unknown"), () =>
        //    {
        //        EditorGUILayout.Space();
        //        GUILayoutHelper.Indent(() =>
        //        {
        //            EditorGUILayout.LabelField("Unknown1 [1 Bytes]");
        //            EditorGUILayout.SelectableLabel(selectedCareer.RawData.Unknown1.ToString("X2"), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        //        });
        //        GUILayoutHelper.Indent(() =>
        //        {
        //            EditorGUILayout.LabelField("Unknown2 [8 Bytes]");
        //            string valuesString = string.Empty;
        //            for (int i = 0; i < selectedCareer.RawData.Unknown2.Length; i++)
        //            {
        //                valuesString += selectedCareer.RawData.Unknown2[i].ToString("X2") + " ";
        //            }
        //            EditorGUILayout.SelectableLabel(valuesString, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        //        });
        //    });
        //}

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
                    classTemplates = new DFCareer[files.Length - 1];
                    classNames = new GUIContent[files.Length - 1];
                    for (int i = 0; i < files.Length - 1; i++)
                    {
                        ClassFile classFile = new ClassFile(files[i]);
                        classTemplates[i] = classFile.Career;
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
                    monsterTemplates = new DFCareer[cfgIndices.Count];
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
                        monsterTemplates[i] = classFile.Career;
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