using UnityEditor;
using UnityEngine;

namespace InteractiveMusicPlayer.Editor
{
	[CustomEditor(typeof(MusicStinger))][CanEditMultipleObjects]
	public class MusicStingerEditor : UnityEditor.Editor
	{
		private MusicStinger track;
		public override void OnInspectorGUI () {
			base.OnInspectorGUI();
			if (GUILayout.Button("Calculate Duration from Clip"))
			{							
				track.GetDurationFromClip();						
			}
		}

		private void OnEnable()
		{
			track = (MusicStinger) target;
		}
	}
}
