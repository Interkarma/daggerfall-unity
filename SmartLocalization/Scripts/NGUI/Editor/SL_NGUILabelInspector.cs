//
// SL_NGUILabelInspector.cs
// 
// Copyright (c) 2013-2014 Niklas Borglund, Jakob Hillerström
//

//#define SMART_LOC_NGUI //<--- UNCOMMENT THIS FOR NGUI CLASSES

#if SMART_LOC_NGUI
namespace SmartLocalization.Editor
{
	
	using UnityEngine;
	using UnityEditor;
	
	[CustomEditor(typeof(SL_NGUILabel))]
	public class SL_NGUILabelInspector : Editor 
	{
		private string selectedKey = null;
		
		void Awake()
		{
			SL_NGUILabel textObject = ((SL_NGUILabel)target);
			if(textObject != null)
			{
				selectedKey = textObject.localizedKey;
			}
		}
		
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			selectedKey = LocalizedKeySelector.SelectKeyGUI(selectedKey, true, LocalizedObjectType.STRING);
			
			if(!Application.isPlaying && GUILayout.Button("Use Key", GUILayout.Width(70)))
			{
				SL_NGUILabel textObject = ((SL_NGUILabel)target);
				
				textObject.localizedKey = selectedKey;
			}
		}
	}
} //namespace SmartLocalization.Editor
#endif