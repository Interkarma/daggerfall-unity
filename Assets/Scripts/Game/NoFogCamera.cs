using UnityEngine;

public class NoFogCamera : MonoBehaviour
{

    bool doWeHaveFogInScene;

    private void OnPreRender()
    {
        doWeHaveFogInScene = RenderSettings.fog;
        RenderSettings.fog = false;
    }
    private void OnPostRender()
    {
        RenderSettings.fog = doWeHaveFogInScene;
    }
}