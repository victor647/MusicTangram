using UnityEngine;
using UnityEngine.Audio;

namespace InteractiveMusicPlayer
{
	/*
	 * second level class, inherited from Music Component but contains audio clip data.
	 * Music Track, Music Stinger and Music Transition Segment inherits from Music Clip.
	 */
	public abstract class MusicClip : MusicComponent
	{
		[Header("Audio Track Settings")]
		[SerializeField]
		private AudioClip _clip = null;
		public AudioClip Clip
		{
			get { return _clip; }
		}
		
		[SerializeField]
		private AudioMixerGroup _mixerBus; 
		protected AudioSource _source;
        
		[Tooltip("If this music is affected by LPF")]
		[SerializeField]
		private bool _lowPassFilter = false;
		[ConditionalHide("_lowPassFilter", true)]
		[SerializeField]
		private float _LPFResonance = 1f;
		[Tooltip("If this music is affected by HPF")]
		[SerializeField]
		private bool _highPassFilter = false;
		[ConditionalHide("_highPassFilter", true)]
		[SerializeField]
		private float _HPFResonance = 1f;
		private AudioLowPassFilter _LPF;
		private AudioHighPassFilter _HPF;

		private bool _isFadingIn;
		protected bool _isFadingOut;
		private float _fadeTimeStamp;
		protected bool _muted;						

		#region CONTROLS
		//for muting music without setting playback volume to 0
		public override void Mute()
		{            
			if (_muted) return;            
			_muted = true;
			if (FadeTime > 0f && _source.isPlaying)
			{
				_fadeTimeStamp = Time.time;
				_isFadingOut = true;
			}
			else
			{
				_source.volume = 0f;
			}            			
		}
        
		//resume the volume back
		public override void UnMute()
		{            
			if (!_muted) return;
			_muted = false;
			if (FadeTime > 0f && _source.isPlaying)
			{
				_fadeTimeStamp = Time.time;
				_isFadingIn = true;
			}
			else
				_source.volume = _volume;         
		}
        
		//if the output mixing bus needs to be changed
		public override void SetOutputBus(AudioMixerGroup bus)
		{
			if (_source) _source.outputAudioMixerGroup = _mixerBus = bus;
		}
		
		//changing volume from external calls
		public override void SetVolume(float v)
		{                        
			base.SetVolume(v);
			if (!_isFadingIn && !_isFadingOut && _source)
			{
				_volume = v;
				if (!_muted)
					_source.volume = _volume;
			}
		}
        
		//changing pan from external calls
		public override void SetPan(float p)
		{                        
			base.SetPan(p);            
			if (_source) _source.panStereo = _pan = p;
		}

		//changing the cutoff of low pass filter
		public override void SetLPFCutoff(float cutoff)
		{
			base.SetLPFCutoff(cutoff);
			if (_LPF) _LPF.cutoffFrequency = _lowPassFilterCutoff = cutoff;                
		}

		//changing the cutoff of high pass filter
		public override void SetHPFCutoff(float cutoff)
		{
			base.SetHPFCutoff(cutoff);
			if (_HPF) _HPF.cutoffFrequency = _highPassFilterCutoff = cutoff;
		}
		#endregion
		
		#region INITIALIZATION
		//creating audio source
		private void Awake()
		{
			_source = gameObject.AddComponent<AudioSource>();
			_source.clip = _clip;
			_source.volume = _volume;
			_source.panStereo = _pan;
			_source.outputAudioMixerGroup = _mixerBus; 
		}

		protected override void Start()
		{                                    
			if (!_clip) //make sure there is an audio clip to play
			{
				Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": Music has no clip to play!");
				return;
			}                                   
			InitializeFilter();			
			
			//if user forgets to input track duration, generates automatically
			if (TrackDuration == BarAndBeat.Zero)            
				GetDurationFromClip();                                        
			base.Start();            
		}        
    
		//for the editor to calculate track duration from clip length
		public virtual void GetDurationFromClip()
		{
			TrackDuration = RealTimetoBarAndBeat(_clip.length);
			TrackLength = BarAndBeatToRealTime(TrackDuration);  
			ConvertDurationsFromBeatToTime();
		}
		
		//initialize filter settings
		protected void InitializeFilter()
		{
			if (_lowPassFilter)
			{
				_LPF = gameObject.AddComponent<AudioLowPassFilter>();
				_LPF.cutoffFrequency = _lowPassFilterCutoff;
				_LPF.lowpassResonanceQ = _LPFResonance;
			}

			if (_highPassFilter)
			{
				_HPF = gameObject.AddComponent<AudioHighPassFilter>();
				_HPF.cutoffFrequency = _highPassFilterCutoff;
				_HPF.highpassResonanceQ = _HPFResonance;
			}
		}
		#endregion
		
		#region PLAYBACK    
  		//overriding pre-entry stage   
        protected override void PreEntryStage()
        {                          
            base.PreEntryStage();   
            if (FadeTime > 0f && !_muted) //fade in
            {
                _fadeTimeStamp = Time.time;	            
                _isFadingIn = true;
            }
            else            
                PlaySource();                                                                      
        }
		
		//play the audio source
        protected virtual void PlaySource()
        {            
            if(!_muted && FadeTime == 0f)
                _source.volume = _volume;           
            _source.PlayOneShot(_clip, 1f);    
            _isFadingOut = false;            
            MusicManager.Instance.voicePlaying++;
        }  
		
		//override stop and check fade settings
        public override void Stop()
        {                                                         
            if (FadeTime > 0f && !_muted) //fade out
            {
                _fadeTimeStamp = Time.time;	            
                _isFadingOut = true;
            }
            else            
                _source.Stop();
            base.Stop();     
            MusicManager.Instance.voicePlaying--;
        }
        #endregion
		
		#region FADE
		//check fade in and out progress based on time stamp
		private void FixedUpdate()
		{
			if (_isFadingIn)
			{
				float progress = (Time.time - _fadeTimeStamp) / FadeTime;                  
				_source.volume = Mathf.Lerp(0f, _volume, progress);
            
				if (PlayingStatus != PlayStatus.Idle && !_source.isPlaying) 
					PlaySource();
				if (progress >= 1)
				{
					_isFadingIn = false;
					FadeTime = 0f;
				}
			}
            
			if (_isFadingOut)
			{
				float progress = (Time.time - _fadeTimeStamp) / FadeTime;                 
				_source.volume = Mathf.Lerp(_volume, 0f, progress);
                
				if (progress >= 1)
				{
					_isFadingOut = false;
					if (PlayingStatus == PlayStatus.Idle)
						_source.Stop();
					FadeTime = 0f;
				}            
			}
		}              
		#endregion
	}		
}