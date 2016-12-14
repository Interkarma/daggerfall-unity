// Project:         Daggerfall Tools For Unity
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Web Site:        http://www.dfworkshop.net

using UnityEngine;
using DaggerfallWorkshop.Utility;
using System.Collections;

namespace DaggerfallWorkshop.Game
{
    /// <summary>
    /// Adds grass to the terrain based on the tiles
    /// </summary>
    public class RealGrass : MonoBehaviour
    {
        public Texture2D greenGrass;
        public Texture2D brownGrass;

        private DetailPrototype[] detailPrototype;

        //Differient values that determine the overal thickness and lenght of the grass, it creates some variation depedning on differient tiles. 
        const int thickLower = 8;
        const int thickHigher = 24;
        const int thinLower = 4;
        const int thinHigher = 12;

        private Color32[] tilemap;
        private int[,] details;
        private DaggerfallDateTime.Seasons currentSeason;

        private void Awake()
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
            detailPrototype = new DetailPrototype[1];
            detailPrototype[0] = new DetailPrototype();

            //All the settings
            detailPrototype[0].minHeight = 0.65f;
            detailPrototype[0].minWidth = 0.65f;
            detailPrototype[0].maxHeight = 1.0f;
            detailPrototype[0].maxWidth = 1.0f;
            detailPrototype[0].noiseSpread = 0.4f;

            detailPrototype[0].healthyColor = new Color(0.70f, 0.70f, 0.70f);
            detailPrototype[0].dryColor = new Color(0.70f, 0.70f, 0.70f);
            detailPrototype[0].renderMode = UnityEngine.DetailRenderMode.GrassBillboard;

        }
        // Add Grass
        private void AddGrass(DaggerfallTerrain daggerTerrain, TerrainData terrainData)
        {
            //			Used to check performance
            //			Stopwatch stopwatch = new Stopwatch();
            //			stopwatch.Start();

            details = new int[256, 256];

            //Get the current season
            currentSeason = DaggerfallUnity.Instance.WorldTime.Now.SeasonValue;

            //Proceed if it's NOT winter, and if the worldClimate contains grass, which is everything above 225, with the exception of 229
            if (currentSeason != DaggerfallDateTime.Seasons.Winter && (daggerTerrain.MapData.worldClimate > 225 && daggerTerrain.MapData.worldClimate != 229))
            {
                //Switch the grass texture based on the climate
                if (daggerTerrain.MapData.worldClimate == 226 || daggerTerrain.MapData.worldClimate == 227 || daggerTerrain.MapData.worldClimate == 228 || daggerTerrain.MapData.worldClimate == 230)
                    detailPrototype[0].prototypeTexture = brownGrass;
                else
                    detailPrototype[0].prototypeTexture = greenGrass;

                tilemap = daggerTerrain.TileMap;
                terrainData.detailPrototypes = detailPrototype;
                terrainData.wavingGrassTint = Color.gray;
                terrainData.SetDetailResolution(256, 8);

                int colorValue;

                //Check all the tiles, Daggerfall uses the red color value to draw tiles
                for (int i = 0; i < 128; i++)
                {
                    for (int j = 0; j < 128; j++)
                    {
                        colorValue = tilemap[(i * 128) + j].r; //For easier checking

                        switch (colorValue)
                        {
                            //Four corner tiles
                            case 8:
                            case 9:
                            case 10:
                            case 11:
                                details[i * 2, j * 2] = Random.Range(thickLower, thickHigher);
                                details[i * 2, (j * 2) + 1] = Random.Range(thickLower, thickHigher);
                                details[(i * 2) + 1, j * 2] = Random.Range(thickLower, thickHigher);
                                details[(i * 2) + 1, (j * 2) + 1] = Random.Range(thickLower, thickHigher);
                                break;

                            //Upper left corner 
                            case 40:
                            case 224:
                            case 164:
                            case 176:
                            case 181:
                                details[(i * 2) + 1, j * 2] = Random.Range(thinLower, thinHigher);
                                break;

                            //Lower left corner 
                            case 41:
                            case 221:
                            case 165:
                            case 177:
                            case 182:
                                details[i * 2, j * 2] = Random.Range(thinLower, thinHigher);
                                break;

                            //Lower right corner 
                            case 42:
                            case 222:
                            case 166:
                            case 178:
                            case 183:
                                details[i * 2, (j * 2) + 1] = Random.Range(thinLower, thinHigher);
                                break;

                            //Upper right corner 
                            case 43:
                            case 223:
                            case 167:
                            case 179:
                            case 180:
                                details[(i * 2) + 1, (j * 2) + 1] = Random.Range(thinLower, thinHigher);
                                break;

                            //Left side
                            case 44:
                            case 66:
                            case 84:
                            case 160:
                            case 168:
                                details[(i * 2) + 1, j * 2] = Random.Range(thinLower, thinHigher);
                                details[i * 2, j * 2] = Random.Range(thinLower, thinHigher);
                                break;

                            //lower side
                            case 45:
                            case 67:
                            case 85:
                            case 161:
                            case 169:
                                details[i * 2, (j * 2) + 1] = Random.Range(thinLower, thinHigher);
                                details[i * 2, j * 2] = Random.Range(thinLower, thinHigher);
                                break;

                            //right side
                            case 46:
                            case 64:
                            case 86:
                            case 162:
                            case 170:
                                details[(i * 2) + 1, (j * 2) + 1] = Random.Range(thinLower, thinHigher);
                                details[i * 2, (j * 2) + 1] = Random.Range(thinLower, thinHigher);
                                break;

                            //upper side
                            case 47:
                            case 65:
                            case 87:
                            case 163:
                            case 171:
                                details[(i * 2) + 1, (j * 2) + 1] = Random.Range(thinLower, thinHigher);
                                details[(i * 2) + 1, j * 2] = Random.Range(thinLower, thinHigher);
                                break;

                            //All expect lower right
                            case 48:
                            case 62:
                            case 88:
                            case 156:
                                details[i * 2, j * 2] = Random.Range(thinLower, thinHigher);
                                details[(i * 2) + 1, j * 2] = Random.Range(thinLower, thinHigher);
                                details[(i * 2) + 1, (j * 2) + 1] = Random.Range(thinLower, thinHigher);
                                break;

                            //All expect upper right
                            case 49:
                            case 63:
                            case 89:
                            case 157:
                                details[i * 2, j * 2] = Random.Range(thinLower, thinHigher);
                                details[i * 2, (j * 2) + 1] = Random.Range(thinLower, thinHigher);
                                details[(i * 2) + 1, j * 2] = Random.Range(thinLower, thinHigher);
                                break;

                            //All expect upper left
                            case 50:
                            case 60:
                            case 90:
                            case 158:
                                details[i * 2, j * 2] = Random.Range(thinLower, thinHigher);
                                details[i * 2, (j * 2) + 1] = Random.Range(thinLower, thinHigher);
                                details[(i * 2) + 1, (j * 2) + 1] = Random.Range(thinLower, thinHigher);
                                break;

                            //All expect lower left
                            case 51:
                            case 61:
                            case 91:
                            case 159:
                                details[i * 2, (j * 2) + 1] = Random.Range(thinLower, thinHigher);
                                details[(i * 2) + 1, j * 2] = Random.Range(thinLower, thinHigher);
                                details[(i * 2) + 1, (j * 2) + 1] = Random.Range(thinLower, thinHigher);
                                break;

                            //Left to right
                            case 204:
                            case 206:
                            case 214:
                                details[i * 2, j * 2] = Random.Range(thinLower, thinHigher);
                                details[(i * 2) + 1, (j * 2) + 1] = Random.Range(thinLower, thinHigher);
                                break;

                            //Right to left
                            case 205:
                            case 207:
                            case 213:
                                details[(i * 2) + 1, j * 2] = Random.Range(thinLower, thinHigher);
                                details[i * 2, (j * 2) + 1] = Random.Range(thinLower, thinHigher);
                                break;

                        }

                    }
                }
                terrainData.SetDetailLayer(0, 0, 0, details);
            }

            //			stopwatch.Stop();
            //			// Write result
            //			UnityEngine.Debug.Log("Time elapsed: " +
            //			                      stopwatch.Elapsed);
        }



    }


}
