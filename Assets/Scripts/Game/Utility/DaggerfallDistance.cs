namespace DaggerfallWorkshop.Game.Utility
{
    public static class DaggerfallDistance
    {
        public static IDistance GetDistance()
        {
            // The cheaper the padding, the closer we get to plain substring search
            const float prefixWordsPadding = 0.8f;
            const float wordPrefixPadding = 0.8f;
            const float wordSuffixPadding = 0.4f;
            const float suffixWordsPadding = 0.4f;

            return new EditDistance(
                            s => s.Trim().ToLowerInvariant(),
                            // Inserting/deleting separators is cheap
                            c => IsSeparator(c) ? 3f : 12f,
                            c => IsSeparator(c) ? 3f : 12f,
                            // fixing separators is cheap
                            (c1, c2) =>
                            {
                                if (IsSeparator(c1) && IsSeparator(c2))
                                    return 3f;
                                return 18f;
                            },
                            (s, stop) =>
                            {
                                // exact prefix is very good
                                if (stop == 0)
                                    return 0;
                                // beginning of word is good
                                char last_char = s[stop - 1];
                                if (IsSeparator(last_char))
                                    return prefixWordsPadding * GetNumberOfWords(s, 0, stop);
                                // Otherwise small cost of longer prefixes
                                int beginningOfWord = stop - 1;
                                while (beginningOfWord > 0 && !IsSeparator(s[beginningOfWord - 1]))
                                    beginningOfWord--;
                                return wordPrefixPadding * (stop - beginningOfWord) + prefixWordsPadding * GetNumberOfWords(s, 0, beginningOfWord);
                            },
                            (s, start) =>
                            {
                                int l = s.Length;
                                // exact suffix is very good
                                if (start == l)
                                    return 0;
                                // ending of word is good
                                char first_char = s[start];
                                if (IsSeparator(first_char))
                                    return suffixWordsPadding * GetNumberOfWords(s, start, l);
                                // Otherwise small cost of longer suffixes
                                int endOfWord = start + 1;
                                while (endOfWord < l && !IsSeparator(s[endOfWord]))
                                    endOfWord++;
                                return wordSuffixPadding * (endOfWord - start) + suffixWordsPadding * GetNumberOfWords(s, endOfWord, l);
                            });
        }

        private static bool IsSeparator(char c)
        {
            return (c == ' ' | c == '-' | c == '\'');
        }

        private static int GetNumberOfWords(string s, int start, int stop)
        {
            int count = 0;
            for (int i = start; i < stop; i++)
                if (IsSeparator(s[i]))
                    count++;

            return count;
        }
    }
}
