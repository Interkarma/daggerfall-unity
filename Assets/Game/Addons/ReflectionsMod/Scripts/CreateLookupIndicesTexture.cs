using System;
using UnityEngine;
using UnityEngine.Rendering;

using ReflectionsMod;

using DaggerfallWorkshop.Game;

namespace ReflectionsMod
{
    public class CreateLookupIndicesTexture : MonoBehaviour
    {
        [SerializeField]
        private Shader m_ShaderCreateLookupIndices;
        public Shader shaderCreateLookupIndices
        {
            get
            {
                if (m_ShaderCreateLookupIndices == null)
                    m_ShaderCreateLookupIndices = Shader.Find("Daggerfall/CreateLookupIndices");

                return m_ShaderCreateLookupIndices;
            }
        }

        private Material m_MaterialCreateLookupIndices;
        public Material materialCreateLookupIndices
        {
            get
            {
                if (m_MaterialCreateLookupIndices == null)
                    m_MaterialCreateLookupIndices = new Material(shaderCreateLookupIndices);

                return m_MaterialCreateLookupIndices;
            }
        }

        private Shader m_ShaderCreateLookupIndexReflectionTexture;
        public Shader shaderCreateLookupIndexReflectionTexture
        {
            get
            {
                if (m_ShaderCreateLookupIndexReflectionTexture == null)
                    m_ShaderCreateLookupIndexReflectionTexture = Shader.Find("Daggerfall/CreateLookupIndexReflectionTexture");

                return m_ShaderCreateLookupIndexReflectionTexture;
            }
        }

        private Material m_MaterialCreateLookupIndexReflectionTexture;
        public Material materialCreateLookupIndexReflectionTexture
        {
            get
            {
                if (m_MaterialCreateLookupIndexReflectionTexture == null)
                    m_MaterialCreateLookupIndexReflectionTexture = new Material(shaderCreateLookupIndexReflectionTexture);

                return m_MaterialCreateLookupIndexReflectionTexture;
            }
        }

        private Camera m_Camera;
        public Camera camera_
        {
            get
            {
                return m_Camera;
            }
        }

        public RenderTexture m_RenderTextureLookupIndices;
        public RenderTexture renderTextureLookupIndices
        {
            get
            {
                if (m_RenderTextureLookupIndices == null)
                    m_RenderTextureLookupIndices = new RenderTexture(camera_.pixelWidth, camera_.pixelHeight, 16, RenderTextureFormat.RGFloat); // 2-channel 16-bit floating-point per channel texture

                return m_RenderTextureLookupIndices;
            }
        }

        public RenderTexture m_RenderTextureIndexReflectionsTexture;
        public RenderTexture renderTextureIndexReflectionsTexture
        {
            get
            {
                if (m_RenderTextureIndexReflectionsTexture == null)
                    m_RenderTextureIndexReflectionsTexture = new RenderTexture(camera_.pixelWidth, camera_.pixelHeight, 16, RenderTextureFormat.R8); // 1-channel 8-bit fixed point texture

                return m_RenderTextureIndexReflectionsTexture;
            }
        }

        private UpdateReflectionTextures instanceUpdateReflectionTextures = null;

        private void Awake()
        {
            if (m_Camera == null)
            {
                m_Camera = this.gameObject.AddComponent<Camera>();
                m_Camera.CopyFrom(Camera.main);
                m_Camera.renderingPath = RenderingPath.Forward; // important to make fragment shader work with function Camera.RenderWithShader() - it won't work in deferred, this is no problem since we only want to render a custom index rendertexture anyway                
                m_Camera.enabled = false; // important to disable camera so we can invoke Camera.RenderWithShader() manually later
            }
        }

        void Start()
        {
            instanceUpdateReflectionTextures = GameObject.Find("ReflectionsMod").GetComponent<UpdateReflectionTextures>();
        }

        private void OnEnable()
        {
        }

        void OnDisable()
        {
            if (m_MaterialCreateLookupIndices)
                DestroyImmediate(m_MaterialCreateLookupIndices);

            m_MaterialCreateLookupIndices = null;

            if (m_MaterialCreateLookupIndexReflectionTexture)
                DestroyImmediate(m_MaterialCreateLookupIndexReflectionTexture);

            m_MaterialCreateLookupIndexReflectionTexture = null;
        }

        public void createLookupIndicesTexture()
        {
            m_Camera.transform.position = Camera.main.transform.position;
            m_Camera.transform.rotation = Camera.main.transform.rotation;

            /*
            Renderer[] renderers = null;            

            if (GameManager.Instance.IsPlayerInside != null)
            {
                if (GameManager.Instance.IsPlayerInsideBuilding)
                {
                    renderers = GameManager.Instance.InteriorParent.GetComponentsInChildren<Renderer>();
                }
                else
                {
                    renderers = GameManager.Instance.DungeonParent.GetComponentsInChildren<Renderer>();
                }
            }

            if (renderers != null)
            {
                foreach (Renderer r in renderers)
                {
                    Material[] mats = r.sharedMaterials;
                    foreach (Material m in mats)
                    {
                        m.SetFloat("_GroundLevelHeight", instanceUpdateReflectionTextures.ReflectionPlaneGroundLevelY);
                        m.SetFloat("_LowerLevelHeight", instanceUpdateReflectionTextures.ReflectionPlaneLowerLevelY);
                    }
                    r.sharedMaterials = mats;
                }
            }
            */

            /*
            if (GameManager.Instance.IsPlayerInside)
            {
                if (GameManager.Instance.IsPlayerInsideBuilding)
                {
                    Transform transform = GameManager.Instance.InteriorParent.transform.GetChild(2).transform.Find("People Flats");
                    transform.gameObject.SetActive(false);
                         // Interior Flats
                }
            }
            */

            Shader.SetGlobalFloat("_GroundLevelHeight", instanceUpdateReflectionTextures.ReflectionPlaneGroundLevelY);
            Shader.SetGlobalFloat("_LowerLevelHeight", instanceUpdateReflectionTextures.ReflectionPlaneLowerLevelY);
            m_Camera.targetTexture = renderTextureLookupIndices;
            m_Camera.RenderWithShader(shaderCreateLookupIndices, ""); // apply custom fragment shader and write into renderTextureLookupIndices
            m_Camera.targetTexture = renderTextureIndexReflectionsTexture;
            m_Camera.RenderWithShader(shaderCreateLookupIndexReflectionTexture, ""); // apply custom fragment shader and write into renderTextureIndexReflectionsTexture
            /*
            if (GameManager.Instance.IsPlayerInside)
            {
                if (GameManager.Instance.IsPlayerInsideBuilding)
                {
                    Transform transform = GameManager.Instance.InteriorParent.transform.GetChild(2).transform.Find("People Flats");
                    transform.gameObject.SetActive(true);
                    // Interior Flats
                }
            }
            */
        }
    }
}