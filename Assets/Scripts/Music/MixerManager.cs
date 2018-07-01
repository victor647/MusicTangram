using System.Collections;
using InteractiveMusicPlayer;
using UnityEngine;
using UnityEngine.Audio;

public class MixerManager : MonoBehaviour {
	
	public static MixerManager instance;
	
	[HideInInspector] public AudioMixerSnapshot lastMixer;
	[HideInInspector] public AudioMixerSnapshot currentMixer;
	
	public AudioMixerSnapshot refOnly;
	public AudioMixerSnapshot sampleOnly;
	public AudioMixerSnapshot selfOnly;
	public AudioMixerSnapshot allOn;
	public AudioMixerGroup shape;
	public AudioMixerGroup self;
	public MusicSwitch sampleTrack;
	
	void Awake ()
	{
		if (!instance)
			instance = this;
		else		
			Destroy(instance.gameObject);		
	}
	
	public void SetMixerSnapshot(AudioMixerSnapshot snapshot, float time)
	{
		snapshot.TransitionTo(time);
		lastMixer = currentMixer == sampleOnly || currentMixer == selfOnly? allOn : currentMixer;
		currentMixer = snapshot;
	}
	
	private void Start()
	{
		lastMixer = currentMixer = allOn;
	}

}
