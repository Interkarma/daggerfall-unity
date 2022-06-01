// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: XJDHDR
// Contributors:     
// 
// Notes:
//

using DaggerfallWorkshop;
using DaggerfallWorkshop.Game;
using DaggerfallWorkshop.Utility;
using UnityEngine;

namespace DaggerfallWorkshop
{
    /// <summary>
    /// Attached to all Automap models to perform various functions specific to them.
    /// </summary>
    public sealed class AutomapModel : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        private bool subscribedToEvents = false;

        void Awake()
        {
            if (!subscribedToEvents)
            {
                Automap.OnInjectMeshAndMaterialProperties += Automap_OnInjectMeshAndMaterialProperties;
                subscribedToEvents = true;
            }
        }

        private void OnDestroy()
        {
            if (subscribedToEvents)
            {
                Automap.OnInjectMeshAndMaterialProperties -= Automap_OnInjectMeshAndMaterialProperties;
                subscribedToEvents = false;
            }
        }

        private void Automap_OnInjectMeshAndMaterialProperties(bool playerIsInsideBuilding, Vector3 playerAdvancedPos, Material automapMaterial, bool resetDiscoveryState = true)
        {
            Automap.OnInjectMeshAndMaterialProperties -= Automap_OnInjectMeshAndMaterialProperties;
            subscribedToEvents = false;

            // get rid of animated materials (will not break automap rendering but is not necessary)
            AnimatedMaterial[] animatedMaterials = gameObject.GetComponentsInChildren<AnimatedMaterial>();
            foreach (AnimatedMaterial animatedMaterial in animatedMaterials)
            {
                UnityEngine.Object.Destroy(animatedMaterial);
            }

            MeshRenderer[] meshRenderers = gameObject.GetComponentsInChildren<MeshRenderer>();
            if (meshRenderers == null)
                return;

            // Update materials. If inside an interior, set visitedInThisEntering parameter to True so that they are always coloured.
            // Otherwise, mark meshes as not visited in this run (so "Dungeon" geometry that has been discovered in a previous dungeon run is rendered in grayscale)
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                UpdateMaterialsOfMeshRenderer(playerAdvancedPos, automapMaterial, meshRenderer, playerIsInsideBuilding);

                // if player is inside dungeon or castle and forced reset of discovery state.
                if ((!playerIsInsideBuilding) && (resetDiscoveryState))
                {
                    // mark meshRenderer as undiscovered
                    meshRenderer.enabled = false;
                }
            }
        }

        private void UpdateMaterialsOfMeshRenderer(Vector3 playerAdvancedPos, Material automapMaterial, MeshRenderer meshRenderer, bool visitedInThisEntering = false)
        {
            Material[] newMaterials = new Material[meshRenderer.materials.Length];
            for (int i = 0; i < meshRenderer.materials.Length; i++)
            {
                Material curMaterial = meshRenderer.materials[i];
                Material newMaterial = Instantiate(automapMaterial);

                //newMaterial.CopyPropertiesFromMaterial(material);
                newMaterial.name = "AutomapBelowSclicePlane injected for: " + curMaterial.name;
                if (curMaterial.HasProperty(Uniforms.MainTex))
                    newMaterial.SetTexture(Uniforms.MainTex, curMaterial.GetTexture(Uniforms.MainTex));
                if (curMaterial.HasProperty(Uniforms.BumpMap))
                    newMaterial.SetTexture(Uniforms.BumpMap, curMaterial.GetTexture(Uniforms.BumpMap));
                if (curMaterial.HasProperty(Uniforms.EmissionMap))
                    newMaterial.SetTexture(Uniforms.EmissionMap, curMaterial.GetTexture(Uniforms.EmissionMap));
                if (curMaterial.HasProperty(Uniforms.EmissionColor))
                    newMaterial.SetColor(Uniforms.EmissionColor, curMaterial.GetColor(Uniforms.EmissionColor));
                Vector4 playerPosition = new Vector4(playerAdvancedPos.x, playerAdvancedPos.y + Camera.main.transform.localPosition.y, playerAdvancedPos.z, 0.0f);
                newMaterial.SetVector("_PlayerPosition", playerPosition);
                if (visitedInThisEntering == true)
                    newMaterial.DisableKeyword("RENDER_IN_GRAYSCALE");
                else
                    newMaterial.EnableKeyword("RENDER_IN_GRAYSCALE");
                newMaterials[i] = newMaterial;
            }
            meshRenderer.materials = newMaterials;
        }
    }
}
