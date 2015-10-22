using UnityEngine;
using UnityEditor;
using System.Collections;
namespace EnhancedSky
{

    [CustomEditor(typeof(SkyManager))]
    public class SkyManagerEditor : Editor
    {
       

        public override void OnInspectorGUI()
        {
            SkyManager skyMan = (SkyManager)target;

            DrawDefaultInspector();

            if(GUILayout.Button("Toggle Enhanced Sky"))
            {
                skyMan.EnhancedSkyToggle = !skyMan.EnhancedSkyToggle;
                skyMan.ToggleSkyObjects(skyMan.EnhancedSkyToggle);
            }

           // if(GUILayout.Button("Toggle Sun Flare"))
            //{
               // skyMan.UseSunFlare = !skyMan.UseSunFlare;

                
          //  }



        }


    }

}
