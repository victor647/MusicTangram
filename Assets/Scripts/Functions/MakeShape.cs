using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeShape : MonoBehaviour
{
    public List<Vector2> vertices2D;

    private MeshFilter filter;
    private MeshRenderer meshRenderer;		
    private Mesh mesh;
    private PolygonCollider2D polygonCollider;

    public void GenerateShape()
    {
        //make sure there are mesh components
        filter = GetComponent<MeshFilter>();
        if (!filter)
            filter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        if (!meshRenderer)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        mesh = new Mesh();
        
        //make sure there is a collider
        polygonCollider = GetComponent<PolygonCollider2D>();	
        if (!polygonCollider)
            polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
        
        
        //if there are fewer than 3 points, a shape can't be enclosed
        if (vertices2D.Count == 0)
            return;
        else if (vertices2D.Count < 3)
        {
            Debug.LogError("A shape must have at least 3 vertices!");
            return;
        }
        //generate 3D vertices for the mesh
        Vector3[] vertices3D = new Vector3[vertices2D.Count];			
        for (int i = 0; i < vertices2D.Count; i++)
        {
            vertices3D[i] = vertices2D[i];
        }
		//calculate triangles for the mesh
        Triangulator tri = new Triangulator(vertices2D.ToArray());
        int[] indices = tri.Triangulate();
        mesh.vertices = vertices3D;
        mesh.triangles = indices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        filter.sharedMesh = mesh;
        //generate UV for the mesh
        Vector2[] uv = new Vector2[vertices2D.Count];        
        for (int i = 0; i < vertices3D.Length; i++)
        {
            uv[i] = new Vector2((vertices3D[i].x - mesh.bounds.min.x) / (mesh.bounds.max.x - mesh.bounds.min.x), (vertices3D[i].y - mesh.bounds.min.y) / (mesh.bounds.max.y - mesh.bounds.min.y));
        }			
        mesh.uv = uv;
        //default to white color
        meshRenderer.sharedMaterial.SetColor("Color", new Color32(255, 255, 255, 255));
        //generate vertices for the colliders
        polygonCollider.points = vertices2D.ToArray();
    }

    public void ClearVertices()
    {
        vertices2D = new List<Vector2>();        
        GenerateShape();
    }

    private void Awake()
    {
        GenerateShape();
    }
}