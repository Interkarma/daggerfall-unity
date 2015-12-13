using UnityEngine;
using UnityEditor;
using System.Collections;
namespace EnhancedSky
{

    [CustomEditor(typeof(SkyManager))]
    public class SkyManagerEditor : Editor
    {
        SkyObjectSize skyObjectSizeSelected = SkyObjectSize.Normal;
        
        public override void OnInspectorGUI()
        {
            SkyManager skyMan = (SkyManager)target;
            

            DrawDefaultInspector();

            if (EditorApplication.isPlaying)
            {
               


                if (GUILayout.Button("Toggle Enhanced Sky"))
                {
                    skyMan.EnhancedSkyCurrentToggle = !skyMan.EnhancedSkyCurrentToggle;
                    skyMan.ToggleEnhancedSky(skyMan.EnhancedSkyCurrentToggle);
                }
                if (GUILayout.Button("Toggle Sun Flare"))
                {
                    skyMan.UseSunFlare = !skyMan.UseSunFlare;
                }

                if (GUILayout.Button("Apply normal cloud texture"))
                {
                    Cloud cloud = GameObject.FindObjectOfType<Cloud>();
                    if(cloud != null)
                    {
                        cloud.GetNewTexture(false);
                    }

                }

                if (GUILayout.Button("Apply overcast cloud texture"))
                {
                    Cloud cloud = GameObject.FindObjectOfType<Cloud>();
                    if (cloud != null)
                    {
                        cloud.GetNewTexture(true);
                    }
                }

                if(GUILayout.Button("Toggle Weather"))          //WeatherManager doesn't like this before the game is properly started
                {
                    if (skyMan.IsOvercast)
                        skyMan.weatherMan.ClearAllWeather();
                    else
                        skyMan.weatherMan.SetRainOvercast(true);


                    Debug.Log("Is Overcast: " + skyMan.weatherMan.IsOvercast);
                }
                    

                skyObjectSizeSelected = (SkyObjectSize)EditorGUILayout.EnumPopup("Sky Object Size", skyObjectSizeSelected);

                if (GUILayout.Button("Update Setting"))
                {
                    skyMan.SkyObjectSizeChange(skyObjectSizeSelected);
                }
            }



        }


    }

}
