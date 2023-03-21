// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Podleron (podleron@gmail.com)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using DaggerfallWorkshop.Game.Utility.ModSupport;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using FullSerializer;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace DaggerfallWorkshop.Game.Addons.RmbBlockEditor
{
#if UNITY_EDITOR
    public static class RMBModManager
    {
        static Dictionary<string, ModInfo> DevModInfo;
        private static Dictionary<string, HashSet<string>> DevModModels;
        private static Dictionary<string, HashSet<string>> DevModTextures;

        static Dictionary<string, ModInfo> PackagedModInfo;
        static Dictionary<string, HashSet<string>> PackagedModelIds;
        static Dictionary<string, HashSet<string>> PackagedBillboardIds;

        private static Dictionary<string, string> PackagedModFilePaths;
        private static Dictionary<string, string> PackagedModModelPaths;
        private static Dictionary<string, string> PackagedModBillboardImagePaths;
        private static Dictionary<string, string> PackagedModBillboardXmlPaths;

        private static Dictionary<string, string> DevModModelPaths;
        private static Dictionary<string, string> DevModBillboardImagePaths;
        private static Dictionary<string, string> DevModBillboardXmlPaths;

        public static void LoadDevModInfos()
        {
            InstantiateDevModDictionaries();

            string modsfolder = Path.Combine(Application.dataPath, "Game", "Mods");
            foreach (var directory in Directory.EnumerateDirectories(modsfolder))
            {
                string foundFile = Directory.EnumerateFiles(directory)
                    .FirstOrDefault(file => file.EndsWith(".dfmod.json"));
                if (string.IsNullOrEmpty(foundFile))
                    continue;

                ModInfo modInfo = null;
                if (ModManager._serializer.TryDeserialize(fsJsonParser.Parse(File.ReadAllText(foundFile)), ref modInfo)
                    .Failed)
                    continue;

                var modelIds = new HashSet<string>();
                var textureIds = new HashSet<string>();

                foreach (var subFile in modInfo.Files)
                {
                    var isCustomModel = IsCustomModel(subFile);
                    var isCustomBillboard = IsCustomBillboardImage(subFile);
                    var isCustomXml = IsCustomBillboardXML(subFile);

                    var fileName = Path.GetFileNameWithoutExtension(subFile);
                    if (isCustomModel)
                    {
                        modelIds.Add(fileName);
                        if (!DevModModelPaths.ContainsKey(fileName))
                        {
                            DevModModelPaths.Add(fileName, subFile);
                        }
                    }
                    else if (isCustomBillboard)
                    {
                        var id = FileToBillboardId(fileName);
                        textureIds.Add(id);
                        if (!DevModBillboardImagePaths.ContainsKey(fileName))
                        {
                            DevModBillboardImagePaths.Add(fileName, subFile);
                        }
                    }
                    else if (isCustomXml)
                    {
                        if (!DevModBillboardXmlPaths.ContainsKey(fileName))
                        {
                            DevModBillboardXmlPaths.Add(fileName, subFile);
                        }
                    }
                }

                DevModInfo.Add(modInfo.ModTitle, modInfo);
                DevModModels.Add(modInfo.ModTitle, modelIds);
                DevModTextures.Add(modInfo.ModTitle, textureIds);
            }
        }

        public static void LoadPackagedMods()
        {
            InstantiatePackagedModsDictionaries();

            foreach (string file in Directory.EnumerateFiles(
                         Path.Combine(Application.dataPath, "StreamingAssets", "Mods"), "*.dfmod"))
            {
                AssetBundle bundle = null;
                try
                {
                    bundle = AssetBundle.LoadFromFile(file);
                    if (bundle == null)
                        continue;

                    string dfmodAssetName = bundle.GetAllAssetNames()
                        .FirstOrDefault(assetName => assetName.EndsWith(".dfmod.json"));
                    if (string.IsNullOrEmpty(dfmodAssetName))
                        continue;

                    TextAsset dfmodAsset = bundle.LoadAsset<TextAsset>(dfmodAssetName);
                    if (dfmodAsset == null)
                        continue;

                    ModInfo modInfo = null;
                    if (ModManager._serializer.TryDeserialize(fsJsonParser.Parse(dfmodAsset.text), ref modInfo).Failed)
                        continue;

                    PackagedModFilePaths.Add(modInfo.ModTitle, file);

                    var modelIds = new HashSet<string>();
                    var flatIds = new HashSet<string>();

                    foreach (var subFile in modInfo.Files)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(subFile);
                        var isCustomModel = IsCustomModel(subFile);
                        var isCustomBillboard = IsCustomBillboardImage(subFile);
                        var isCustomXml = IsCustomBillboardXML(subFile);

                        if (isCustomModel)
                        {
                            modelIds.Add(fileName);
                            if (!PackagedModModelPaths.ContainsKey(fileName))
                            {
                                PackagedModModelPaths.Add(fileName, subFile);
                            }
                        }
                        else if (isCustomBillboard)
                        {
                            var id = FileToBillboardId(fileName);
                            flatIds.Add(id);
                            if (!PackagedModBillboardImagePaths.ContainsKey(fileName))
                            {
                                PackagedModBillboardImagePaths.Add(fileName, subFile);
                            }
                        }
                        else if (isCustomXml)
                        {
                            if (!PackagedModBillboardXmlPaths.ContainsKey(fileName))
                            {
                                PackagedModBillboardXmlPaths.Add(fileName, subFile);
                            }
                        }
                    }

                    PackagedModInfo.Add(modInfo.ModTitle, modInfo);
                    PackagedModelIds.Add(modInfo.ModTitle, modelIds);
                    PackagedBillboardIds.Add(modInfo.ModTitle, flatIds);
                }
                catch (Exception err)
                {
                    Debug.Log(err);
                }
                finally
                {
                    bundle?.Unload(false);
                }
            }
        }

        public static List<CatalogItem> GetCustomCatalogModels()
        {
            LoadPackagedMods();
            LoadDevModInfos();
            var devMods = DevModInfo.Keys.ToList();
            var packagedMods = PackagedModInfo.Keys.ToList();
            var allIds = new List<CatalogItem>();

            foreach (var mod in packagedMods)
            {
                var models = PackagedModelIds[mod];
                foreach (var id in models)
                {
                    allIds.Add(new CatalogItem(id, id, "Mods", mod));
                }
            }

            foreach (var mod in devMods)
            {
                var models = DevModModels[mod];
                foreach (var id in models)
                {
                    allIds.Add(new CatalogItem(id, id, "Mods", mod));
                }
            }

            return allIds;
        }

        public static List<CatalogItem> GetCustomCatalogFlats()
        {
            LoadPackagedMods();
            LoadDevModInfos();
            var devMods = DevModModels.Keys.ToList();
            var packagedMods = PackagedModInfo.Keys.ToList();
            var allIds = new List<CatalogItem>();

            foreach (var mod in packagedMods)
            {
                var flats = PackagedBillboardIds[mod];
                foreach (var id in flats)
                {
                    allIds.Add(new CatalogItem(id, id, "Mods", mod));
                }
            }

            foreach (var mod in devMods)
            {
                var flats = DevModTextures[mod];
                foreach (var id in flats)
                {
                    allIds.Add(new CatalogItem(id, id, "Mods", mod));
                }
            }

            return allIds;
        }

        public static GameObject GetCustomBillboard(string id)
        {
            var go = GetDevModBillboard(id);
            return go == null ? GetPackagedModBillboard(id) : go;
        }

        public static GameObject GetCustomModel(string id)
        {
            var go = GetDevModModel(id);
            return go == null ? GetPackagedModModel(id) : go;
        }

        private static GameObject GetDevModModel(string id)
        {
            if (!DevModModelPaths.ContainsKey(id)) return null;
            var subFile = DevModModelPaths[id];
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(subFile);
            var go = Object.Instantiate(asset);
            var goTransform = go.GetComponent<Transform>();
            goTransform.position = Vector3.zero;
            goTransform.localPosition = Vector3.zero;
            go.name = $"Custom Daggerfall Mesh [ID={id}]";
            var runtimeMaterial = go.GetComponent<RuntimeMaterials>();
            var renderer = go.GetComponent<Renderer>();
            if (runtimeMaterial == null) return go;

            // RuntimeMaterials do not show up in the editor so we need to apply some normal materials to the renderer
            try
            {
                // Use reflection to read the private variable 'Materials' from the RuntimeMaterials Component
                var materials =
                    typeof(RuntimeMaterials).GetField("Materials", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(runtimeMaterial) as RuntimeMaterial[];

                // Iterate through the runtime materials and create normal materials from them
                var materialsToApply = new Material [materials.Length];
                for (var index = 0; index < materials.Length; index++)
                {
                    var material = materials[index];
                    var resolvedMaterial = GetMaterialFromRuntimeMaterial(material);
                    materialsToApply[index] = resolvedMaterial;
                }

                // apply the materials to the renderer
                renderer.materials = materialsToApply;
            }
            catch (Exception err)
            {
                Debug.Log(err);
            }

            return go;
        }

        private static GameObject GetPackagedModModel(string id)
        {
            // Find the mod that this model belongs to
            string modName = null;
            foreach (var key in PackagedModelIds.Keys.Where(key => PackagedModelIds[key].Contains(id)))
            {
                modName = key;
            }

            if (modName == null)
            {
                return null;
            }

            // Get the file path of the mod
            var fileName = PackagedModFilePaths[modName];

            AssetBundle bundle = null;
            try
            {
                // Load the mod bundle
                bundle = AssetBundle.LoadFromFile(fileName);

                // Find and load the asset
                var subFile = PackagedModModelPaths[id];
                var asset = bundle.LoadAsset<GameObject>(subFile);

                // Create a game object from the asset
                var go = Object.Instantiate(asset);
                go.name = $"Custom DaggerfallMesh [ID={id}]";

                // Unload the bundle and return the game object
                bundle.Unload(false);
                return go;
            }
            catch (Exception err)
            {
                Debug.Log(err);
                if (bundle != null)
                {
                    bundle.Unload(false);
                }
                return null;
            }
        }

        private static GameObject GetPackagedModBillboard(string id)
        {
            var parts = id.Split('.');
            var billboardName = TextureReplacement.GetName(int.Parse(parts[0]), int.Parse(parts[1]));

            // Find the mod that this billboard belongs to
            string modName = null;
            foreach (var key in PackagedBillboardIds.Keys.Where(key => PackagedBillboardIds[key].Contains(id)))
            {
                modName = key;
            }

            if (modName == null)
            {
                return null;
            }

            // Get the file path of the mod
            var fileName = PackagedModFilePaths[modName];

            AssetBundle bundle = null;
            try
            {
                // Load the mod bundle
                bundle = AssetBundle.LoadFromFile(fileName);

                // Find and load the image
                var textureSubFile = PackagedModBillboardImagePaths[billboardName];
                var texture = bundle.LoadAsset<Texture2D>(textureSubFile);

                // Find and load xml
                TextAsset xml = null;
                if (PackagedModBillboardXmlPaths.ContainsKey(billboardName))
                {
                    var xmlSubFile = PackagedModBillboardXmlPaths[billboardName];
                    xml = bundle.LoadAsset<TextAsset>(xmlSubFile);
                }

                // Set the scale of the texture
                var scale = Vector2.one;
                if (xml != null)
                {
                    scale = GetScale(xml);
                }
                var x = texture.width * scale.x * MeshReader.GlobalScale;
                var y = texture.height * scale.y * MeshReader.GlobalScale;
                var newScale = new Vector2(x, y);


                // Create the game object and add the DaggerfallBillboard component
                var go = new GameObject($"Custom Billboard {id}");
                var billboard = go.AddComponent<DaggerfallBillboard>();
                billboard.SetMaterial(texture, newScale);

                // Unload the bundle and return the game object
                bundle.Unload(false);
                return go;
            }
            catch (Exception err)
            {
                Debug.Log(err);
                if (bundle != null)
                {
                    bundle.Unload(false);
                }
                return null;
            }
        }

        private static GameObject GetDevModBillboard(string id)
        {
            // Get the file id
            var parts = id.Split('.');
            var fileId = TextureReplacement.GetName(int.Parse(parts[0]), int.Parse(parts[1]));

            // If the file id is not found, return null
            if (!DevModBillboardImagePaths.ContainsKey(fileId)) return null;

            // Load the texture from the AssetDatabase
            var imageSubFile = DevModBillboardImagePaths[fileId];
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(imageSubFile);

            // Check if there is an XML file for the same id
            var scale = Vector2.one;
            if (DevModBillboardXmlPaths.ContainsKey(fileId))
            {
                var xmlSubFile = DevModBillboardXmlPaths[fileId];
                var xml = AssetDatabase.LoadAssetAtPath<TextAsset>(xmlSubFile);
                scale = GetScale(xml);
            }

            // Create the game object
            var go = new GameObject($"Custom Billboard {id}");
            var x = texture.width * scale.x * MeshReader.GlobalScale;
            var y = texture.height * scale.y * MeshReader.GlobalScale;

            var newScale = new Vector2(x, y);
            var billboard = go.AddComponent<DaggerfallBillboard>();
            billboard.SetMaterial(texture, newScale);

            return go;
        }

        private static void InstantiatePackagedModsDictionaries()
        {
            PackagedModFilePaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            PackagedModInfo = new Dictionary<string, ModInfo>(StringComparer.OrdinalIgnoreCase);
            PackagedModelIds = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            PackagedBillboardIds = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

            PackagedModModelPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            PackagedModBillboardImagePaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            PackagedModBillboardXmlPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        private static void InstantiateDevModDictionaries()
        {
            DevModInfo = new Dictionary<string, ModInfo>(StringComparer.OrdinalIgnoreCase);
            DevModModels = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            DevModTextures = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

            DevModModelPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            DevModBillboardImagePaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            DevModBillboardXmlPaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        private static Material GetMaterialFromRuntimeMaterial(RuntimeMaterial runtimeMaterial)
        {
            int archive = runtimeMaterial.Archive;
            int record = runtimeMaterial.Record;

            var climate = PersistedSettings.ClimateBases();
            var season = PersistedSettings.ClimateSeason();


            if (runtimeMaterial.ApplyClimate)
                archive = ClimateSwaps.ApplyClimate(archive, record, climate, season);

            return DaggerfallUnity.Instance.MaterialReader.GetMaterial(archive, record);
        }

        private static Vector2 GetScale(TextAsset xml)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml.text);

            var infoElement = xmlDoc.SelectSingleNode("info");
            if (infoElement == null)
            {
                return Vector2.one;
            }

            // Get the "scaleX" element if it exists
            var scaleX = 1f;
            var scaleXElement = infoElement.SelectSingleNode("scaleX");
            if (scaleXElement != null)
            {
                float.TryParse(scaleXElement.InnerText, NumberStyles.Float, CultureInfo.InvariantCulture, out scaleX);
            }

            // Get the "scaleY" element if it exists
            var scaleY = 1f;
            var scaleYElement = infoElement.SelectSingleNode("scaleY");
            if (scaleYElement != null)
            {
                float.TryParse(scaleYElement.InnerText, NumberStyles.Float, CultureInfo.InvariantCulture, out scaleY);
            }

            return new Vector2(scaleX, scaleY);
        }

        private static bool IsCustomModel(string filePath)
        {
            return filePath.EndsWith(".prefab") && int.TryParse(Path.GetFileNameWithoutExtension(filePath), out int _);
        }

        private static bool IsCustomBillboardImage(string filePath)
        {
            Regex r = new Regex(@"/\d+_\d+-\d+\.(jpg|png)");
            return r.IsMatch(filePath);
        }

        private static bool IsCustomBillboardXML(string filePath)
        {
            Regex r = new Regex(@"/\d+_\d+-\d+\.(xml)");
            return r.IsMatch(filePath);
        }

        private static string FileToBillboardId(string id)
        {
            var parts = id.Split('-');
            return parts[0].Replace('_', '.');
        }
    }
#endif
}