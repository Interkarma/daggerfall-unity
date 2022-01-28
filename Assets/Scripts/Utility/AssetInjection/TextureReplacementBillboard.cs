using DaggerfallConnect;
using DaggerfallConnect.Arena2;
using DaggerfallWorkshop;
using DaggerfallWorkshop.Utility;
using DaggerfallWorkshop.Utility.AssetInjection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureReplacementBillboard : MonoBehaviour
{
    public Material material;
    MobileBillboardImportedTextures importedTextures;

    public IEnumerator Load(
        int archive,
        MeshFilter meshFilter,
        MobileBillboardImportedTextures importedTextures)
    {
        if (!DaggerfallUnity.Settings.AssetInjection)
            yield break;

        Texture2D tex, emission;
        if (importedTextures.HasImportedTextures = TextureReplacement.LoadFromCacheOrImport(archive, 0, 0, true, true, out tex, out emission))
        {
            string renderMode = null;

            // Read xml configuration
            XMLManager xml;
            if (XMLManager.TryReadXml(TextureReplacement.ImagesPath, string.Format("{0:000}", archive), out xml))
            {
                xml.TryGetString("renderMode", out renderMode);
                importedTextures.IsEmissive = xml.GetBool("emission");
            }

            // Make material
            material = TextureReplacement.MakeBillboardMaterial(renderMode);

            // Enable emission
            TextureReplacement.ToggleEmission(material, importedTextures.IsEmissive |= emission != null);

            // If the archive has a Arena2 texture file, use it to get record and frame count
            string fileName = TextureFile.IndexToFileName(archive);
            var textureFile = new TextureFile();
            if (textureFile.Load(Path.Combine(DaggerfallUnity.Instance.Arena2Path, fileName), FileUsage.UseMemory, true))
            {
                yield return null;

                // Import all textures in this archive
                importedTextures.Albedo = new Texture2D[textureFile.RecordCount][];
                importedTextures.EmissionMaps = importedTextures.IsEmissive ? new Texture2D[textureFile.RecordCount][] : null;
                for (int record = 0; record < textureFile.RecordCount; record++)
                {
                    int frames = textureFile.GetFrameCount(record);
                    var frameTextures = new Texture2D[frames];
                    var frameEmissionMaps = importedTextures.IsEmissive ? new Texture2D[frames] : null;

                    for (int frame = 0; frame < frames; frame++)
                    {
                        if (record != 0 || frame != 0)
                        {
                            TextureReplacement.LoadFromCacheOrImport(archive, record, frame, importedTextures.IsEmissive, true, out tex, out emission);
                            yield return null;
                        }

                        frameTextures[frame] = tex ?? ImageReader.GetTexture(fileName, record, frame, true);
                        if (frameEmissionMaps != null)
                            frameEmissionMaps[frame] = emission ?? frameTextures[frame];
                    }

                    importedTextures.Albedo[record] = frameTextures;
                    if (importedTextures.EmissionMaps != null)
                        importedTextures.EmissionMaps[record] = frameEmissionMaps;
                }
            }
            // Otherwise, check what files are available in the injected assets
            else
            {
                List<Texture2D[]> allAlbedo = new List<Texture2D[]>();
                List<Texture2D[]> allEmission = importedTextures.IsEmissive ? new List<Texture2D[]>() : null;

                int record = 0;
                while (TextureReplacement.TryImportTexture(archive, record, out Texture2D[] currentAlbedo))
                {
                    yield return null;
                    allAlbedo.Add(currentAlbedo);

                    if (importedTextures.IsEmissive)
                    {
                        if (TextureReplacement.TryImportTexture(TextureReplacement.texturesPath, frame => TextureReplacement.GetName(archive, record, frame, TextureMap.Emission), out Texture2D[] currentEmissive))
                        {
                            yield return null;
                            allEmission.Add(currentEmissive);
                        }
                        else
                        {
                            allEmission.Add(currentAlbedo);
                        }
                    }

                    ++record;
                }

                importedTextures.Albedo = allAlbedo.ToArray();
                importedTextures.EmissionMaps = importedTextures.IsEmissive ? allEmission.ToArray() : null;
            }

            // Update UV map
            TextureReplacement.SetUv(meshFilter);

            //return
            this.importedTextures = importedTextures;
        }
    }
}
