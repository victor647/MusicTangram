using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScreenPosition))]
public class ScreenPositioneEditor : Editor
{
	private ScreenPosition sp;
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
        
		if (GUILayout.Button("Update Position"))
		{
			sp.UpdatePosition();
		}		
	}
	
	private void OnEnable()
	{
		sp = (ScreenPosition) target;
		sp.UpdatePosition();
	}
}
