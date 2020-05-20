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
            int GetNearestColor(Color32 targetColor, out Color32 color);
        }

        class LeafPalette : IPalette
        {
            private Color32[] colors;

            public LeafPalette(Color32[] colors)
            {
                this.colors = colors;
            }

            public int GetNearestColor(Color32 targetColor, out Color32 color)
            {
                int bestColorIndex = 0;
                int bestFit = colorSqrDistance(targetColor, bestColorIndex);
                for (int i = 1; i < colors.Length; i++)
                {
                    int fit = colorSqrDistance(targetColor, i);
                    if (fit < bestFit)
                    {
                        bestColorIndex = i;
                        bestFit = fit;
                    }
                }
                color = colors[bestColorIndex];
                return bestFit;
            }

            private int colorSqrDistance(Color32 targetColor, int i)
            {
                int diffr = (targetColor.r - colors[i].r);
                int diffg = (targetColor.g - colors[i].g);
                int diffb = (targetColor.b - colors[i].b);
                return diffr * diffr + diffg * diffg + diffb * diffb;
            }
        }

        class SplitPalette : IPalette
        {
            private Func<Color32, int> proj;
            private int splitValue;
            private IPalette belowPalette;
            private IPalette abovePalette;

            public SplitPalette(Color32[] colors, Func<Color32, int> proj, int depth)
            {
                this.proj = proj;
                Array.Sort(colors, (Color32 a, Color32 b) => Math.Sign(proj(a) - proj(b)));
                int middle = (colors.Length + 1) / 2;
                splitValue = (proj(colors[middle - 1]) + proj(colors[middle])) / 2;
                List<Color32> belowColors = new List<Color32>();
                List<Color32> aboveColors = new List<Color32>();
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

            public int GetNearestColor(Color32 targetColor, out Color32 color)
            {
                int diff = proj(targetColor) - splitValue;
                IPalette majorPalette = (diff >= 0) ? abovePalette : belowPalette;

                int majorSqrDistance = majorPalette.GetNearestColor(targetColor, out color);
                if (majorSqrDistance >= diff * diff)
                {
                    IPalette minorPalette = (diff >= 0) ? belowPalette : abovePalette;

                    Color32 minorColor;
                    int minorSqrDistance = minorPalette.GetNearestColor(targetColor, out minorColor);
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

        static public IPalette BuildPalette(Color32[] colors, int depth = 0)
        {
            if (colors.Length <= PaletteCutoff)
                return new LeafPalette(colors);
            else
            {
                SplitPalette palette;
                switch (depth % 3)
                {
                    case 0:
                        palette = new SplitPalette(colors, (Color32 color) => color.r, depth + 1);
                        break;
                    case 1:
                        palette = new SplitPalette(colors, (Color32 color) => color.g, depth + 1);
                        break;
                    case 2:
                        palette = new SplitPalette(colors, (Color32 color) => color.b, depth + 1);
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
