using UnityEditor;
using UnityEngine;

namespace InteractiveMusicPlayer.Editor
{
	[CustomEditor(typeof(MusicLayers))][CanEditMultipleObjects]
	public class MusicLayersEditor : UnityEditor.Editor
	{
		private MusicLayers musicLayer;
		public override void OnInspectorGUI () {
			base.OnInspectorGUI();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Calculate Track Duration"))
			{							
				musicLayer.CalculateTrackDuration();					
			}
			if (GUILayout.Button("Copy Settings to Children"))
			{							
				musicLayer.CopySettingsToChildren();						
			}
			GUILayout.EndHorizontal();
		}

		private void OnEnable()
		{
			musicLayer = (MusicLayers) target;
		}
	}
}
