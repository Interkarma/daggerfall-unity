using System;
using UnityEngine;
using UnityEngine.Rendering;

using ReflectionsMod;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ReflectionsMod
{
    [ExecuteInEditMode]
#if UNITY_5_4_OR_NEWER
    [ImageEffectAllowedInSceneView]
#endif

   public static class ImageEffectHelper
    {
        public static bool IsSupported(Shader s, bool needDepth, bool needHdr, MonoBehaviour effect)
        {
#if UNITY_EDITOR
            // Don't check for shader compatibility while it's building as it would disable most effects
            // on build farms without good-enough gaming hardware.
            if (!BuildPipeline.isBuildingPlayer)
            {
#endif
                if (s == null || !s.isSupported)
                {
                    Debug.LogWarningFormat("Missing shader for image effect {0}", effect);
                    return false;
                }

                if (!SystemInfo.supportsImageEffects)
                {
                    Debug.LogWarningFormat("Image effects aren't supported on this device ({0})", effect);
                    return false;
                }

                if (needDepth && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
                {
                    Debug.LogWarningFormat("Depth textures aren't supported on this device ({0})", effect);
                    return false;
                }

                if (needHdr && !SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
                {
                    Debug.LogWarningFormat("Floating point textures aren't supported on this device ({0})", effect);
                    return false;
                }
#if UNITY_EDITOR
            }
#endif

            return true;
        }

        public static Material CheckShaderAndCreateMaterial(Shader s)
        {
            //if (s == null || !s.isSupported)
            //    return null;

            var material = new Material(s);
            material.hideFlags = HideFlags.DontSave;
            return material;
        }

        public static bool supportsDX11
        {
            get { return SystemInfo.graphicsShaderLevel >= 50 && SystemInfo.supportsComputeShaders; }
        }
    }

    [RequireComponent(typeof(Camera))]    
    public class DeferredPlanarReflections : MonoBehaviour
    {

        ///////////// Unexposed Variables //////////////////

        [SerializeField]
        private Shader m_Shader;
        public Shader shader
        {
            get
            {
                if (m_Shader == null)
                    m_Shader = Shader.Find("Daggerfall/ReflectionsMod/DeferredPlanarReflections");

                return m_Shader;
            }
        }

        private Material m_Material;
        public Material material
        {
            get
            {
                if (m_Material == null)
                    m_Material = ReflectionsMod.ImageEffectHelper.CheckShaderAndCreateMaterial(shader);

                return m_Material;
            }
        }

        private Camera m_Camera;
        public Camera camera_
        {
            get
            {
                if (m_Camera == null)
                    m_Camera = GetComponent<Camera>();

                return m_Camera;
            }
        }

        private CommandBuffer m_CommandBuffer;

        private static int kFinalReflectionTexture;
        private static int kTempTexture;


        private UpdateReflectionTextures componentUpdateReflectionTextures = null;

        public RenderTexture reflectionGroundTexture;
        public RenderTexture reflectionLowerLevelTexture;

        CreateReflectionLookupTextures componentCreateReflectionLookupTextures = null;
        GameObject goCreateReflectionLookupTextures = null;

        // Shader pass indices used by the effect
        private enum PassIndex
        {
            ReflectionStep = 0,
            CompositeFinal = 1,
        }

        private void OnEnable()
        {
            //if (!ReflectionsMod.ImageEffectHelper.IsSupported(shader, false, true, this))
            //{
            //   enabled = false;
            //    return;
            //}

            camera_.depthTextureMode |= DepthTextureMode.Depth;

            kFinalReflectionTexture = Shader.PropertyToID("_FinalReflectionTexture");
            kTempTexture = Shader.PropertyToID("_TempTexture");

            GameObject goReflectionsMod = GameObject.Find("ReflectionsMod");

            componentUpdateReflectionTextures = goReflectionsMod.GetComponent<UpdateReflectionTextures>();

            goCreateReflectionLookupTextures = new GameObject("CreateReflectionLookupTextures");
            goCreateReflectionLookupTextures.transform.SetParent(goReflectionsMod.transform);
            componentCreateReflectionLookupTextures = goCreateReflectionLookupTextures.AddComponent<CreateReflectionLookupTextures>();     
        }

        void OnDisable()
        {
            if (componentCreateReflectionLookupTextures != null)
            {
                Destroy(componentCreateReflectionLookupTextures);
            }
            if (goCreateReflectionLookupTextures != null)
            {
                Destroy(goCreateReflectionLookupTextures);
            }

            if (m_Material)
                DestroyImmediate(m_Material);

            m_Material = null;

            if (camera_ != null)
            {
                if (m_CommandBuffer != null)
                {
                    camera_.RemoveCommandBuffer(CameraEvent.AfterReflections, m_CommandBuffer);
                }

                m_CommandBuffer = null;
            }            
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (camera_ != null)
            {
                if (m_CommandBuffer != null)
                {
                    camera_.RemoveCommandBuffer(CameraEvent.AfterReflections, m_CommandBuffer);
                }

                m_CommandBuffer = null;
            }
        }
#endif

        public void OnPreRender()
        {
            componentCreateReflectionLookupTextures.createReflectionTextureCoordinatesAndIndexTextures();

            if (material == null)
            {
                return;
            }
            else if (Camera.current.actualRenderingPath != RenderingPath.DeferredShading)
            {
                return;
            }

            int downsampleAmount = 1; // 2;

            var rtW = camera_.pixelWidth / downsampleAmount;
            var rtH = camera_.pixelHeight / downsampleAmount;

            float sWidth = camera_.pixelWidth;
            float sHeight = camera_.pixelHeight;

            Matrix4x4 P = GetComponent<Camera>().projectionMatrix;
            Vector4 projInfo = new Vector4
                    ((-2.0f / (sWidth * P[0])),
                    (-2.0f / (sHeight * P[5])),
                    ((1.0f - P[2]) / P[0]),
                    ((1.0f + P[6]) / P[5]));

            RenderTextureFormat intermediateFormat = camera_.hdr ? RenderTextureFormat.ARGBHalf : RenderTextureFormat.ARGB32;
     
            Matrix4x4 cameraToWorldMatrix = GetComponent<Camera>().worldToCameraMatrix.inverse;
            material.SetVector("_ProjInfo", projInfo); // used for unprojection
            material.SetMatrix("_CameraToWorldMatrix", cameraToWorldMatrix);
            material.SetMatrix("_InverseViewProject", (GetComponent<Camera>().projectionMatrix * GetComponent<Camera>().worldToCameraMatrix).inverse);

            reflectionGroundTexture = componentUpdateReflectionTextures.getGroundReflectionRenderTexture();
            reflectionLowerLevelTexture = componentUpdateReflectionTextures.getSeaReflectionRenderTexture();
            material.SetTexture("_ReflectionGroundTex", reflectionGroundTexture);
            material.SetTexture("_ReflectionLowerLevelTex", reflectionLowerLevelTexture);

            material.SetTexture("_ReflectionsTextureIndexTex", componentCreateReflectionLookupTextures.renderTextureReflectionTextureIndex);
            material.SetTexture("_ReflectionsTextureCoordinatesTex", componentCreateReflectionLookupTextures.renderTextureReflectionTextureCoordinates);

            material.SetFloat("_GroundLevelHeight", componentUpdateReflectionTextures.ReflectionPlaneGroundLevelY);
            material.SetFloat("_LowerLevelHeight", componentUpdateReflectionTextures.ReflectionPlaneLowerLevelY);            

            if (m_CommandBuffer == null)
            {
                m_CommandBuffer = new CommandBuffer();
                m_CommandBuffer.name = "Deferred Planar Reflections";


                m_CommandBuffer.GetTemporaryRT(kFinalReflectionTexture, rtW, rtH, 0, FilterMode.Point, intermediateFormat);

                m_CommandBuffer.GetTemporaryRT(kTempTexture, camera_.pixelWidth, camera_.pixelHeight, 0, FilterMode.Bilinear, intermediateFormat);
                m_CommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, kFinalReflectionTexture, material, (int)PassIndex.ReflectionStep);

                m_CommandBuffer.Blit(BuiltinRenderTextureType.CameraTarget, kTempTexture, material, (int)PassIndex.CompositeFinal);
                m_CommandBuffer.Blit(kTempTexture, BuiltinRenderTextureType.CameraTarget);                

                m_CommandBuffer.ReleaseTemporaryRT(kTempTexture);
                camera_.AddCommandBuffer(CameraEvent.AfterFinalPass, m_CommandBuffer);
            }
        }
    }
}
