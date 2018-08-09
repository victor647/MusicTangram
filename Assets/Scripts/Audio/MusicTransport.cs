using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InteractiveMusicPlayer;

public class MusicTransport : MonoBehaviour
{
	public static MusicTransport instance;
	private Slider _slider;
	private MusicComponent _baseTrack;	
	private bool _isRunning;
	private float _timeStamp;
	public delegate void WaitToPlay();
	public WaitToPlay MusicInQueue;		
	private float _loopDuration;
	private bool firstRun = true;
	
	
	private void Awake()
	{
		if (!instance)
			instance = this;
		else		
			Destroy(instance);
	}	
	
	private void Start ()
	{		
		_slider = GetComponent<Slider>();
		_baseTrack = MusicManager.FindMusicByName("bass");
		if (_baseTrack)
		{
			_baseTrack.PlayCallback += StartRolling;
			_baseTrack.LoopCallback += ResetSlider;					
			_baseTrack.StopCallback += StopRolling;			
		}
	}
	
	private void StartRolling()
	{
		_loopDuration = _baseTrack.ExitTime - _baseTrack.EntryTime;
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
			if (_slider.value > 0.93 && firstRun && ReferenceMix.instance)
			{
				ReferenceMix.instance.Toggle.isOn = false; //turn off CD for first time
				firstRun = false;
			}
		}					
	}
}
