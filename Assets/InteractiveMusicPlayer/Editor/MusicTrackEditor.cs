using UnityEditor;
using UnityEngine;

namespace InteractiveMusicPlayer.Editor
{
	[CustomEditor(typeof(MusicTrack))][CanEditMultipleObjects]
	public class MusicTrackEditor : UnityEditor.Editor
	{
		private MusicTrack track;
		public override void OnInspectorGUI () {
			base.OnInspectorGUI();
			if (GUILayout.Button("Calculate Duration from Clip"))
			{											
				track.GetDurationFromClip();		
			}			
		}

		private void OnEnable()
		{
			track = (MusicTrack) target;
		}
	}
}
