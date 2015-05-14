//
// SL_NGUISpriteAtlasInspector.cs
// 
// Written by Niklas Borglund and Jakob Hillerström
//

//#define SMARTLOC_NGUI //<--- UNCOMMENT THIS FOR NGUI CLASSES

#if SMARTLOC_NGUI
namespace SmartLocalization.Editor
{
	using UnityEngine;
	using UnityEditor;

[CustomEditor(typeof(SL_NGUISpriteAtlas))]
public class SL_NGUISpriteAtlasInspector : Editor 
{
	private string selectedKey = null;
	
	void Awake()
	{
		SL_NGUISpriteAtlas textObject = ((SL_NGUISpriteAtlas)target);
		if(textObject != null)
		{
			selectedKey = textObject.localizedKey;
		}
	}
	
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		
		selectedKey = LocalizedKeySelector.SelectKeyGUI(selectedKey, true, LocalizedObjectType.GAME_OBJECT);
		
		if(!Application.isPlaying && GUILayout.Button("Use Key", GUILayout.Width(70)))
		{
			SL_NGUISpriteAtlas atlasObject = ((SL_NGUISpriteAtlas)target);
			
			atlasObject.localizedKey = selectedKey;
		}
	}
}
} //namespace SmartLocalization.Editor
#endif
