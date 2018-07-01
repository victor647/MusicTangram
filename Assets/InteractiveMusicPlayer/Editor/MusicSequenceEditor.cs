using UnityEditor;
using UnityEngine;

namespace InteractiveMusicPlayer.Editor
{
	[CustomEditor(typeof(MusicSequence))][CanEditMultipleObjects]
	public class MusicSequenceEditor : UnityEditor.Editor
	{
		private MusicSequence seq;
		public override void OnInspectorGUI () {
			base.OnInspectorGUI();
			GUILayout.BeginHorizontal();
			if (GUILayout.Button("Calculate Sequence Duration"))
			{							
				seq.CalculateSequenceDuration();					
			}
			if (GUILayout.Button("Copy Settings to Children"))
			{							
				seq.CopySettingsToChildren();						
			}
			GUILayout.EndHorizontal();
		}

		private void OnEnable()
		{
			seq = (MusicSequence) target;
		}
	}
}
