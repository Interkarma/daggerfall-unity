using UnityEngine;
using UnityEditor;
using System.Collections;
using DaggerfallWorkshop.Game;

[CustomEditor(typeof(DaggerfallFLCPlayer))]
public class DaggerfallFLCPlayerEditor : Editor {

    string path = null;
    string playText = null;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DaggerfallFLCPlayer flcPlayer = (DaggerfallFLCPlayer)target;
      
        if (GUILayout.Button("Select File"))
        {
            path = EditorUtility.OpenFilePanel("FLC file", DaggerfallWorkshop.DaggerfallUnity.Instance.Arena2Path, "FLC");
            flcPlayer.Open(path);
            flcPlayer.Play();
        }
        if (flcPlayer.IsPlaying)
        {
            playText = "Stop";
            if (GUILayout.Button(playText))
            {
                flcPlayer.IsPlaying = false;

            }

        }
        else
        {
            playText = "Play";
            if (GUILayout.Button(playText))
            {
                flcPlayer.PlayHelper();

            }

        }


        flcPlayer.speedMod = (EditorGUILayout.Slider("Playback Speed", flcPlayer.speedMod, .01f, 2.0f));


    }

}
