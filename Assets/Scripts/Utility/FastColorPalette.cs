using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaggerfallWorkshop.Utility
{
    // Implement fast color lookup using k-D tree nearest neighbour algorithm
    // https://en.wikipedia.org/wiki/K-d_tree#Nearest_neighbour_search
    static class FastColorPalette
    {
        public interface IPalette
        {
            float GetNearestColor(Color targetColor, out Color color);
        }

        class LeafPalette : IPalette
        {
            private Color[] colors;

            public LeafPalette(Color[] colors)
            {
                this.colors = colors;
            }

            public float GetNearestColor(Color targetColor, out Color color)
            {
                int bestColorIndex = 0;
                float bestFit = colorSqrDistance(targetColor, bestColorIndex);
                for (int i = 1; i < colors.Length; i++)
                {
                    float fit = colorSqrDistance(targetColor, i);
                    if (fit < bestFit)
                    {
                        bestColorIndex = i;
                        bestFit = fit;
                    }
                }
                color = colors[bestColorIndex];
                return bestFit;
            }

            private float colorSqrDistance(Color targetColor, int i)
            {
                float diffr = (targetColor.r - colors[i].r);
                float diffg = (targetColor.g - colors[i].g);
                float diffb = (targetColor.b - colors[i].b);
                return diffr * diffr + diffg * diffg + diffb * diffb;
            }
        }

        class SplitPalette : IPalette
        {
            private Func<Color, float> proj;
            private float splitValue;
            private IPalette belowPalette;
            private IPalette abovePalette;

            public SplitPalette(Color[] colors, Func<Color, float> proj, int depth)
            {
                this.proj = proj;
                Array.Sort(colors, (Color a, Color b) => Math.Sign(proj(a) - proj(b)));
                int middle = (colors.Length + 1) / 2;
                splitValue = (proj(colors[middle - 1]) + proj(colors[middle])) / 2;
                List<Color> belowColors = new List<Color>();
                List<Color> aboveColors = new List<Color>();
                for (int i = 0; i < colors.Length; i++)
                {
                    if (proj(colors[i]) >= splitValue)
                        aboveColors.Add(colors[i]);
                    else
                        belowColors.Add(colors[i]);
                }
                if (belowColors.Count > 0)
                    belowPalette = BuildPalette(belowColors.ToArray(), depth);
                if (aboveColors.Count > 0)
                    abovePalette = BuildPalette(aboveColors.ToArray(), depth);
            }

            public bool IsDegenerated()
            {
                return abovePalette == null || belowPalette == null;
            }

            public float GetNearestColor(Color targetColor, out Color color)
            {
                float diff = proj(targetColor) - splitValue;
                IPalette majorPalette = (diff >= 0) ? abovePalette : belowPalette;

                float majorSqrDistance = majorPalette.GetNearestColor(targetColor, out color);
                if (majorSqrDistance >= diff * diff)
                {
                    IPalette minorPalette = (diff >= 0) ? belowPalette : abovePalette;

                    Color minorColor;
                    float minorSqrDistance = minorPalette.GetNearestColor(targetColor, out minorColor);
                    if (minorSqrDistance < majorSqrDistance)
                    {
                        color = minorColor;
                        return minorSqrDistance;
                    }
                }
                return majorSqrDistance;
            }
        }

        const int PaletteCutoff = 10;

        static public IPalette BuildPalette(Color[] colors, int depth = 0)
        {
            if (colors.Length <= PaletteCutoff)
                return new LeafPalette(colors);
            else
            {
                SplitPalette palette;
                switch (depth % 3)
                {
                    case 0:
                        palette = new SplitPalette(colors, (Color color) => color.r, depth + 1);
                        break;
                    case 1:
                        palette = new SplitPalette(colors, (Color color) => color.g, depth + 1);
                        break;
                    case 2:
                        palette = new SplitPalette(colors, (Color color) => color.b, depth + 1);
                        break;
                    default:
                        throw new NotSupportedException();
                }
                if (palette.IsDegenerated())
                    // Split with a different projection
                    return BuildPalette(colors, depth + 1);
                return palette;
            }
        }

    }

}
