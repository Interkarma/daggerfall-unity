using UnityEngine;

/// <summary>
/// Simple FPS counter using OnGui(), taken from here:
/// http://wiki.unity3d.com/index.php/FramesPerSecond
/// </summary>

public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;
    public bool ShowDebugString = true;

    public static FPSDisplay fpsDisplay;

    void Awake()
    {
        if (fpsDisplay == null)
            fpsDisplay = this;
        else
            Destroy(this);

    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        if (!ShowDebugString)
            return;

        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(w / 2, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.white;  //new Color(0.0f, 0.0f, 0.5f, 1.0f);
        float msec = deltaTime * 1000.0f;
        float fps = 1.0f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }
}
