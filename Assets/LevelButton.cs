using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : MonoBehaviour
{

	public string targetLevel;
	public UISound clickSound;
	public UISound hoverSound;
	private MeshRenderer _mesh;
	private Material _originalMaterial;
	private Material _opaqueMaterial;
	private bool _onHoverCooldown;

	private void Start()
	{
		_mesh = GetComponent<MeshRenderer>();
		_originalMaterial = _mesh.material;
		_opaqueMaterial = new Material(_originalMaterial);
		Color32 tempColor = _opaqueMaterial.color;
		tempColor.a = 200;
		_opaqueMaterial.color = tempColor;
	}

	private void OnMouseDown()
	{
		clickSound.TriggerSound();
	}

	private void OnMouseUpAsButton()
	{
		Scenes.instance.ChangeScene("Level_" + targetLevel);
	}

	private void OnMouseEnter()
	{
		_mesh.material = _opaqueMaterial;		
		if (!_onHoverCooldown)
		{
			hoverSound.TriggerSound();
			_onHoverCooldown = true;
			Invoke("ResetHoverCooldown", 0.5f);
		}
	}

	private void ResetHoverCooldown()
	{
		_onHoverCooldown = false;
	}
	
	private void OnMouseExit()
	{
		_mesh.material = _originalMaterial;
	}
}
