namespace DaggerfallWorkshop.Game.Utility
{
    public static class DaggerfallDistance
    {
        public static IDistance GetDistance()
        {
            return new EditDistance(
                            s => s.Trim().ToLowerInvariant(),
                            // Inserting/deleting separators is cheap
                            c => IsSeparator(c) ? 3 : 10,
                            c => IsSeparator(c) ? 3 : 10,
                            // fixing separators is cheap
                            (c1, c2) =>
                            {
                                if (IsSeparator(c1) && IsSeparator(c2))
                                    return 2;
                                return 15;
                            },
                            (s, stop) =>
                            {
                                // exact prefix is very good
                                if (stop == 0)
                                    return 0;
                                // beginning of word is good
                                char last_char = s[stop - 1];
                                if (IsSeparator(last_char))
                                    return (float)GetNumberOfWords(s, 0, stop);
                                // Otherwise small cost of longer prefixes
                                return stop;
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
                                    return (float)GetNumberOfWords(s, start, l);
                                // Otherwise small cost of longer suffixes
                                return l - start;
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
