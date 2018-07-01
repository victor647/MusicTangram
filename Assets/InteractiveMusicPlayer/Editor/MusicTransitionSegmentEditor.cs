using UnityEditor;
using UnityEngine;

namespace InteractiveMusicPlayer.Editor
{
	[CustomEditor(typeof(MusicTransitionSegment))][CanEditMultipleObjects]
	public class MusicTransitionSegmentEditor : UnityEditor.Editor
	{
		private MusicTransitionSegment track;
		public override void OnInspectorGUI () {
			base.OnInspectorGUI();
			if (GUILayout.Button("Calculate Duration from Clip"))
			{											
				track.GetDurationFromClip();				
			}
		}

		private void OnEnable()
		{
			track = (MusicTransitionSegment) target;
		}
	}
}
