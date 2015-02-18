#if UNITY_4_6

namespace SmartLocalization.Editor{
	using UnityEngine.UI;
	using UnityEngine;
	using UnityEditor;
	using System.Collections;
	
	[CustomEditor(typeof(LocalizedText))]
	public class LocalizedTextInspector : Editor {
		private string selectedKey = null;
		
		void Awake()
		{
			LocalizedText textObject = ((LocalizedText)target);
			if(textObject != null)
			{
				selectedKey = textObject.localizedKey;
			}
		}
		
		public override void OnInspectorGUI (){
			base.OnInspectorGUI ();
			
			selectedKey = LocalizedKeySelector.SelectKeyGUI(selectedKey, true, LocalizedObjectType.STRING);
			
			if(!Application.isPlaying && GUILayout.Button("Use Key", GUILayout.Width(70))){
				LocalizedText textObject = ((LocalizedText)target);
				textObject.localizedKey = selectedKey;
			}
		}
		
	}
}
#endif