using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveMusicPlayer;
using UnityEngine.Audio;

public class ShapeMusic : MonoBehaviour
{

	[HideInInspector] public GameObject musicPrefab;
	[HideInInspector] public MusicSwitch music;	
	private bool _waitToPlay = true;
	
	private MeshRenderer _mesh;
	private Material _originalMaterial;
	private Material _transparentMaterial;			
	private bool _onDrag;
	private AudioMixerGroup _originalMixer;

	private void Start()
	{		
		_mesh = GetComponent<MeshRenderer>();			
		UpdateColor();
	}

	public void UpdateColor()
	{
		_originalMaterial = _mesh.material;
		_transparentMaterial = new Material(_originalMaterial);
		Color32 tempColor = _transparentMaterial.color;
		tempColor.a = 127;
		_transparentMaterial.color = tempColor;
	}
	
	private void OnMouseDown()
	{							
		_mesh.material = _transparentMaterial;
		_onDrag = true;
		
		if (music.PlayingStatus != MusicComponent.PlayStatus.Idle && _originalMaterial.name.Substring(0, 2) != "wt")
		{
			MixerManager.instance.SetMixerSnapshot(MixerManager.instance.selfOnly, 0.5f);
			_originalMixer = ((MusicClip) music.CurrentMusic).MixerBus;
			music.CurrentMusic.SetOutputBus(MixerManager.instance.self);	
		}	
	}

	private void OnMouseUp()
	{
		_mesh.material = _originalMaterial;
		_onDrag = false;
		
		if (music.PlayingStatus != MusicComponent.PlayStatus.Idle && _originalMaterial.name.Substring(0, 2) != "wt")
		{
			MixerManager.instance.SetMixerSnapshot(MixerManager.instance.allOn, 0.5f);
			Invoke("ResetMixer", 0.6f);
		}		
	}

	private void ResetMixer()
	{
		if (!_onDrag)
			music.CurrentMusic.SetOutputBus(_originalMixer);   
	}

	private void StartBlink()
	{
		StartCoroutine(Blinking());
		MusicManager.FindMusicByName("bass").BeatCallback -= StartBlink;
	}
	
	private void StopBlink()
	{
		_waitToPlay = false;
	}

	IEnumerator Blinking()
	{
		while (_waitToPlay)
		{
			_mesh.material = _transparentMaterial;
			yield return new WaitForSeconds(music.BeatLength);
			if (!_waitToPlay) break;
			_mesh.material = _originalMaterial;
			yield return new WaitForSeconds(music.BeatLength);
		}
		_mesh.material = _originalMaterial;
	}
	
	// Use this for initialization
	private void OnEnable () {
		Invoke("InitializeMusic", 0.1f);
	}

	private void InitializeMusic()
	{
		music = Instantiate(musicPrefab).GetComponent<MusicSwitch>();
		music.PlayCallback += StopBlink; //stops blinking when music starts playing		
		MusicManager.FindMusicByName("bass").BeatCallback += StartBlink; //starts blinking next beat
		MusicTransport.instance.MusicInQueue += music.Play; //add music to queue			
	}
	
	// Update is called once per frame
	public void Removed()
	{
		CancelInvoke("InitializeMusic");
		if (!music) return;
		
		MusicTransport.instance.MusicInQueue -= music.Play; //add music to queue
		music.Stop();
		Destroy(music.gameObject, music.FadeTime);
	}
}
