using UnityEditor;
using UnityEngine;

namespace InteractiveMusicPlayer.Editor
{
	[CustomEditor(typeof(MusicEvent))][CanEditMultipleObjects]
	public class MusicEventEditor : UnityEditor.Editor
	{
		private MusicEvent evt;
		public override void OnInspectorGUI () {
			base.OnInspectorGUI();		
			
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Generate Event Name"))
			{									
				evt.GenerateEventName();
			}
			if (GUILayout.Button("Click to Manually Activate"))
			{									
				evt.ManualTrigger();
			}
			GUILayout.EndHorizontal();
		}

		private void OnEnable()
		{
			evt = (MusicEvent) target;
		}
	}
}
