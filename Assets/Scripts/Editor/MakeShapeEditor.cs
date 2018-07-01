using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MakeShape))]
public class MakeShapeEditor : Editor
{
    private MakeShape ms;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        if (GUILayout.Button("Generate Shape"))
        {
            ms.GenerateShape();
        }
        if (GUILayout.Button("Clear Vertices"))
        {
            ms.ClearVertices();
        }
    }

    private void OnEnable()
    {
        ms = (MakeShape) target;
        ms.GenerateShape();
    }
}