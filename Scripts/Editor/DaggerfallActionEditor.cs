using UnityEngine;
using UnityEditor;
using DaggerfallWorkshop;

[CustomEditor(typeof(DaggerfallAction))]
public class DaggerfallActionEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        DrawDefaultInspector();
        DaggerfallAction thisAction = (DaggerfallAction)target;

        if (GUILayout.Button("Activate") && EditorApplication.isPlaying)
        {
            if (thisAction.PreviousObject == null)
                thisAction.Play(GameObject.FindGameObjectWithTag("Player"));
            else
                thisAction.Play(thisAction.PreviousObject);
        }
    }
}
