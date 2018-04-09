// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2018 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using UnityEngine;
using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.MagicAndEffects
{
    /// <summary>
    /// Enumerates available magic effects and coordinates their instantiation.
    /// Also coordinates "magic rounds" at 5-second intervals for all entity EntityEffectManager components active in scene.
    /// </summary>
    public class EntityEffectBroker : MonoBehaviour
    {
        #region Fields

        const float roundInterval = 5.0f;

        int magicRoundsSinceStartup = 0;
        float roundTimer = 0f;
        IEnumerable<BaseEntityEffect> magicEffectTemplates;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of 5-second "magic rounds" since startup.
        /// </summary>
        public int MagicRoundsSinceStartup
        {
            get { return magicRoundsSinceStartup; }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Stores an effect key and group/subgroup names.
        /// </summary>
        public struct EffectKeyNamePair
        {
            public string key;
            public int classicGroup;
            public int classicSubGroup;
            public string groupName;
            public string subGroupName;
        }

        #endregion

        #region Unity

        void Start()
        {
            // Enumerate classes implementing an effect and create an instance to use as factory
            // TODO: Provide an external method for mods to register custom effects without reflections
            magicEffectTemplates = ReflectiveEnumerator.GetEnumerableOfType<BaseEntityEffect>();
        }

        void Update()
        {
            // Increment magic round timer when not paused
            if (!GameManager.IsGamePaused)
            {
                roundTimer += Time.deltaTime;
                if (roundTimer > roundInterval)
                {
                    RaiseOnNewMagicRoundEvent();
                    magicRoundsSinceStartup++;
                    roundTimer = 0;
                    //Debug.Log("New magic round starting.");
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets group names of registered effects.
        /// </summary>
        /// <param name="sortAlpha">True to sort group names by alpha.</param>
        /// <returns>Array of group names.</returns>
        public string[] GetGroupNames(bool sortAlpha = true)
        {
            List<string> groupNames = new List<string>();

            // Get group list without duplicates
            foreach(BaseEntityEffect effect in magicEffectTemplates)
            {
                if (!groupNames.Contains(effect.GroupName))
                    groupNames.Add(effect.GroupName);
            }

            // Sort if required
            if (sortAlpha)
                groupNames.Sort();

            return groupNames.ToArray();
        }

        /// <summary>
        /// Gets subgroup names of registered effects.
        /// </summary>
        /// <param name="groupName">The group name to collect subgroups of.</param>
        /// <param name="sortAlpha">True to sort subgroup names by alpha.</param>
        /// <returns>Array of subgroup names.</returns>
        public string[] GetSubGroupNames(string groupName, bool sortAlpha = true)
        {
            List<string> subGroupNames = new List<string>();

            foreach (BaseEntityEffect effect in magicEffectTemplates.Where(effect => effect.GroupName == groupName))
            {
                subGroupNames.Add(effect.SubGroupName);
            }

            // Sort if required
            if (sortAlpha)
                subGroupNames.Sort();

            return subGroupNames.ToArray();
        }

        /// <summary>
        /// Gets all effect key name pairs belonging to group name.
        /// </summary>
        /// <param name="groupName">The group name to collect effects from.</param>
        /// <returns>List of effect key name pairs.</returns>
        public List<EffectKeyNamePair> GetEffectKeyNamePairs(string groupName)
        {
            List<EffectKeyNamePair> keyNamePairs = new List<EffectKeyNamePair>();

            foreach (BaseEntityEffect effect in magicEffectTemplates.Where(effect => effect.GroupName == groupName))
            {
                EffectKeyNamePair knp = new EffectKeyNamePair();
                knp.key = effect.GroupKey;
                knp.classicGroup = effect.ClassicGroup;
                knp.classicSubGroup = effect.ClassicSubGroup;
                knp.groupName = effect.GroupName;
                knp.subGroupName = effect.SubGroupName;
                keyNamePairs.Add(knp);
            }

            return keyNamePairs;
        }

        #endregion

        #region Events

        // OnNewMagicRound
        public delegate void OnNewMagicRoundEventHandler();
        public static event OnNewMagicRoundEventHandler OnNewMagicRound;
        protected virtual void RaiseOnNewMagicRoundEvent()
        {
            if (OnNewMagicRound != null)
                OnNewMagicRound();
        }

        #endregion
    }

    /// <summary>
    /// Find all subclasses of type and create instance.
    /// </summary>
    public static class ReflectiveEnumerator
    {
        static ReflectiveEnumerator() { }

        public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class
        {
            List<T> objects = new List<T>();
            foreach (Type type in
                Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T))))
            {
                objects.Add((T)Activator.CreateInstance(type, constructorArgs));
            }
            return objects;
        }
    }
}
