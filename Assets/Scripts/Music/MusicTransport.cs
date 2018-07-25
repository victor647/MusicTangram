using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InteractiveMusicPlayer;

public class MusicTransport : MonoBehaviour
{
	//public static MusicTransport instance;
	private Slider _slider;	
	public MusicTrack BaseTrack;	
	private bool _isRunning;
	private float _timeStamp;
	public delegate void WaitToPlay();
	public static WaitToPlay MusicInQueue;		
	private float _loopDuration;

	// Use this for initialization
	private void Start ()
	{
		_slider = GetComponent<Slider>();
		if (BaseTrack)
		{
			BaseTrack.PlayCallback += StartRolling;
			BaseTrack.LoopCallback += ResetSlider;			
			BaseTrack.StopCallback += StopRolling;			
		}
	}

	private void StartRolling()
	{
		_loopDuration = BaseTrack.ExitTime - BaseTrack.EntryTime;
		_slider.value = 0f;
		_isRunning = true;		
	}

	private void StopRolling()
	{
		_isRunning = false;
	}

	private void ResetSlider()
	{
		_timeStamp = Time.time;
		if (MusicInQueue != null) MusicInQueue();
		MusicInQueue = null;		
	}

	private void Update()
	{
		if (_isRunning)
		{
			_slider.value = (Time.time - _timeStamp) / _loopDuration;
		}			
		
		if (!ReferenceMix.instance) return; //sandbox mode doesn't have ref mix
		
		if (_slider.value > 0.93f && !ReferenceMix.instance.mouseOver)
		{
			if (LevelManager.instance.totalShapesInGame > 0 && ReferenceMix.instance.Toggle.isOn)
			{
				MixerManager.instance.lastMixer = MixerManager.instance.allOn;
				ReferenceMix.instance.Toggle.isOn = false;
			}	
			if (LevelManager.instance.totalShapesInGame == 0 && !ReferenceMix.instance.Toggle.isOn 
			    && MixerManager.instance.currentMixer == MixerManager.instance.allOn)
			{
				ReferenceMix.instance.Toggle.isOn = true;				
			}	
		}
	}
}
