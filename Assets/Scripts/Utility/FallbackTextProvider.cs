// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: kaboissonneault (kaboissonneault@gmail.com)
// Contributors:    
// 
// Notes:
//

using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Items;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Base class that allows derived implementations to only override certain text functionality,
    /// while having the another implementation as a "fallback" for the cases it's not interested in
    /// For mods, this usually means taking the previous DaggerfallUnity.Instance.TextProvider as the fallback,
    /// then replacing the game's text provider with their own implementation based on FallbackTextProvider
    ///
    /// Ex:
    ///
    /// class MyModTextProvider : FallbackTextProvider
    /// {
    ///   public string GetSkillName(DFCareer.Skills skill)
    ///   {
    ///     if(skill == DFCareer.Skills.Mercantile)
    ///         return "Commerce";
    ///     else
    ///         return FallbackProvider.GetSkillName(skill);
    ///   }
    /// }
    ///
    /// public void Awake()
    /// {
    ///     DaggerfallUnity.Instance.TextProvider = new MyModProvider(DaggerfallUnity.Instance.TextProvider);
    /// }
    ///
    /// This pattern allows multiple mods to add their own functionality to the text provider without conflicting
    /// </summary>
    public abstract class FallbackTextProvider : ITextProvider
    {
        ITextProvider fallback;

        protected ITextProvider FallbackProvider
        {
            get { return fallback; }
        }

        public FallbackTextProvider(ITextProvider Fallback)
        {
            fallback = Fallback;
        }

        /// <summary>
        /// Gets tokens from a TEXT.RSC record.
        /// </summary>
        /// <param name="id">Text resource ID.</param>
        /// <returns>Text resource tokens.</returns>
        public virtual TextFile.Token[] GetRSCTokens(int id)
        {
            return fallback.GetRSCTokens(id);
        }

        /// <summary>
        /// Gets tokens from RSC localization table with custom string ID and conversion back to RSC tokens.
        /// Does not support fallback to classic TEXT.RSC. Record key must exist in RSC localization table.
        /// </summary>
        /// <param name="id">String table key.</param>
        /// <returns>Text resource tokens.</returns>
        public virtual TextFile.Token[] GetRSCTokens(string id)
        {
            return fallback.GetRSCTokens(id);
        }

        /// <summary>
        /// Gets tokens from a randomly selected subrecord.
        /// </summary>
        /// <param name="id">Text resource ID.</param>
        /// <param name="dfRand">Use Daggerfall rand() for random selection.</param>
        /// <returns>Text resource tokens.</returns>
        public virtual TextFile.Token[] GetRandomTokens(int id, bool dfRand = false)
        {
            return fallback.GetRandomTokens(id, dfRand);
        }

        /// <summary>
        /// Creates a custom token array.
        /// </summary>
        /// <param name="formatting">Formatting of each line.</param>
        /// <param name="lines">All text lines.</param>
        /// <returns>Token array.</returns>
        public virtual TextFile.Token[] CreateTokens(TextFile.Formatting formatting, params string[] lines)
        {
            return fallback.CreateTokens(formatting, lines);
        }

        /// <summary>
        /// Gets string from token array.
        /// </summary>
        /// <param name="id">Text resource ID.</param>
        /// <returns>String from text resource.</returns>
        public virtual string GetText(int id)
        {
            return fallback.GetText(id);
        }

        /// <summary>
        /// Gets random string from separated token array.
        /// Example would be flavour text variants when finding dungeon exterior.
        /// </summary>
        /// <param name="id">Text resource ID.</param>
        /// <returns>String randomly selected from variants.</returns>
        public virtual string GetRandomText(int id)
        {
            return fallback.GetRandomText(id);
        }

        /// <summary>
        /// Gets name of weapon material type.
        /// </summary>
        /// <param name="material">Material type of weapon.</param>
        /// <returns>String for weapon material name.</returns>
        public virtual string GetWeaponMaterialName(WeaponMaterialTypes material)
        {
            return fallback.GetWeaponMaterialName(material);
        }

        /// <summary>
        /// Gets name of armor material type.
        /// </summary>
        /// <param name="material">Material type of armor.</param>
        /// <returns>String for armor material name.</returns>
        public virtual string GetArmorMaterialName(ArmorMaterialTypes material)
        {
            return fallback.GetArmorMaterialName(material);
        }

        /// <summary>
        /// Gets text for skill name.
        /// </summary>
        /// <param name="skill">Skill.</param>
        /// <returns>Text for this skill.</returns>
        public virtual string GetSkillName(DFCareer.Skills skill)
        {
            return fallback.GetSkillName(skill);
        }

        /// <summary>
        /// Gets text to be shown in the Skill summary popup
        /// </summary>
        /// <param name="skill">Skill.s</param>
        /// <param name="startPosition">Position in pixel of the first positioning token</param>
        /// <returns>Tokens for the skill.</returns>
        public virtual TextFile.Token[] GetSkillSummary(DFCareer.Skills skill, int startPosition)
        {
            return fallback.GetSkillSummary(skill, startPosition);
        }

        /// <summary>
        /// Gets text for stat name.
        /// </summary>
        /// <param name="stat">Stat.</param>
        /// <returns>Text for this stat.</returns>
        public virtual string GetStatName(DFCareer.Stats stat)
        {
            return fallback.GetStatName(stat);
        }

        /// <summary>
        /// Gets abbreviated text for stat name.
        /// </summary>
        /// <param name="stat">Stat.</param>
        /// <returns>Abbreviated text for this stat.</returns>
        public virtual string GetAbbreviatedStatName(DFCareer.Stats stat)
        {
            return fallback.GetAbbreviatedStatName(stat);
        }

        /// <summary>
        /// Gets text resource ID of stat description.
        /// </summary>
        /// <param name="stat">Stat.</param>
        /// <returns>Text resource ID.</returns>
        public virtual int GetStatDescriptionTextID(DFCareer.Stats stat)
        {
            return fallback.GetStatDescriptionTextID(stat);
        }

        /// <summary>
        /// Gets the name associated with a custom enemy id (ie: not defined in MobileTypes).
        /// Returns null if the enemy id is unknown.
        /// </summary>
        /// <param name="enemyId">Custom enemy id</param>
        /// <returns>Name if the enemy id is known, null otherwise</returns>
        public virtual string GetCustomEnemyName(int enemyId)
        {
            return fallback.GetCustomEnemyName(enemyId);
        }

        /// <summary>
        /// Attempts to read a localized string from a named table collection.
        /// </summary>
        /// <param name="collection">Name of table collection.</param>
        /// <param name="id">ID of string to get.</param>
        /// <param name="result">Localized string result or null/empty.</param>
        /// <returns>True if string found, otherwise false.</returns>
        public virtual bool GetLocalizedString(string collection, string id, out string result)
        {
            return fallback.GetLocalizedString(collection, id, out result);
        }

        /// <summary>
        /// Enable or disable verbose localized string debug in player log.
        /// </summary>
        /// <param name="enable">True to enable, false to disable.</param>
        public virtual void EnableLocalizedStringDebug(bool enable)
        {
            fallback.EnableLocalizedStringDebug(enable);
        }
    }
}
