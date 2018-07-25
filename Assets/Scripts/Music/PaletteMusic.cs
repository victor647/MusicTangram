using UnityEngine;
using InteractiveMusicPlayer;

public class PaletteMusic : MonoBehaviour {

	private MeshRenderer _mesh;
	private string _color;
	private Material _originalMaterial;
	private Material _tempMaterial;
	private Color32 _tempColor;

	private void Start()
	{
		_mesh = GetComponent<MeshRenderer>();
		_originalMaterial = _mesh.material;
		_color = _originalMaterial.name.Substring(0, 2);
	}		

	private void OnMouseOver()
	{
		if (Input.GetMouseButtonUp(1))
		{
			MusicManager.PostEvent("TriggerStinger_" + _color);
			_tempMaterial = new Material(_originalMaterial);
			_tempColor = _tempMaterial.color;
			_tempColor.a = 127;
			_tempMaterial.color = _tempColor;
			_mesh.material = _tempMaterial;
			Invoke("ResetColor", 1f);
		}		
	}

	private void ResetColor()
	{
		_mesh.material = _originalMaterial; 
	}
}
