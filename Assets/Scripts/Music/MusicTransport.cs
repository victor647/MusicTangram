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
	void Start ()
	{
		_slider = GetComponent<Slider>();
		if (BaseTrack)
		{
			BaseTrack.PlayCallback += StartRolling;
			BaseTrack.LoopCallback += ResetSlider;			
			BaseTrack.StopCallback += StopRolling;
			_loopDuration = BaseTrack.ExitTime - BaseTrack.EntryTime;
		}
	}	
	
	void StartRolling()
	{
		_slider.value = 0f;
		_isRunning = true;		
	}

	void StopRolling()
	{
		_isRunning = false;
	}
	
	void ResetSlider()
	{
		_timeStamp = Time.time;
		if (MusicInQueue != null) MusicInQueue();
		MusicInQueue = null;		
	}

	private void LateUpdate()
	{
		if (_isRunning)
		{
			_slider.value = (Time.time - _timeStamp) / _loopDuration;
		}

		if (_slider.value > 0.93f)
		{
			if (GameManager.instance.totalShapesInGame > 0 && ReferenceMix.instance.toggleOn && !ReferenceMix.instance.mouseOver)
			{
				ReferenceMix.instance.RefMusicOff();
			}	
			if (GameManager.instance.totalShapesInGame == 0 && !ReferenceMix.instance.toggleOn && !ReferenceMix.instance.mouseOver 
			    && MixerManager.instance.currentMixer == MixerManager.instance.allOn)
			{
				ReferenceMix.instance.RefMusicOn();
			}	
		}
	}
}
