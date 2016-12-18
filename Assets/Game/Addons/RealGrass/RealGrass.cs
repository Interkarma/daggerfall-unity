// Project:         Daggerfall Tools For Unity
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net

using UnityEngine;
using DaggerfallWorkshop.Utility;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Adds grass to the terrain based on the tiles.
    ///
    /// The density of grass is set randomly based on the type of tile. Tiles that are all grass will have a higher
    /// grass billboard density. Tiles that are partially grass will have lower density.
    ///
    /// The position, height, and width of the grass billboards are randomly generated.
    /// </summary>
    public class RealGrass : MonoBehaviour
    {
        public Texture2D greenGrass;
        public Texture2D brownGrass;

        // Description of the grass billboards to render
        DetailPrototype[] _detailPrototype;

        // Ranges for number of grass billboards per terrain tile
        const int MinGrassThick = 3;
        const int MaxGrassThick = 20;
        const int MinGrassSparse = 0;
        const int MaxGrassSparse = 7;
        // Ranges for shape of grass billboards
        const float MinGrassHeight = 0.5f;
        const float MaxGrassHeight = 2.5f;
        const float MinGrassWidth = 0.8f;
        const float MaxGrassWidth = 1.3f;
        // The "spread" of the variety of grass, if you view it as a distribution of heights & widths.
        // The higher this is, the more varied the grass billboards will look.
        // This does not control the density of the grass, but it does control the positions.
        const float GrassVarietyFactor = 4.0f;

        // Convenience/readability enums for how a tile should be filled with grass
        enum Fill
        {
            All,
            UpperSide,
            LowerSide,
            RightSide,
            LeftSide,
            OnlyUpperRight,
            OnlyUpperLeft,
            OnlyLowerRight,
            OnlyLowerLeft,
            RightToLeft,
            LeftToRight,
            NotUpperRight,
            NotLowerRight,
            NotLowerLeft,
            NotUpperLeft,
            None
        }

        void Awake()
        {
            // Disable self if mod not enabled
            if (!DaggerfallUnity.Settings.UncannyValley_RealGrass)
            {
                gameObject.SetActive(false);
                return;
            }

            //Subscribe to the onPromoteTerrainData
            DaggerfallTerrain.OnPromoteTerrainData += AddGrass;

            //Create a holder for our grass
            _detailPrototype = new[]
            {
                new DetailPrototype
                {
                    minHeight = MinGrassHeight,
                    maxHeight = MaxGrassHeight,
                    minWidth = MinGrassWidth,
                    maxWidth = MaxGrassWidth,
                    noiseSpread = GrassVarietyFactor,
                    healthyColor = new Color(0.70f, 0.70f, 0.70f),
                    dryColor = new Color(0.70f, 0.70f, 0.70f),
                    renderMode = DetailRenderMode.GrassBillboard
                }
            };
        }

        /// <summary>
        /// Get a random grass count for thick patches of grass.
        /// </summary>
        /// <returns>Number of grass billboards to draw</returns>
        static int GetThick()
        {
            return Random.Range(MinGrassThick, MaxGrassThick);
        }

        /// <summary>
        /// Get a random grass count for sparse patches of grass.
        /// </summary>
        /// <returns>Number of grass billboards to draw</returns>
        static int GetSparse()
        {
            return Random.Range(MinGrassSparse, MaxGrassSparse);
        }

        /// <summary>
        /// Determines the quadrants that need to be filled. Broken out for readability.
        /// </summary>
        /// <param name="tileCode">The terrain tile information</param>
        /// <returns>The fill type for the terrain tile</returns>
        static Fill GetGrassFillType(byte tileCode)
        {
            switch (tileCode)
            {
                case 8:
                case 9:
                case 10:
                case 11:
                    return Fill.All;

                case 40:
                case 164:
                case 176:
                case 181:
                case 224:
                    return Fill.OnlyUpperLeft;

                case 41:
                case 165:
                case 177:
                case 182:
                case 221:
                    return Fill.OnlyLowerLeft;

                case 42:
                case 166:
                case 178:
                case 183:
                case 222:
                    return Fill.OnlyLowerRight;

                case 43:
                case 167:
                case 179:
                case 180:
                case 223:
                    return Fill.OnlyUpperRight;

                case 44:
                case 66:
                case 84:
                case 160:
                case 168:
                    return Fill.LeftSide;

                case 45:
                case 67:
                case 85:
                case 161:
                case 169:
                    return Fill.LowerSide;

                case 46:
                case 64:
                case 86:
                case 162:
                case 170:
                    return Fill.RightSide;

                case 47:
                case 65:
                case 87:
                case 163:
                case 171:
                    return Fill.UpperSide;

                case 48:
                case 62:
                case 88:
                case 156:
                    return Fill.NotLowerRight;

                case 49:
                case 63:
                case 89:
                case 157:
                    return Fill.NotUpperRight;

                case 50:
                case 60:
                case 90:
                case 158:
                    return Fill.NotUpperLeft;

                case 51:
                case 61:
                case 91:
                case 159:
                    return Fill.NotLowerLeft;

                case 204:
                case 206:
                case 214:
                    return Fill.LeftToRight;

                case 205:
                case 207:
                case 213:
                    return Fill.RightToLeft;
            }
            return Fill.None;
        }

        /// <summary>
        /// Set the grass density into the given grass detail map
        /// </summary>
        /// <param name="grassFill">Quadrants to fill</param>
        /// <param name="y">Y position on terrain detail map to fill</param>
        /// <param name="x">X position on terrain detail map to fill</param>
        /// <param name="grassMap">Terrain detail map to fill with grass counts</param>
        static void SetGrassDensity(Fill grassFill, int y, int x, ref int[,] grassMap)
        {
            switch (grassFill)
            {
                case Fill.All:
                    grassMap[y, x] = GetThick();
                    grassMap[y, x + 1] = GetThick();
                    grassMap[y + 1, x] = GetThick();
                    grassMap[y + 1, x + 1] = GetThick();
                    break;

                case Fill.UpperSide:
                    grassMap[y + 1, x + 1] = GetSparse();
                    grassMap[y + 1, x] = GetSparse();
                    break;

                case Fill.LowerSide:
                    grassMap[y, x + 1] = GetSparse();
                    grassMap[y, x] = GetSparse();
                    break;

                case Fill.RightSide:
                    grassMap[y + 1, x + 1] = GetSparse();
                    grassMap[y, x + 1] = GetSparse();
                    break;

                case Fill.LeftSide:
                    grassMap[y + 1, x] = GetSparse();
                    grassMap[y, x] = GetSparse();
                    break;

                case Fill.OnlyUpperRight:
                    grassMap[y + 1, x + 1] = GetSparse();
                    break;

                case Fill.OnlyUpperLeft:
                    grassMap[y + 1, x] = GetSparse();
                    break;

                case Fill.OnlyLowerRight:
                    grassMap[y, x + 1] = GetSparse();
                    break;

                case Fill.OnlyLowerLeft:
                    grassMap[y, x] = GetSparse();
                    break;

                case Fill.RightToLeft:
                    grassMap[y + 1, x] = GetSparse();
                    grassMap[y, x + 1] = GetSparse();
                    break;

                case Fill.LeftToRight:
                    grassMap[y, x] = GetSparse();
                    grassMap[y + 1, x + 1] = GetSparse();
                    break;

                case Fill.NotUpperRight:
                    grassMap[y, x] = GetSparse();
                    grassMap[y, x + 1] = GetSparse();
                    grassMap[y + 1, x] = GetSparse();
                    break;

                case Fill.NotLowerRight:
                    grassMap[y, x] = GetSparse();
                    grassMap[y + 1, x] = GetSparse();
                    grassMap[y + 1, x + 1] = GetSparse();
                    break;

                case Fill.NotLowerLeft:
                    grassMap[y, x + 1] = GetSparse();
                    grassMap[y + 1, x] = GetSparse();
                    grassMap[y + 1, x + 1] = GetSparse();
                    break;

                case Fill.NotUpperLeft:
                    grassMap[y, x] = GetSparse();
                    grassMap[y, x + 1] = GetSparse();
                    grassMap[y + 1, x + 1] = GetSparse();
                    break;

                case Fill.None:
                default:
                    break;
            }
        }

        // Add Grass
        void AddGrass(DaggerfallTerrain daggerTerrain, TerrainData terrainData)
        {
//            // Used to check performance
//            Stopwatch stopwatch = new Stopwatch();
//            stopwatch.Start();

            //Get the current season
            var currentSeason = DaggerfallUnity.Instance.WorldTime.Now.SeasonValue;

            //Proceed if it's NOT winter, and if the worldClimate contains grass, which is everything above 225, with the exception of 229
            if (currentSeason == DaggerfallDateTime.Seasons.Winter || daggerTerrain.MapData.worldClimate <= 225 ||
                daggerTerrain.MapData.worldClimate == 229)
                return;

            //Switch the grass texture based on the climate
            if (daggerTerrain.MapData.worldClimate == 226 || daggerTerrain.MapData.worldClimate == 227 ||
                daggerTerrain.MapData.worldClimate == 228 || daggerTerrain.MapData.worldClimate == 230)
                _detailPrototype[0].prototypeTexture = brownGrass;
            else
                _detailPrototype[0].prototypeTexture = greenGrass;

            var grassMap = new int[256, 256];

            // The red channel specifies the kind of terrain.
            // This tell us which quadrants of the tile grass can be drawn on.
            for (int i = 0; i < 128; i++)
            {
                for (int j = 0; j < 128; j++)
                {
                    byte tileCode = daggerTerrain.TileMap[i * 128 + j].r;
                    var fillType = GetGrassFillType(tileCode);
                    SetGrassDensity(fillType, i * 2, j * 2, ref grassMap);
                }
            }

            terrainData.detailPrototypes = _detailPrototype;
            terrainData.wavingGrassTint = Color.gray;
            terrainData.SetDetailResolution(256, 8);
            terrainData.SetDetailLayer(0, 0, 0, grassMap);

//            stopwatch.Stop();
//            // Write result
//            Debug.LogWarning("Time elapsed: " + stopwatch.Elapsed.TotalMilliseconds + " ms");
        }
    }
}
