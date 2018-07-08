using System.Collections;
using System.Collections.Generic;
using InteractiveMusicPlayer;
using UnityEngine;

public class MusicOnDrag : MonoBehaviour {

	private MeshRenderer _mesh;
	private Material _originalMaterial;
	private Material _tempMaterial;
	private Color32 _tempColor;	
	private ShapeMusic _shapeMusic;
	private bool _onDrag;

	private void Start()
	{
		_shapeMusic = GetComponent<ShapeMusic>();
		_mesh = GetComponent<MeshRenderer>();	
	}
	
	private void OnMouseDown()
	{					
		_originalMaterial = _mesh.material;
		_tempMaterial = new Material(_originalMaterial);
		_tempColor = _tempMaterial.color;
		_tempColor.a = 127;
		_tempMaterial.color = _tempColor;
		_mesh.material = _tempMaterial;
		_onDrag = true;
		
		if (_shapeMusic.music.PlayingStatus != MusicComponent.PlayStatus.Idle)
		{
			MixerManager.instance.SetMixerSnapshot(MixerManager.instance.selfOnly, 1f);
			_shapeMusic.music.SetOutputBus(MixerManager.instance.self);	
		}	
	}

	private void OnMouseUp()
	{
		_mesh.material = _originalMaterial;
		_onDrag = false;
		
		if (_shapeMusic.music.PlayingStatus != MusicComponent.PlayStatus.Idle)
		{
			MixerManager.instance.SetMixerSnapshot(MixerManager.instance.lastMixer, 1f);
			Invoke("ResetMixer", 1f);
		}		
	}

	private void ResetMixer()
	{
		if (!_onDrag)
			_shapeMusic.music.SetOutputBus(MixerManager.instance.shape);   
	}
}
