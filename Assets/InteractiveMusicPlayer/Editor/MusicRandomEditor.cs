using UnityEditor;
using UnityEngine;

namespace InteractiveMusicPlayer.Editor
{
	[CustomEditor(typeof(MusicRandom))][CanEditMultipleObjects]
	public class MusicRandomEditor : UnityEditor.Editor
	{
		private MusicRandom rnd;
		public override void OnInspectorGUI () {
			base.OnInspectorGUI();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Fill with Children Tracks"))
			{							
				rnd.FillWithChildren();					
			}
			if (GUILayout.Button("Copy Settings to Children"))
			{							
				rnd.CopySettingsToChildren();						
			}
			GUILayout.EndHorizontal();
		}

		private void OnEnable()
		{
			rnd = (MusicRandom) target;
		}
	}
}
