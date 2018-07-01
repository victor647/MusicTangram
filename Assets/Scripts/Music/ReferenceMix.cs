using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using InteractiveMusicPlayer;

public class ReferenceMix : MonoBehaviour
{
	public static ReferenceMix instance;
	public bool toggleOn = true;	
	private MeshRenderer mesh;
	private Material originalMaterial;
	private Material tempMaterial;
	private Color32 tempColor;
	public MusicTrack refMix;
	[HideInInspector] public bool mouseOver;

	private void Awake()
	{
		if (!instance)
			instance = this;
		else		
			Destroy(instance.gameObject);
	}
	
	
	private void Start()
	{
		mesh = GetComponent<MeshRenderer>();
		originalMaterial = mesh.material;						
	}

	private void ChangeColor(byte alpha)
	{		
		
		tempMaterial = new Material(originalMaterial);
		tempColor = tempMaterial.color;
		tempColor.a = alpha;
		tempMaterial.color = tempColor;
		mesh.material = tempMaterial;			
	}

	private void OnMouseEnter()
	{
		mouseOver = true;
	}

	private void OnMouseExit()
	{
		mouseOver = false;
	}

	private void OnMouseUpAsButton()
	{
		if (toggleOn)
			RefMusicOff();
		else
			RefMusicOn();
	}
	
	public void RefMusicOn()
	{
		toggleOn = true;
		mesh.material = originalMaterial;
		refMix.UnMute(1f);
		if (SampleMusic.highlightedObject) SampleMusic.highlightedObject.Reset();
		MixerManager.instance.SetMixerSnapshot(MixerManager.instance.refOnly, 0.8f);
	}
	
	public void RefMusicOff()
	{					
		toggleOn = false;
		ChangeColor(100);		
		refMix.Mute(1f);
		MixerManager.instance.SetMixerSnapshot(MixerManager.instance.lastMixer, 0.8f);
		if (SampleMusic.highlightedObject) SampleMusic.highlightedObject.Reset();
	}
}
