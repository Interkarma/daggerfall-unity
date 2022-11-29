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
using System.IO;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility.AssetInjection;
using System.Text;

/// <summary>
/// Represents data for localized book format.
/// Can import classic books and export localized file.
/// </summary>
public class LocalizedBook
{
    const string localizedFilenameSuffix = "-LOC";
    const string fileExtension = ".txt";
    const string textFolderName = "Text";
    const string booksFolderName = "Books";

    // Formatting markup
    const string markupJustifyLeft = "[/left]";
    const string markupJustifyCenter = "[/center]";
    const string markupFont = "[/font={0}]";

    // Keywords for save/load localized book format
    const string titleKeyword = "Title";
    const string authorKeyword = "Author";
    const string isNaughtyKeyword = "IsNaughty";
    const string priceKeyword = "Price";
    const string isUniqueKeyword = "IsUnique";
    const string whenVarSetKeyword = "WhenVarSet";
    const string contentKeyword = "Content";

    // Book data fields
    public string Title;        // Title of book
    public string Author;       // Author's name
    public bool IsNaughty;      // Flagged for adult content
    public int Price;           // Base price for merchants
    public bool IsUnique;       // Book only available to mods and will not appear in loot tables or shelves
    public string WhenVarSet;   // Book only available in loot tables once this GlobalVar is set
    public string Content;      // Text content of book

    /// <summary>
    /// Opens either localized or classic book file.
    /// Seeks localized books first then classic books.
    /// </summary>
    /// <param name="filename">Filename of classic book file, e.g. "BOK00042.txt".</param>
    /// <returns>True if successful.</returns>
    public bool OpenBookFile(string filename)
    {
        // Seek localized book file
        if (!OpenLocalizedBookFile(filename))
        {
            // Fallback to classic book file
            if (!OpenClassicBookFile(filename))
                return false;
        }

        return true;
    }

    /// <summary>
    /// Opens a classic book file.
    /// Seeks classic book files from ARENA2 and mods.
    /// Formatting of classic books is unreliable and often broken in classic data.
    /// Opening a localized book cleaned up by a human is always preferred.
    /// </summary>
    /// <param name="filename">Filename of classic book file, e.g. "BOK00042.txt".</param>
    /// <returns>True if successful.</returns>
    public bool OpenClassicBookFile(string filename)
    {
        // Try to open book
        BookFile bookFile = new BookFile();
        if (!BookReplacement.TryImportBook(filename, bookFile) &&
            !bookFile.OpenBook(DaggerfallUnity.Instance.Arena2Path, filename))
            return false;

        // Prepare initial data
        Title = bookFile.Title;
        Author = bookFile.Author;
        IsNaughty = bookFile.IsNaughty;
        Price = bookFile.Price;
        IsUnique = false;
        WhenVarSet = string.Empty;
        Content = string.Empty;

        // Convert page tokens to markup and set book content
        string content = string.Empty;
        for (int page = 0; page < bookFile.PageCount; page++)
        {
            TextFile.Token[] tokens = bookFile.GetPageTokens(page); // Get all tokens on current page
            content += ConvertTokensToString(tokens); // Convert contents of page to book markup
        }
        Content = content;

        return true;
    }

    /// <summary>
    /// Opens a localized book file from StreamingAssets/Text/Books.
    /// As classic books are also .txt files (despite not being true plain text) the localized files append "-LOC" to book filename.
    /// This is to ensure load process can seek correct version of book later when loading localized book files from mods.
    /// </summary>
    /// <param name="filename">Filename of classic or localized book file, e.g. "BOK00042-LOC.txt" or "BOK00042.txt". -LOC is added to filename automatically if missing.</param>
    /// <returns>True if successfull.</returns>
    public bool OpenLocalizedBookFile(string filename)
    {
        // Append -LOC if missing from filename
        string fileNoExt = Path.GetFileNameWithoutExtension(filename);
        if (!fileNoExt.EndsWith(localizedFilenameSuffix))
            filename = fileNoExt + localizedFilenameSuffix + fileExtension;

        // Attempt to load file from StreamingAssets/Text/Books
        // TODO: Also seek localized book file from mods
        string path = Path.Combine(Application.streamingAssetsPath, textFolderName, booksFolderName, filename);
        string[] lines = File.ReadAllLines(path);
        if (lines == null || lines.Length == 0)
            return false;

        return false;
    }

    /// <summary>
    /// Saves localized book file.
    /// </summary>
    /// <param name="filename">Filename of localized book file.</param>
    /// <returns>True if successfull.</returns>
    public void SaveLocalizedBook(string filename)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine(string.Format("{0}: {1}", titleKeyword, Title));
        builder.AppendLine(string.Format("{0}: {1}", authorKeyword, Author));
        builder.AppendLine(string.Format("{0}: {1}", isNaughtyKeyword, IsNaughty));
        builder.AppendLine(string.Format("{0}: {1}", priceKeyword, Price));
        builder.AppendLine(string.Format("{0}: {1}", isUniqueKeyword, IsUnique));
        builder.AppendLine(string.Format("{0}: {1}", whenVarSetKeyword, WhenVarSet));
        builder.AppendLine(string.Format("{0}:\n{1}", contentKeyword, Content));
        File.WriteAllText(filename, builder.ToString());
    }

    /// <summary>
    /// Convert book tokens to markup.
    /// This is a simplified version of RSC markup converter.
    /// </summary>
    /// <param name="tokens">Input tokens.</param>
    /// <returns>String result.</returns>
    string ConvertTokensToString(TextFile.Token[] tokens)
    {
        string text = string.Empty;

        // Classic books use only the following tokens
        for (int i = 0; i < tokens.Length; i++)
        {
            switch (tokens[i].formatting)
            {
                case TextFile.Formatting.Text:
                    text += tokens[i].text;
                    break;
                case TextFile.Formatting.NewLine:
                    text += "\n";
                    break;
                case TextFile.Formatting.JustifyLeft:
                    text += markupJustifyLeft;
                    break;
                case TextFile.Formatting.JustifyCenter:
                    text += markupJustifyCenter;
                    break;
                case TextFile.Formatting.FontPrefix:
                    text += string.Format(markupFont, tokens[i].x);
                    break;
                case TextFile.Formatting.PositionPrefix:
                    // Unused
                    break;
                case TextFile.Formatting.SameLineOffset:
                    // Unused
                    break;
                default:
                    Debug.LogFormat("Unknown book formatting token '{0}'", tokens[i].formatting);
                    break;
            }
        }

        return text;
    }
}