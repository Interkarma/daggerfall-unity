using System;
using UnityEngine;
using UnityEngine.Rendering;

using ReflectionsMod;

namespace ReflectionsMod
{
    public class CreateLookupIndicesTexture : MonoBehaviour
    {
        [SerializeField]
        private Shader m_Shader;
        public Shader shader
        {
            get
            {
                if (m_Shader == null)
                    m_Shader = Shader.Find("Daggerfall/CreateLookupIndices");

                return m_Shader;
            }
        }

        private Material m_Material;
        public Material material
        {
            get
            {
                if (m_Material == null)
                    m_Material = new Material(shader);

                return m_Material;
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

        private RenderTexture m_RenderTextureLookupIndices;
        public RenderTexture renderTextureLookupIndices
        {
            get
            {
                if (m_RenderTextureLookupIndices == null)
                    m_RenderTextureLookupIndices = new RenderTexture(camera_.pixelWidth, camera_.pixelHeight, 16, RenderTextureFormat.ARGB32);

                return m_RenderTextureLookupIndices;
            }
        }

        private void Awake()
        {
            if (m_Camera == null)
            {
                m_Camera = this.gameObject.AddComponent<Camera>();
                m_Camera.CopyFrom(Camera.main);
                m_Camera.renderingPath = RenderingPath.Forward; // important to make fragment shader work with function Camera.RenderWithShader() - it won't work in deferred, this is no problem since we only want to render a custom index rendertexture anyway
                m_Camera.targetTexture = renderTextureLookupIndices;
                m_Camera.enabled = false; // important to disable camera so we can invoke Camera.RenderWithShader() manually later
            }
        }

        void Start()
        {
        }

        private void OnEnable()
        {
        }

        void OnDisable()
        {
            if (m_Material)
                DestroyImmediate(m_Material);

            m_Material = null;
        }

        public void createLookupIndicesTexture()
        {
            m_Camera.transform.position = Camera.main.transform.position;
            m_Camera.transform.rotation = Camera.main.transform.rotation;
            m_Camera.RenderWithShader(shader, ""); // apply custom fragment shader and write into renderTextureLookupIndices
        }
    }
}