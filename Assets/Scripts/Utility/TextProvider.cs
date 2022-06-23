// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors: Numidium
//
// Notes:
//

using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;
using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop.Game.Items;
using DaggerfallWorkshop.Localization;
using UnityEngine.Localization.Tables;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Game.Entity;

namespace DaggerfallWorkshop.Utility
{
    /// <summary>
    /// Interface to a text provider.
    /// Provides common functionality for dealing with various text sources.
    /// </summary>
    public interface ITextProvider
    {
        /// <summary>
        /// Gets tokens from a TEXT.RSC record.
        /// </summary>
        /// <param name="id">Text resource ID.</param>
        /// <returns>Text resource tokens.</returns>
        TextFile.Token[] GetRSCTokens(int id);

        /// <summary>
        /// Gets tokens from RSC localization table with custom string ID and conversion back to RSC tokens.
        /// Does not support fallback to classic TEXT.RSC. Record key must exist in RSC localization table.
        /// </summary>
        /// <param name="id">String table key.</param>
        /// <returns>Text resource tokens.</returns>
        TextFile.Token[] GetRSCTokens(string id);

        /// <summary>
        /// Gets tokens from a randomly selected subrecord.
        /// </summary>
        /// <param name="id">Text resource ID.</param>
        /// <param name="dfRand">Use Daggerfall rand() for random selection.</param>
        /// <returns>Text resource tokens.</returns>
        TextFile.Token[] GetRandomTokens(int id, bool dfRand = false);

        /// <summary>
        /// Creates a custom token array.
        /// </summary>
        /// <param name="formatting">Formatting of each line.</param>
        /// <param name="lines">All text lines.</param>
        /// <returns>Token array.</returns>
        TextFile.Token[] CreateTokens(TextFile.Formatting formatting, params string[] lines);

        /// <summary>
        /// Gets string from token array.
        /// </summary>
        /// <param name="id">Text resource ID.</param>
        /// <returns>String from text resource.</returns>
        string GetText(int id);

        /// <summary>
        /// Gets random string from separated token array.
        /// Example would be flavour text variants when finding dungeon exterior.
        /// </summary>
        /// <param name="id">Text resource ID.</param>
        /// <returns>String randomly selected from variants.</returns>
        string GetRandomText(int id);

        /// <summary>
        /// Gets name of weapon material type.
        /// </summary>
        /// <param name="material">Material type of weapon.</param>
        /// <returns>String for weapon material name.</returns>
        string GetWeaponMaterialName(WeaponMaterialTypes material);

        /// <summary>
        /// Gets name of armor material type.
        /// </summary>
        /// <param name="material">Material type of armor.</param>
        /// <returns>String for armor material name.</returns>
        string GetArmorMaterialName(ArmorMaterialTypes material);

        /// <summary>
        /// Gets text for skill name.
        /// </summary>
        /// <param name="skill">Skill.</param>
        /// <returns>Text for this skill.</returns>
        string GetSkillName(DFCareer.Skills skill);

        /// <summary>
        /// Gets text to be shown in the Skill summary popup
        /// </summary>
        /// <param name="skill">Skill.s</param>
        /// <param name="startPosition">Position in pixel of the first positioning token</param>
        /// <returns>Tokens for the skill.</returns>
        TextFile.Token[] GetSkillSummary(DFCareer.Skills skill, int startPosition);

        /// <summary>
        /// Gets text for stat name.
        /// </summary>
        /// <param name="stat">Stat.</param>
        /// <returns>Text for this stat.</returns>
        string GetStatName(DFCareer.Stats stat);

        /// <summary>
        /// Gets abbreviated text for stat name.
        /// </summary>
        /// <param name="stat">Stat.</param>
        /// <returns>Abbreviated text for this stat.</returns>
        string GetAbbreviatedStatName(DFCareer.Stats stat);

        /// <summary>
        /// Gets text resource ID of stat description.
        /// </summary>
        /// <param name="stat">Stat.</param>
        /// <returns>Text resource ID.</returns>
        int GetStatDescriptionTextID(DFCareer.Stats stat);

        /// <summary>
        /// Gets the name associated with a custom enemy id (ie: not defined in MobileTypes).
        /// Returns null if the enemy id is unknown.
        /// </summary>
        /// <param name="enemyId">Custom enemy id</param>
        /// <returns>Name if the enemy id is known, null otherwise</returns>
        string GetCustomEnemyName(int enemyId);

        /// <summary>
        /// Attempts to read a localized string from a named table collection.
        /// </summary>
        /// <param name="collection">Name of table collection.</param>
        /// <param name="id">ID of string to get.</param>
        /// <param name="result">Localized string result or null/empty.</param>
        /// <returns>True if string found, otherwise false.</returns>
        bool GetLocalizedString(string collection, string id, out string result);

        /// <summary>
        /// Enable or disable verbose localized string debug in player log.
        /// </summary>
        /// <param name="enable">True to enable, false to disable.</param>
        void EnableLocalizedStringDebug(bool enable);

    }

    /// <summary>
    /// Implementation of a text provider.
    /// Inherit from this class and override as needed.
    /// </summary>
    public abstract class TextProvider : ITextProvider
    {
        public bool localizedStringDebug = false;

        TextFile rscFile = new TextFile();

        public TextProvider()
        {
        }

        public virtual TextFile.Token[] GetRSCTokens(int id)
        {
            if (localizedStringDebug && !string.IsNullOrEmpty(TextManager.Instance.RuntimeRSCStrings))
                Debug.LogFormat("Trying localized string using RSC collection '{0}'", TextManager.Instance.RuntimeRSCStrings);

            // First attempt to get string from localization
            string localizedString;
            if (GetLocalizedString(TextManager.Instance.RuntimeRSCStrings, id.ToString(), out localizedString))
                return DaggerfallStringTableImporter.ConvertStringToRSCTokens(localizedString);

            if (localizedStringDebug)
                Debug.Log("Failed to get localized string. Fallback to TEXT.RSC");

            if (!rscFile.IsLoaded)
                OpenTextRSCFile();

            byte[] buffer = rscFile.GetBytesById(id);
            if (buffer == null)
                return null;

            return TextFile.ReadTokens(ref buffer, 0, TextFile.Formatting.EndOfRecord);
        }

        public virtual TextFile.Token[] GetRSCTokens(string id)
        {
            if (localizedStringDebug && !string.IsNullOrEmpty(TextManager.Instance.RuntimeRSCStrings))
                Debug.LogFormat("Trying localized string using RSC collection '{0}'", TextManager.Instance.RuntimeRSCStrings);

            // Attempt to get string from localization, no fallback for string IDs
            string localizedString;
            if (GetLocalizedString(TextManager.Instance.RuntimeRSCStrings, id, out localizedString))
                return DaggerfallStringTableImporter.ConvertStringToRSCTokens(localizedString);

            return null;
        }

        public virtual TextFile.Token[] GetRandomTokens(int id, bool dfRand = false)
        {
            TextFile.Token[] sourceTokens = GetRSCTokens(id);

            // Build a list of token subrecords
            List<TextFile.Token> currentStream = new List<TextFile.Token>();
            List<TextFile.Token[]> tokenStreams = new List<TextFile.Token[]>();
            for (int i = 0; i < sourceTokens.Length; i++)
            {
                // If we're at end of subrecord then start a new stream
                if (sourceTokens[i].formatting == TextFile.Formatting.SubrecordSeparator)
                {
                    tokenStreams.Add(currentStream.ToArray());
                    currentStream.Clear();
                    continue;
                }

                // Otherwise keep adding to current stream
                currentStream.Add(sourceTokens[i]);
            }

            // Complete final stream
            tokenStreams.Add(currentStream.ToArray());

            // Select a random token stream
            int index = dfRand ? (int)(DFRandom.rand() % tokenStreams.Count) : UnityEngine.Random.Range(0, tokenStreams.Count);

            // Select the next to last item from the array if the length of the last one is zero
            index = (tokenStreams[index].Length == 0 ? index - 1 : index);

            return tokenStreams[index];
        }

        /// <summary>
        /// Gets string from token array.
        /// </summary>
        /// <param name="id">Text resource ID.</param>
        /// <returns>String from single text resource.</returns>
        public virtual string GetText(int id)
        {
            TextFile.Token[] tokens = GetRSCTokens(id);
            if (tokens == null || tokens.Length == 0)
                return string.Empty;

            return tokens[0].text;
        }

        public virtual string GetRandomText(int id)
        {
            // Collect text items
            List<string> textItems = new List<string>();
            TextFile.Token[] tokens = GetRSCTokens(id);
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i].formatting == TextFile.Formatting.Text)
                    textItems.Add(tokens[i].text);
            }

            // Validate items
            if (textItems.Count == 0)
                return string.Empty;

            // Select random text item
            int index = UnityEngine.Random.Range(0, textItems.Count);

            return textItems[index];
        }

        public virtual TextFile.Token[] CreateTokens(TextFile.Formatting formatting, params string[] lines)
        {
            List<TextFile.Token> tokens = new List<TextFile.Token>();

            foreach(string line in lines)
            {
                tokens.Add(new TextFile.Token(TextFile.Formatting.Text, line));
                tokens.Add(new TextFile.Token(formatting));
            }

            tokens.Add(new TextFile.Token(TextFile.Formatting.EndOfRecord));

            return tokens.ToArray();
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
            result = string.Empty;
            if (string.IsNullOrEmpty(collection))
            {
                if (localizedStringDebug)
                    Debug.Log("Collection name is null or empty.");
                return false;
            }

            StringTable table = null;
            var sd = LocalizationSettings.StringDatabase;
            var op = sd.GetTableAsync(collection);
            op.WaitForCompletion();
            if (op.IsDone)
                table = op.Result;
            else
                Debug.LogErrorFormat("GetLocalizedString() failed on collection='{0}' id='{1}'", collection, id);

            if (table != null)
            {
                var entry = table.GetEntry(id);
                if (entry != null)
                {
                    result = entry.GetLocalizedString();
                    if (localizedStringDebug)
                        Debug.LogFormat("Found localized string for locale {0}\n{1}", LocalizationSettings.SelectedLocale.name, result);
                    return true;
                }
            }
            else
            {
                if (localizedStringDebug)
                    Debug.LogFormat("StringTable collection '{0}' not found", collection);
            }

            return false;
        }

        /// <summary>
        /// Enable or disable verbose localized string debug in player log.
        /// </summary>
        /// <param name="enable">True to enable, false to disable.</param>
        public void EnableLocalizedStringDebug(bool enable)
        {
            localizedStringDebug = enable;
        }

        public string GetWeaponMaterialName(WeaponMaterialTypes material)
        {
            switch(material)
            {
                case WeaponMaterialTypes.Iron:
                    return TextManager.Instance.GetLocalizedText("iron");
                case WeaponMaterialTypes.Steel:
                    return TextManager.Instance.GetLocalizedText("steel");
                case WeaponMaterialTypes.Silver:
                    return TextManager.Instance.GetLocalizedText("silver");
                case WeaponMaterialTypes.Elven:
                    return TextManager.Instance.GetLocalizedText("elven");
                case WeaponMaterialTypes.Dwarven:
                    return TextManager.Instance.GetLocalizedText("dwarven");
                case WeaponMaterialTypes.Mithril:
                    return TextManager.Instance.GetLocalizedText("mithril");
                case WeaponMaterialTypes.Adamantium:
                    return TextManager.Instance.GetLocalizedText("adamantium");
                case WeaponMaterialTypes.Ebony:
                    return TextManager.Instance.GetLocalizedText("ebony");
                case WeaponMaterialTypes.Orcish:
                    return TextManager.Instance.GetLocalizedText("orcish");
                case WeaponMaterialTypes.Daedric:
                    return TextManager.Instance.GetLocalizedText("daedric");
                default:
                    return string.Empty;
            }
        }

        public string GetArmorMaterialName(ArmorMaterialTypes material)
        {
            switch (material)
            {
                case ArmorMaterialTypes.Leather:
                    return TextManager.Instance.GetLocalizedText("leather");
                case ArmorMaterialTypes.Chain:
                case ArmorMaterialTypes.Chain2:
                    return TextManager.Instance.GetLocalizedText("chain");
                case ArmorMaterialTypes.Iron:
                    return TextManager.Instance.GetLocalizedText("iron");
                case ArmorMaterialTypes.Steel:
                    return TextManager.Instance.GetLocalizedText("steel");
                case ArmorMaterialTypes.Silver:
                    return TextManager.Instance.GetLocalizedText("silver");
                case ArmorMaterialTypes.Elven:
                    return TextManager.Instance.GetLocalizedText("elven");
                case ArmorMaterialTypes.Dwarven:
                    return TextManager.Instance.GetLocalizedText("dwarven");
                case ArmorMaterialTypes.Mithril:
                    return TextManager.Instance.GetLocalizedText("mithril");
                case ArmorMaterialTypes.Adamantium:
                    return TextManager.Instance.GetLocalizedText("adamantium");
                case ArmorMaterialTypes.Ebony:
                    return TextManager.Instance.GetLocalizedText("ebony");
                case ArmorMaterialTypes.Orcish:
                    return TextManager.Instance.GetLocalizedText("orcish");
                case ArmorMaterialTypes.Daedric:
                    return TextManager.Instance.GetLocalizedText("daedric");
            }

            // Standard material type value not found.
            // Try again using material value masked to material type only.
            // Some save editors will not write back correct material mask, so we must try to handle this if possible.
            // Clamping range so we don't end up in infinite loop.
            int value = (int)material >> 8;
            value = Mathf.Clamp(value, (int)ArmorMaterialTypes.Iron, (int)ArmorMaterialTypes.Daedric);
            return GetArmorMaterialName((ArmorMaterialTypes)value);
        }

        public string GetSkillName(DFCareer.Skills skill)
        {
            switch (skill)
            {
                case DFCareer.Skills.Medical:
                    return TextManager.Instance.GetLocalizedText("medical");
                case DFCareer.Skills.Etiquette:
                    return TextManager.Instance.GetLocalizedText("etiquette");
                case DFCareer.Skills.Streetwise:
                    return TextManager.Instance.GetLocalizedText("streetwise");
                case DFCareer.Skills.Jumping:
                    return TextManager.Instance.GetLocalizedText("jumping");
                case DFCareer.Skills.Orcish:
                    return TextManager.Instance.GetLocalizedText("orcish");
                case DFCareer.Skills.Harpy:
                    return TextManager.Instance.GetLocalizedText("harpy");
                case DFCareer.Skills.Giantish:
                    return TextManager.Instance.GetLocalizedText("giantish");
                case DFCareer.Skills.Dragonish:
                    return TextManager.Instance.GetLocalizedText("dragonish");
                case DFCareer.Skills.Nymph:
                    return TextManager.Instance.GetLocalizedText("nymph");
                case DFCareer.Skills.Daedric:
                    return TextManager.Instance.GetLocalizedText("daedric");
                case DFCareer.Skills.Spriggan:
                    return TextManager.Instance.GetLocalizedText("spriggan");
                case DFCareer.Skills.Centaurian:
                    return TextManager.Instance.GetLocalizedText("centaurian");
                case DFCareer.Skills.Impish:
                    return TextManager.Instance.GetLocalizedText("impish");
                case DFCareer.Skills.Lockpicking:
                    return TextManager.Instance.GetLocalizedText("lockpicking");
                case DFCareer.Skills.Mercantile:
                    return TextManager.Instance.GetLocalizedText("mercantile");
                case DFCareer.Skills.Pickpocket:
                    return TextManager.Instance.GetLocalizedText("pickpocket");
                case DFCareer.Skills.Stealth:
                    return TextManager.Instance.GetLocalizedText("stealth");
                case DFCareer.Skills.Swimming:
                    return TextManager.Instance.GetLocalizedText("swimming");
                case DFCareer.Skills.Climbing:
                    return TextManager.Instance.GetLocalizedText("climbing");
                case DFCareer.Skills.Backstabbing:
                    return TextManager.Instance.GetLocalizedText("backstabbing");
                case DFCareer.Skills.Dodging:
                    return TextManager.Instance.GetLocalizedText("dodging");
                case DFCareer.Skills.Running:
                    return TextManager.Instance.GetLocalizedText("running");
                case DFCareer.Skills.Destruction:
                    return TextManager.Instance.GetLocalizedText("destruction");
                case DFCareer.Skills.Restoration:
                    return TextManager.Instance.GetLocalizedText("restoration");
                case DFCareer.Skills.Illusion:
                    return TextManager.Instance.GetLocalizedText("illusion");
                case DFCareer.Skills.Alteration:
                    return TextManager.Instance.GetLocalizedText("alteration");
                case DFCareer.Skills.Thaumaturgy:
                    return TextManager.Instance.GetLocalizedText("thaumaturgy");
                case DFCareer.Skills.Mysticism:
                    return TextManager.Instance.GetLocalizedText("mysticism");
                case DFCareer.Skills.ShortBlade:
                    return TextManager.Instance.GetLocalizedText("shortBlade");
                case DFCareer.Skills.LongBlade:
                    return TextManager.Instance.GetLocalizedText("longBlade");
                case DFCareer.Skills.HandToHand:
                    return TextManager.Instance.GetLocalizedText("handToHand");
                case DFCareer.Skills.Axe:
                    return TextManager.Instance.GetLocalizedText("axe");
                case DFCareer.Skills.BluntWeapon:
                    return TextManager.Instance.GetLocalizedText("bluntWeapon");
                case DFCareer.Skills.Archery:
                    return TextManager.Instance.GetLocalizedText("archery");
                case DFCareer.Skills.CriticalStrike:
                    return TextManager.Instance.GetLocalizedText("criticalStrike");
                default:
                    return string.Empty;
            }
        }

        public TextFile.Token[] GetSkillSummary(DFCareer.Skills skill, int startPosition)
        {
            PlayerEntity playerEntity = GameManager.Instance.PlayerEntity;
            bool highlight = playerEntity.GetSkillRecentlyIncreased(skill);

            List<TextFile.Token> tokens = new List<TextFile.Token>();
            TextFile.Formatting formatting = highlight ? TextFile.Formatting.TextHighlight : TextFile.Formatting.Text;

            TextFile.Token skillNameToken = new TextFile.Token();
            skillNameToken.formatting = formatting;
            skillNameToken.text = DaggerfallUnity.Instance.TextProvider.GetSkillName(skill);

            TextFile.Token skillValueToken = new TextFile.Token();
            skillValueToken.formatting = formatting;
            skillValueToken.text = string.Format("{0}%", playerEntity.Skills.GetLiveSkillValue(skill));

            DFCareer.Stats primaryStat = DaggerfallSkills.GetPrimaryStat(skill);
            TextFile.Token skillPrimaryStatToken = new TextFile.Token();
            skillPrimaryStatToken.formatting = formatting;
            skillPrimaryStatToken.text = DaggerfallUnity.Instance.TextProvider.GetAbbreviatedStatName(primaryStat);

            TextFile.Token positioningToken = new TextFile.Token();
            positioningToken.formatting = TextFile.Formatting.PositionPrefix;

            TextFile.Token tabToken = new TextFile.Token();
            tabToken.formatting = TextFile.Formatting.PositionPrefix;

            if (startPosition != 0) // if this is the second column
            {
                positioningToken.x = startPosition;
                tokens.Add(positioningToken);
            }
            tokens.Add(skillNameToken);
            positioningToken.x = startPosition + 85;
            tokens.Add(positioningToken);
            tokens.Add(skillValueToken);
            positioningToken.x = startPosition + 112;
            tokens.Add(positioningToken);
            tokens.Add(skillPrimaryStatToken);

            return tokens.ToArray();
        }

        public string GetStatName(DFCareer.Stats stat)
        {
            switch (stat)
            {
                case DFCareer.Stats.Strength:
                    return TextManager.Instance.GetLocalizedText("strength");
                case DFCareer.Stats.Intelligence:
                    return TextManager.Instance.GetLocalizedText("intelligence");
                case DFCareer.Stats.Willpower:
                    return TextManager.Instance.GetLocalizedText("willpower");
                case DFCareer.Stats.Agility:
                    return TextManager.Instance.GetLocalizedText("agility");
                case DFCareer.Stats.Endurance:
                    return TextManager.Instance.GetLocalizedText("endurance");
                case DFCareer.Stats.Personality:
                    return TextManager.Instance.GetLocalizedText("personality");
                case DFCareer.Stats.Speed:
                    return TextManager.Instance.GetLocalizedText("speed");
                case DFCareer.Stats.Luck:
                    return TextManager.Instance.GetLocalizedText("luck");
                default:
                    return string.Empty;
            }
        }

        public string GetAbbreviatedStatName(DFCareer.Stats stat)
        {
            switch (stat)
            {
                case DFCareer.Stats.Strength:
                    return TextManager.Instance.GetLocalizedText("STR");
                case DFCareer.Stats.Intelligence:
                    return TextManager.Instance.GetLocalizedText("INT");
                case DFCareer.Stats.Willpower:
                    return TextManager.Instance.GetLocalizedText("WIL");
                case DFCareer.Stats.Agility:
                    return TextManager.Instance.GetLocalizedText("AGI");
                case DFCareer.Stats.Endurance:
                    return TextManager.Instance.GetLocalizedText("END");
                case DFCareer.Stats.Personality:
                    return TextManager.Instance.GetLocalizedText("PER");
                case DFCareer.Stats.Speed:
                    return TextManager.Instance.GetLocalizedText("SPD");
                case DFCareer.Stats.Luck:
                    return TextManager.Instance.GetLocalizedText("LUC");
                default:
                    return string.Empty;
            }
        }

        public int GetStatDescriptionTextID(DFCareer.Stats stat)
        {
            switch (stat)
            {
                case DFCareer.Stats.Strength:
                    return 0;
                case DFCareer.Stats.Intelligence:
                    return 1;
                case DFCareer.Stats.Willpower:
                    return 2;
                case DFCareer.Stats.Agility:
                    return 3;
                case DFCareer.Stats.Endurance:
                    return 4;
                case DFCareer.Stats.Personality:
                    return 5;
                case DFCareer.Stats.Speed:
                    return 6;
                case DFCareer.Stats.Luck:
                    return 7;
                default:
                    return -1;
            }
        }

        // This interface function is meant for mods, the default TextProvider implementation does not know any custom enemy
        public string GetCustomEnemyName(int enemyId)
        {
            return null;
        }

        #region Protected Methods

        protected void OpenTextRSCFile()
        {
            rscFile.Load(DaggerfallUnity.Instance.Arena2Path, TextFile.Filename);
        }

        #endregion
    }
}
