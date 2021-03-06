﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveMusicPlayer;

public class Paintable : MonoBehaviour
{
	private MeshRenderer _mesh;
	private string _musicName;
	private ShapeMusic _shapeMusic;
	private ShapeInfo _shapeInfo;
	
	private void OnTriggerStay2D(Collider2D col)
	{		
		if (col.CompareTag("Palette") && !Input.GetMouseButton(0) && col.transform.position.z < 0)
		{
			_mesh.material = col.GetComponent<MeshRenderer>().material;
			_shapeMusic.UpdateColor();
			string colorName = _mesh.material.name.Substring(0, 2);
			if (colorName != "wt")
				_shapeMusic.music.SetSwitch(_musicName + "_" + colorName, 1f);			
			col.tag = "Untagged";
			_shapeInfo.Color = colorName;
		}
	}
	
	// Update is called once per frame
	private void Start ()
	{
		_mesh = GetComponent<MeshRenderer>();
		_shapeMusic = GetComponent<ShapeMusic>();
		_musicName = _shapeMusic.musicPrefab.name;
		_shapeInfo = GetComponent<ShapeInfo>();
	}
}
