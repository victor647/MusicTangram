using System.Collections;
using InteractiveMusicPlayer;
using UnityEngine;
using UnityEngine.Audio;

public class MixerManager : MonoBehaviour {
	
	public static MixerManager instance;
		
	[ReadOnly] public AudioMixerSnapshot currentMixer;
	
	public AudioMixerSnapshot refOnly;
	public AudioMixerSnapshot sampleOnly;
	public AudioMixerSnapshot selfOnly;
	public AudioMixerSnapshot allOn;	
	public AudioMixerGroup self;
	public MusicSwitch sampleTrack;

	private void Awake ()
	{
		if (!instance)
			instance = this;
		else		
			Destroy(instance.gameObject);		
	}
	
	public void SetMixerSnapshot(AudioMixerSnapshot snapshot, float time)
	{		
		snapshot.TransitionTo(time);		
		currentMixer = snapshot;		
	}
	
	private void Start()
	{
		currentMixer = allOn;		
	}

}
