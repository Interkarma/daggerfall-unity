using System.Collections.Generic;

namespace DaggerfallWorkshop.Game.Utility
{
    public struct DistanceMatch
    {
        public readonly string text;
        public readonly float relevance; // 0f: low relevance .. 1f: high relevance

        public DistanceMatch(string answer, float relevance)
        {
            this.text = answer;
            this.relevance = relevance;
        }
    }

    public interface IDistance
    {

        float GetDistance(string s1, string s2, float upperBound = float.PositiveInfinity);
        void SetDictionary(string[] dictionary);
        void SetDictionary(List<string> dictionary);
        DistanceMatch[] FindBestMatches(string needle, int ntop);
    }
}