// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2021 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: John Doom
// Contributors:    
// 
// Notes:
//

using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using System.Collections;
using UnityEngine;

public class MaterialReaderAtlas : MonoBehaviour
{
    public Rect[] rectsOut;
    public RecordIndex[] indicesOut;
    public Material material;

    public IEnumerator Load(
        MaterialReader materialRender,
            int archive,
            int alphaIndex,
            int padding,
            int maxAtlasSize,
            int border = 0,
            bool dilate = false,
            int shrinkUVs = 0,
            bool copyToOppositeBorder = false,
            bool isBillboard = false)
    {
        // Ready check
        if (!materialRender.IsReady)
            yield break;

        int key = MaterialReader.MakeTextureKey((short)archive, (byte)0, (byte)0, MaterialReader.AtlasKeyGroup);
        if (materialRender.materialDict.ContainsKey(key))
        {
            CachedMaterial cm = materialRender.GetMaterialFromCache(key);
            if (cm.filterMode == materialRender.MainFilterMode)
            {
                // Properties are the same
                rectsOut = cm.atlasRects;
                indicesOut = cm.atlasIndices;
                material = cm.material;
                yield break;
            }
            else
            {
                // Properties don't match, remove material and reload
                materialRender.materialDict.Remove(key);
            }
        }

        // Create material
        if (isBillboard)
            material = MaterialReader.CreateBillboardMaterial();
        else
            material = MaterialReader.CreateDefaultMaterial();

        // Create settings
        GetTextureSettings settings = TextureReader.CreateTextureSettings(archive, 0, 0, alphaIndex, border, dilate);
        settings.createNormalMap = materialRender.GenerateNormals;
        settings.autoEmission = true;
        settings.atlasShrinkUVs = shrinkUVs;
        settings.atlasPadding = padding;
        settings.atlasMaxSize = maxAtlasSize;
        settings.copyToOppositeBorder = copyToOppositeBorder;

        // Setup material
        material.name = string.Format("TEXTURE.{0:000} [Atlas]", archive);

        TextureReaderAtlas atlas = new GameObject().AddComponent<TextureReaderAtlas>();
        yield return atlas.Load(materialRender.TextureReader, settings, materialRender.AlphaTextureFormat);
        GetTextureResults results = atlas.results;
        GameObject.Destroy(atlas.gameObject);

        material.mainTexture = results.albedoMap;
        material.mainTexture.filterMode = materialRender.MainFilterMode;

        // Setup normal map
        if (materialRender.GenerateNormals && results.normalMap != null)
        {
            results.normalMap.filterMode = materialRender.MainFilterMode;
            material.SetTexture(Uniforms.BumpMap, results.normalMap);
            material.EnableKeyword(KeyWords.NormalMap);
        }

        // Setup emission map
        if (results.isEmissive && results.emissionMap != null)
        {
            results.emissionMap.filterMode = materialRender.MainFilterMode;
            material.SetTexture(Uniforms.EmissionMap, results.emissionMap);
            material.SetColor(Uniforms.EmissionColor, Color.white);
            material.EnableKeyword(KeyWords.Emission);
        }

        // TEMP: Bridging between legacy material out params and GetTextureResults for now
        Vector2[] sizesOut, scalesOut, offsetsOut;
        sizesOut = results.atlasSizes.ToArray();
        scalesOut = results.atlasScales.ToArray();
        offsetsOut = results.atlasOffsets.ToArray();
        rectsOut = results.atlasRects.ToArray();
        indicesOut = results.atlasIndices.ToArray();

        // Setup cached material
        CachedMaterial newcm = new CachedMaterial();
        newcm.key = key;
        newcm.keyGroup = MaterialReader.AtlasKeyGroup;
        newcm.atlasRects = rectsOut;
        newcm.atlasIndices = indicesOut;
        newcm.material = material;
        newcm.filterMode = materialRender.MainFilterMode;
        newcm.recordSizes = sizesOut;
        newcm.recordScales = scalesOut;
        newcm.recordOffsets = offsetsOut;
        newcm.atlasFrameCounts = results.atlasFrameCounts.ToArray();
        materialRender.materialDict.Add(key, newcm);
    }
}
