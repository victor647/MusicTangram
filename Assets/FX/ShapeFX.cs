using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(ParticleSystem))]
public class ShapeFX : MonoBehaviour
{

	public bool playOnStart = false;
	public MeshType meshType;
	
	private Mesh _mesh;
	
	public enum MeshType
	{
		Renderer,
		Shape
	}
	
	private ParticleSystem _particleSystem;
	private ParticleSystemRenderer _particleSystemRenderer;

	// Use this for initialization
	void Awake ()
	{
		
		
		
	}

	void Start()
	{
		SetMeshFromParent();
		if (playOnStart)
		{
			_particleSystem.Play();
		}
	}
	// Update is called once per frame
	void Update () {
		//SetMeshFromParent();
	}

	[ContextMenu ("Set Mesh")]
	public void SetMeshFromParent()
	{
		
		_particleSystem = GetComponent<ParticleSystem>();
		_particleSystemRenderer = GetComponent<ParticleSystemRenderer>();
		MeshFilter mf = GetComponentInParent<MeshFilter>();
		MeshRenderer mr = GetComponentInParent<MeshRenderer>();
		if (mf && mr)
		{
			_mesh = mf.mesh;
			if (meshType == MeshType.Renderer)
			{
				_particleSystemRenderer.mesh = _mesh;
			} 
			else if (meshType == MeshType.Shape)
			{
				var psshape = _particleSystem.shape;
				psshape.mesh = _mesh;
				psshape.meshRenderer = mr;
			} 
			
			var psm = _particleSystem.main;
			psm.startColor = mr.sharedMaterial.color;
		}
		else
		{
			Debug.LogError("No Mesh Found From Parent.");
		}
	}
	
	
}
