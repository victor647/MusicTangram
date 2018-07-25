using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InteractiveMusicPlayer;

public class ShapeMusic : MonoBehaviour
{

	[HideInInspector] public GameObject musicPrefab;
	[HideInInspector] public MusicSwitch music;
	
	// Use this for initialization
	private void OnEnable () {
		Invoke("InitializeMusic", 0.1f);
	}

	private void InitializeMusic()
	{
		music = Instantiate(musicPrefab).GetComponent<MusicSwitch>();		
		MusicTransport.MusicInQueue += music.Play; //add music to queue			
	}
	
	// Update is called once per frame
	public void Removed()
	{
		CancelInvoke("InitializeMusic");
		if (!music) return;
		
		MusicTransport.MusicInQueue -= music.Play; //add music to queue
		music.Stop();
		Destroy(music.gameObject, music.FadeTime);
	}
}
