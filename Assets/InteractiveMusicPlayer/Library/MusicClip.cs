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
		[Tooltip("The audio file that this track plays")]
		[SerializeField]
		private AudioClip _clip = null;
		public AudioClip Clip { get { return _clip; }}

		[Tooltip("The output mixer group this music is routed to")]		
		[SerializeField]
		private AudioMixerGroup _mixerBus;
		public AudioMixerGroup MixerBus
		{
			get { return _mixerBus; }
			set { if (_source) _source.outputAudioMixerGroup = _mixerBus = value; }
		}
		
		//the audio source instantiated after game runs
		protected AudioSource _source;
        
		[Tooltip("If this music is affected by low pass filter")]
		[SerializeField]
		private bool _lowPassFilter = false;
		[ConditionalHide("_lowPassFilter", true)]
		[Tooltip("The resonance of the low pass filter")]
		[SerializeField]
		private float _LPFResonance = 1f;
		[Tooltip("If this music is affected by high pass filter")]
		[SerializeField]
		private bool _highPassFilter = false;
		[ConditionalHide("_highPassFilter", true)]
		[Tooltip("The resonance of the high pass filter")]
		[SerializeField]
		private float _HPFResonance = 1f;
		//the low pass filter to be instantiated after game runs
		private AudioLowPassFilter _LPF;
		//the high pass filter to be instantiated after game runs
		private AudioHighPassFilter _HPF;

		//the status indicating if the music is in a fading phase
		private bool _isFadingIn;
		protected bool _isFadingOut;
		//the time stamp of fade start time
		private float _fadeTimeStamp;
		//the status indicating if the music is muted
		protected bool _isMuted;						

		#region CONTROLS
		//for muting music without setting playback volume to 0
		public override void Mute()
		{            			
			if (_isMuted) return; //avoid repeating mute commands  			
			_isMuted = true; //set the mute status			
			if (FadeTime > 0f && _source.isPlaying) //decide if a fade out is needed
			{
				_fadeTimeStamp = Time.time;
				_isFadingOut = true; //set the fade status
			}
			else						
				_source.volume = 0f; //if no need to fade out, simply set audio source volume to 0			            			
		}
        
		//resume the volume back
		public override void UnMute()
		{            
			if (!_isMuted) return;			
			_isMuted = false;
			if (FadeTime > 0f && _source.isPlaying)
			{
				_fadeTimeStamp = Time.time;
				_isFadingIn = true;
			}
			else
				_source.volume = _volume;         
		}
		
		//changing volume from external calls and events
		public override void SetVolume(float v)
		{                        
			base.SetVolume(v); //the base method calls each child components to recursively set volume
			//if currently fading, protect the audio source from changing volume
			if (!_isFadingIn && !_isFadingOut && _source) 
			{
				_volume = v;
				if (!_isMuted) //if the music is muted, only change stored volume, don't change source volume
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
			InitializeFilter();		
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
		
		protected override void Start()
		{                                    
			if (!_clip) //make sure there is an audio clip to play
			{
				Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": Music has no clip to play!");
				return;
			}                                   							
			//if user forgets to input track duration, generates automatically
			if (TrackDuration == BarAndBeat.Zero)            
				GetDurationFromClip();                                        
			base.Start();            
		}        
    
		//for the editor to calculate track duration from clip length
		public void GetDurationFromClip()
		{
			TrackDuration = RealTimetoBarAndBeat(_clip.length); //convert clip duration in seconds to bars/beats			
			ConvertDurationsFromBeatToTime();
		}				
		#endregion
		
		#region PLAYBACK    
  		//overriding pre-entry stage   
        protected override void PreEntryStage()
        {                          
            base.PreEntryStage();   
            if (FadeTime >= 0.01f && !_isMuted) //decide if the music should fade in
            {
                _fadeTimeStamp = Time.time;	            
                _isFadingIn = true;
            }
	        else	        		        
		        PlaySource(); //directly play audio source if no fade in needed	        
        }
		
		//play the audio source
        protected virtual void PlaySource()
        {       
	        if (FadeTime < 0.01f && !_isMuted)
	        	_source.volume = _volume; //directly set audio source volume
            _source.PlayOneShot(_clip, 1f); //play the audio clip    
            _isFadingOut = false; //reset fade out status            
            MusicManager.Instance.voicePlaying++; //add a voice to the music manager
        }  
		
		//override stop and check fade settings
        public override void Stop()
        {                                                         
            if (FadeTime > 0f && !_isMuted)
            {
                _fadeTimeStamp = Time.time;	            
                _isFadingOut = true;
            }
            else            
                _source.Stop();
            base.Stop();     
            MusicManager.Instance.voicePlaying--; //remove a voice from the music manager
        }
        #endregion
		
		#region FADE
		//check fade in and out progress based on time stamp
		private void FixedUpdate()
		{
			if (_isFadingIn)
			{
				if (_isFadingOut) _isFadingOut = false; //stops fading out
				float progress = (Time.time - _fadeTimeStamp) / FadeTime; //the percentage of passed fading time                 
				_source.volume = Mathf.Lerp(0f, _volume, progress);
				//only play audio source in the first frame of fading in
				if (PlayingStatus != PlayStatus.Idle && !_source.isPlaying) 
					PlaySource();
				if (progress >= 1) //the last frame of fading
				{
					_isFadingIn = false;
					FadeTime = 0f; //reset the fade time in case other events change it
				}
			}
            
			if (_isFadingOut)
			{
				if (_isFadingIn) _isFadingIn = false;
				float progress = (Time.time - _fadeTimeStamp) / FadeTime;                 
				_source.volume = Mathf.Lerp(_volume, 0f, progress);                
				if (progress >= 1)
				{
					_isFadingOut = false;
					if (PlayingStatus == PlayStatus.Idle) //don't stop the music if the fading is from a mute call
						_source.Stop();
					FadeTime = 0f;
				}            
			}
		}              
		#endregion
	}		
}