//ReflectionsMod for Daggerfall Tools For Unity
//http://www.reddit.com/r/dftfu
//http://www.dfworkshop.net/
//Author: Michael Rauter (a.k.a. Nystul)
//License: MIT License (http://www.opensource.org/licenses/mit-license.php)

// This script is derived from MirrorReflection4 script

using UnityEngine;
using System.Collections;
using ProjectIncreasedTerrainDistance;
 
namespace ReflectionsMod
{
    [ExecuteInEditMode] // Make mirror live-update even when not in play mode
    public class MirrorReflection : MonoBehaviour
    {
	    public bool m_DisablePixelLights = false;
	    public int m_TextureSize = 256;
        public float m_ClipPlaneOffset = 0.05f; //0.0f; //0.07f;
 
	    public LayerMask m_ReflectLayers = -1;
 
	    private Hashtable m_ReflectionCameras = new Hashtable(); // Camera -> Camera table
 
	    public RenderTexture m_ReflectionTexture = null;
	    private int m_OldReflectionTextureSize = 0;
 
	    private static bool s_InsideRendering = false;

        private Camera cameraToUse = null;

        public enum BackgroundSettings {SkyboxAndGlobalFog, SolidColorBlack};
        private BackgroundSettings currentBackgroundSettings = BackgroundSettings.SolidColorBlack;
        public BackgroundSettings CurrentBackgroundSettings
        {
            get
            {
                return currentBackgroundSettings;
            }
            set { currentBackgroundSettings = value; }
        }

        void Start()
        {
            GameObject stackedCameraGameObject = GameObject.Find("stackedCamera");
            if (stackedCameraGameObject != null)
            {
                cameraToUse = stackedCameraGameObject.GetComponent<Camera>(); // when stacked camera is present use it to prevent reflection of near terrain in stackedCamera clip range distance not being updated
            }
            if (!cameraToUse)  // if stacked camera was not found us main camera
            {
                cameraToUse = Camera.main;
            }
        }

	    // This is called when it's known that the object will be rendered by some
	    // camera. We render reflections and do other updates here.
	    // Because the script executes in edit mode, reflections for the scene view
	    // camera will just work!
	    public void OnWillRenderObject()
        //void Update()
	    {
		    var rend = GetComponent<Renderer>();
		    if (!enabled || !rend || !rend.sharedMaterial) // || !rend.enabled)
			    return;
 
		    Camera cam = Camera.current;
		    if( !cam )
			    return;
            
            if (cam != cameraToUse) // skip every camera that is not the intended camera to use for rendering the mirrored scene
                return;

            // Safeguard from recursive reflections.        
		    if( s_InsideRendering )
			    return;
		    s_InsideRendering = true;
 
		    Camera reflectionCamera;
		    CreateMirrorObjects( cam, out reflectionCamera );
 
		    // find out the reflection plane: position and normal in world space
		    Vector3 pos = transform.position;
		    Vector3 normal = transform.up;
 
		    // Optionally disable pixel lights for reflection
		    int oldPixelLightCount = QualitySettings.pixelLightCount;
		    if( m_DisablePixelLights )
			    QualitySettings.pixelLightCount = 0;
 
		    UpdateCameraModes( cam, reflectionCamera );
 
		    // Render reflection
		    // Reflect camera around reflection plane
		    float d = -Vector3.Dot (normal, pos) - m_ClipPlaneOffset;
		    Vector4 reflectionPlane = new Vector4 (normal.x, normal.y, normal.z, d);
 
		    Matrix4x4 reflection = Matrix4x4.zero;
		    CalculateReflectionMatrix (ref reflection, reflectionPlane);
		    Vector3 oldpos = cam.transform.position;
		    Vector3 newpos = reflection.MultiplyPoint( oldpos );
		    reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;
 
		    // Setup oblique projection matrix so that near plane is our reflection
		    // plane. This way we clip everything below/above it for free.
		    Vector4 clipPlane = CameraSpacePlane( reflectionCamera, pos, normal, 1.0f );
		    //Matrix4x4 projection = cam.projectionMatrix;
		    Matrix4x4 projection = cam.CalculateObliqueMatrix(clipPlane);
		    reflectionCamera.projectionMatrix = projection;

            reflectionCamera.cullingMask = ~(1 << 4) & m_ReflectLayers.value; // never render water layer
		    reflectionCamera.targetTexture = m_ReflectionTexture;

            // next 2 lines are important for making shadows work correctly - otherwise shadows will be broken
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            rend.receiveShadows = false;

            GL.invertCulling = true;
		    reflectionCamera.transform.position = newpos;
		    Vector3 euler = cam.transform.eulerAngles;
		    reflectionCamera.transform.eulerAngles = new Vector3(0, euler.y, euler.z);
		    reflectionCamera.Render();
		    reflectionCamera.transform.position = oldpos;
            GL.invertCulling = false;

		    // Restore pixel light count
		    if( m_DisablePixelLights )
			    QualitySettings.pixelLightCount = oldPixelLightCount;
 
		    s_InsideRendering = false;
	    }
 
 
	    // Cleanup all the objects we possibly have created
	    void OnDisable()
	    {
		    if( m_ReflectionTexture ) {
			    Destroy( m_ReflectionTexture );
			    m_ReflectionTexture = null;
		    }
		    //foreach( DictionaryEntry kvp in m_ReflectionCameras )
			//    DestroyImmediate( ((Camera)kvp.Value).gameObject );
		    m_ReflectionCameras.Clear();
	    }
 
 
	    private void UpdateCameraModes( Camera src, Camera dest )
	    {
		    if( dest == null )
			    return;
            // set camera to clear the same way as current camera            
            CameraClearFlags clearFlags = CameraClearFlags.Skybox;
            if (currentBackgroundSettings == BackgroundSettings.SolidColorBlack)
            {
                clearFlags = CameraClearFlags.Color;
                dest.backgroundColor = Color.black;
                dest.clearFlags = clearFlags;
            }
            else if (currentBackgroundSettings == BackgroundSettings.SkyboxAndGlobalFog)
            {
                clearFlags = CameraClearFlags.Skybox;
                dest.backgroundColor = src.backgroundColor;
                dest.clearFlags = clearFlags;
            }
            if ( clearFlags == CameraClearFlags.Skybox )
		    {
                Skybox sky = src.GetComponent(typeof(Skybox)) as Skybox;
			    Skybox mysky = dest.GetComponent(typeof(Skybox)) as Skybox;
			    if( !sky || !sky.material )
			    {
				    mysky.enabled = false;
			    }
			    else
			    {
				    mysky.enabled = true;
				    mysky.material = sky.material;
			    }
		    }
		    // update other values to match current camera.
		    // even if we are supplying custom camera&projection matrices,
		    // some of values are used elsewhere (e.g. skybox uses far plane)

            // get near clipping plane from main camera
            dest.renderingPath = src.renderingPath;

            dest.farClipPlane = src.farClipPlane;

            dest.nearClipPlane = 0.03f; //src.nearClipPlane;
		    dest.orthographic = src.orthographic;
		    dest.fieldOfView = src.fieldOfView;
		    dest.aspect = src.aspect;
		    dest.orthographicSize = src.orthographicSize;
	    }
 
	    // On-demand create any objects we need
	    private void CreateMirrorObjects( Camera currentCamera, out Camera reflectionCamera )
	    {
		    reflectionCamera = null;
 
		    // Reflection render texture
		    if( !m_ReflectionTexture || m_OldReflectionTextureSize != m_TextureSize )
		    {
			    if( m_ReflectionTexture )
				    Destroy( m_ReflectionTexture );
                m_ReflectionTexture = new RenderTexture(m_TextureSize, m_TextureSize, 16); //, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);

			    m_ReflectionTexture.name = "__MirrorReflection" + GetInstanceID();
			    m_ReflectionTexture.isPowerOfTwo = true;

                //m_ReflectionTexture.generateMips = true;
                m_ReflectionTexture.useMipMap = true;
                m_ReflectionTexture.wrapMode = TextureWrapMode.Clamp;

			    //m_ReflectionTexture.hideFlags = HideFlags.DontSave;
			    m_OldReflectionTextureSize = m_TextureSize;
		    }
 
		    // Camera for reflection
		    reflectionCamera = m_ReflectionCameras[currentCamera] as Camera;
		    if( !reflectionCamera ) // catch both not-in-dictionary and in-dictionary-but-deleted-GO
		    {
			    GameObject go = new GameObject( "Mirror Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(), typeof(Camera), typeof(Skybox) );
			    reflectionCamera = go.GetComponent<Camera>();
			    reflectionCamera.enabled = false;
                reflectionCamera.transform.position = currentCamera.transform.position;
                reflectionCamera.transform.rotation = currentCamera.transform.rotation;
			    reflectionCamera.gameObject.AddComponent<FlareLayer>();
			    //go.hideFlags = HideFlags.HideAndDontSave;
			    m_ReflectionCameras[currentCamera] = reflectionCamera;

                if (currentBackgroundSettings == BackgroundSettings.SkyboxAndGlobalFog)
                {
                    // attach global fog to camera - this is important to get the same reflections like on normal terrain when deferred rendering is used
                    if ((reflectionCamera.renderingPath == RenderingPath.DeferredShading) || (reflectionCamera.renderingPath == RenderingPath.UsePlayerSettings))
                    {
                        UnityStandardAssets.ImageEffects.GlobalFog scriptGlobalFog = go.AddComponent<UnityStandardAssets.ImageEffects.GlobalFog>();
                        UnityStandardAssets.ImageEffects.GlobalFog globalFogMainCamera = Camera.main.gameObject.GetComponent<UnityStandardAssets.ImageEffects.GlobalFog>();
                        scriptGlobalFog.distanceFog = globalFogMainCamera.distanceFog;
                        scriptGlobalFog.excludeFarPixels = globalFogMainCamera.excludeFarPixels; // false
                        scriptGlobalFog.useRadialDistance = globalFogMainCamera.useRadialDistance;
                        scriptGlobalFog.heightFog = globalFogMainCamera.heightFog;
                        scriptGlobalFog.height = globalFogMainCamera.height;
                        scriptGlobalFog.heightDensity = globalFogMainCamera.heightDensity;
                        scriptGlobalFog.startDistance = globalFogMainCamera.startDistance;
                    }
                }

                go.transform.SetParent(GameObject.Find("ReflectionsMod").transform);
		    }
	    }
 
	    // Extended sign: returns -1, 0 or 1 based on sign of a
	    private static float sgn(float a)
	    {
		    if (a > 0.0f) return 1.0f;
		    if (a < 0.0f) return -1.0f;
		    return 0.0f;
	    }
 
	    // Given position/normal of the plane, calculates plane in camera space.
	    private Vector4 CameraSpacePlane (Camera cam, Vector3 pos, Vector3 normal, float sideSign)
	    {
		    Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
		    Matrix4x4 m = cam.worldToCameraMatrix;
		    Vector3 cpos = m.MultiplyPoint( offsetPos );
		    Vector3 cnormal = m.MultiplyVector( normal ).normalized * sideSign;
		    return new Vector4( cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos,cnormal) );
	    }
 
	    // Calculates reflection matrix around the given plane
	    private static void CalculateReflectionMatrix (ref Matrix4x4 reflectionMat, Vector4 plane)
	    {
		    reflectionMat.m00 = (1F - 2F*plane[0]*plane[0]);
		    reflectionMat.m01 = (   - 2F*plane[0]*plane[1]);
		    reflectionMat.m02 = (   - 2F*plane[0]*plane[2]);
		    reflectionMat.m03 = (   - 2F*plane[3]*plane[0]);
 
		    reflectionMat.m10 = (   - 2F*plane[1]*plane[0]);
		    reflectionMat.m11 = (1F - 2F*plane[1]*plane[1]);
		    reflectionMat.m12 = (   - 2F*plane[1]*plane[2]);
		    reflectionMat.m13 = (   - 2F*plane[3]*plane[1]);
 
		    reflectionMat.m20 = (   - 2F*plane[2]*plane[0]);
		    reflectionMat.m21 = (   - 2F*plane[2]*plane[1]);
		    reflectionMat.m22 = (1F - 2F*plane[2]*plane[2]);
		    reflectionMat.m23 = (   - 2F*plane[3]*plane[2]);
 
		    reflectionMat.m30 = 0F;
		    reflectionMat.m31 = 0F;
		    reflectionMat.m32 = 0F;
		    reflectionMat.m33 = 1F;
	    }
    }
}