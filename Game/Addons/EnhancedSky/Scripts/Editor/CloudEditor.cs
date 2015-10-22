using UnityEngine;
using UnityEditor;
using System.Collections;

namespace EnhancedSky
{

    [CustomEditor(typeof(Cloud))]
    public class CloudEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Cloud cloud = (Cloud)target;
            DrawDefaultInspector();

            if (EditorApplication.isPlaying)
            {
                if (GUILayout.Button("Apply normal cloud texture"))
                    cloud.StartCoroutine("CreateTexture", false);
                if (GUILayout.Button("Apply overcast cloud texture"))
                    cloud.StartCoroutine("CreateTexture", true);

                if (GUILayout.Button("Generate Normal clouds"))
                    cloud.BuildQueue(false);

                if (GUILayout.Button("Generate Overcast clouds"))
                    cloud.BuildQueue(true);
                if (GUILayout.Button("Clear cloud Caches"))
                {
                    cloud.normalCloudBuffer = new System.Collections.Generic.Queue<Color[]>(cloud.normalQueueSize);
                    cloud.overcastCloudBuffer = new System.Collections.Generic.Queue<Color[]>(cloud.overCastQueueSize);
                }

            }



        }





    }
}
