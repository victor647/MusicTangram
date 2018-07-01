using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace InteractiveMusicPlayer
{        
    public abstract class MusicComponent : MonoBehaviour {

        //FIELDS
        
        #region SYNCHRONIZATION SETTING
        [Header("Synchronization Settings")]
        [SerializeField]
        private bool _overrideParentRhythm;    
        
        [ConditionalHide("_overrideParentRhythm", false)][SerializeField]
        private float _tempo = 120f;
        public float Tempo
        {
            get { return _tempo; }
        }
        
        [ConditionalHide("_overrideParentRhythm", false)]
        [SerializeField]
        private int _beatsPerBar = 4;
        public int BeatsPerBar
        {
            get { return _beatsPerBar; }
        }

        public enum DurationOfBeat
        {
            QuarterNote, EighthNote, SixteenthNote
        }

        [Tooltip("The duration in note value of a beat")]
        [ConditionalHide("_overrideParentRhythm", false)]
        [SerializeField]
        private DurationOfBeat _beatDuration = DurationOfBeat.QuarterNote;
        public DurationOfBeat BeatDuration
        {
            get { return _beatDuration; }
        }
        
        [Tooltip("The total length of music in bars and beats")]
        [SerializeField]
        private BarAndBeat _trackDuration;	
        public BarAndBeat TrackDuration
        {
            get { return _trackDuration; }
            protected set { _trackDuration = value; }
        }	
	    #endregion
        
        #region ENTRY AND EXIT SETTING
        [Header("Entry and Exit Settings")]
        [SerializeField]
        protected bool _overrideParentLoop;
        
        [Tooltip("0 is infinite, 1 is one shot")] 
        [ConditionalHide("_overrideParentLoop", false)]
        [SerializeField]
        private int _loopCount = 1;
        public int LoopCount
        {
            get { return _loopCount;}
            protected set { _loopCount = value; }
        }
        
        [Tooltip("If the music starts playing when this component is loaded")]
        [SerializeField]
        protected bool _playOnStart;
        
        [ConditionalHide("_playOnStart", true)] 
        [SerializeField]
        private float _fadeInTime = 0f;

        public int RemainingLoops { get; private set; }

        [Tooltip("Pickup bars")]
        [SerializeField]
        private bool _preEntry;
        public bool PreEntry
        {
            get { return _preEntry; }
            protected set { _preEntry = value; }
        }
        
        [ConditionalHide("_preEntry", true)]
        [SerializeField]
        private BarAndBeat _entryPosition;
        public BarAndBeat EntryPosition
        {
            get { return _entryPosition; }
            protected set { _entryPosition = value; }
        }	
        
        [Tooltip("Reverb tails")]
        [SerializeField]
        private bool _postExit;
        public bool PostExit
        {
            get { return _postExit; }
            protected set { _postExit = value; }
        }
        
        [ConditionalHide("_postExit", true)]
        [SerializeField]
        private BarAndBeat _exitPosition;
        public BarAndBeat ExitPosition
        {
            get { return _exitPosition; }
            protected set { _exitPosition = value; }
        }	      
        #endregion
        
        #region TRANSITION SETTING 

        protected enum TransitionType
        {
            Immediate, NextBeat, NextBar, NextGrid, ExitCue, CustomPositions
        }    
        
        [Header("Transition Settings")]
        [SerializeField]
        protected bool _overrideParentTransition; 
        
        [ConditionalHide("_overrideParentTransition", false)]
        [SerializeField]
        protected TransitionType _transitionInterval = TransitionType.NextBar;	
        
        [EnumHide("_transitionInterval", 3, true)]
        [Tooltip("Time interval between possible transiions")]
        [SerializeField]
        private BarAndBeat _gridDuration;
        
        [EnumHide("_transitionInterval", 5, true)]
        [Tooltip("Set custom positions for transition exit")]
        [SerializeField]
        private List<BarAndBeat> _customPositions = new List<BarAndBeat>();
        //keep track of the real time of all the custom positions
        protected readonly List<float> _customPositionsRealTime = new List<float>();
        //keep track of which custom position is currently passed
        protected int _customPositionIndex;
               
        //the current target this music is going to transition to 
        protected MusicComponent _transitionTarget;
        
        //store all music components in child game objects
        protected readonly List<MusicComponent> _childComponents = new List<MusicComponent>();
        private MusicClip[] _childClips;
        
        //the original name, without playback status prepending       
        private string _originalName;
        public string Name
        {
            get { return _originalName; }
        }
        #endregion

        #region AUDIO SETTING
        [Header("Audio Settings")]
        [SerializeField]
        protected bool _overrideParentAudio; 
        
        [ConditionalHide("_overrideParentAudio", false)]
        [Tooltip("Range from 0 to 1")]
        [SerializeField]
        protected float _volume = 1f;
        
        [ConditionalHide("_overrideParentAudio", false)]
        [Tooltip("Range from -1 to 1")]
        [SerializeField]
        protected float _pan = 0f;
        
        [ConditionalHide("_overrideParentAudio", false)]
        [Tooltip("Range from 10 to 22000")]
        [SerializeField]
        protected float _lowPassFilterCutoff = 22000f;
        
        [ConditionalHide("_overrideParentAudio", false)]
        [Tooltip("Range from 10 to 22000")]
        [SerializeField]
        protected float _highPassFilterCutoff = 10f;

        //the current fade time used         
        private float _fadeTime;
        //synchronize fade time to all children
        public float FadeTime
        {
            get { return _fadeTime; }
            set
            {
                _fadeTime = value;
                var clips = GetComponentsInChildren<MusicClip>();
                foreach (var clip in clips)
                {
                    clip._fadeTime = value;
                }                
            }
        }
        #endregion
        
        #region PLAYBACK SETTING
        public enum PlayStatus
        {
            PreEntry, Playing, PostExit, Idle 
        }
        
        [Header("Playing Status")]        
        [ReadOnly]
        [SerializeField]
        private PlayStatus _playingStatus = PlayStatus.Idle;
        public PlayStatus PlayingStatus
        {
            get { return _playingStatus; }
            protected set { _playingStatus = value; }
        }
        
        [ReadOnly]
        [SerializeField]
        protected BarAndBeat _playheadPosition = BarAndBeat.Zero;
        
        [Tooltip("If playing info is shown in console")]
        [SerializeField]
        protected bool _debugLog;
        
        //the timing data, read only from outside
        private float _secondPerBeat;
        public float BeatLength
        {
            get { return _secondPerBeat; }
        }

        private float _secondPerBar;
        public float BarLength
        {
            get { return _secondPerBar; }
        }
        
        private float _trackDurationRealTime;
        public float TrackLength
        {
            get { return _trackDurationRealTime; }
            protected set { _trackDurationRealTime = value; }
        }      
        
        private float _entryPositionRealTime;
        public float EntryTime
        {
            get { return _entryPositionRealTime; }
        }  
        
        private float _exitPositionRealTime;
        public float ExitTime
        {
            get { return _exitPositionRealTime; }
        }  
        
        private float _gridLengthRealTime;
        public float GridLength
        {
            get { return _gridLengthRealTime; }
        }          
        protected float _transitionCheckTimeStamp;
        
		
        //register other events to this track's playing events
        public delegate void MusicCallback();
        public MusicCallback PlayCallback;        
        public MusicCallback LoopCallback;
        public MusicCallback ExitCallback;
        public MusicCallback StopCallback;
        #endregion
                
        //FUNCTIONS
        
        #region INITIALIZATION        
        //if no parent found, always override settings
        private void AlwaysOverride()
        {
            _overrideParentRhythm = true;
            _overrideParentTransition = true;
            _overrideParentLoop = true;
            _overrideParentAudio = true;
        }
        
        //for the editor button
        public void CopySettingsToChildren()
        {
            foreach (Transform child in transform)
            {
                MusicComponent track = child.GetComponent<MusicComponent>();
                track.CopyPositionSettings(this);
                track.CopyRhythmSettings(this);
                track.CopyTransitionSettings(this);
                track.CopyAudioSettings(this);
                if (!track._overrideParentLoop)
                    track._loopCount = _loopCount;   
                track._debugLog = _debugLog;
            }   
        }
        
        //copy position settings from another music
        protected void CopyPositionSettings(MusicComponent copyFrom)
        {                                  			            
            _preEntry = copyFrom._preEntry;
            _entryPosition = copyFrom._entryPosition;
            _postExit = copyFrom._postExit;
            _exitPosition = copyFrom._exitPosition;
            _trackDuration = copyFrom.TrackDuration;                                                   	                                             
            ConvertDurationsFromBeatToTime();            
        }

        //copy tempo and rhythm settings from another music
        protected void CopyRhythmSettings(MusicComponent copyFrom)
        {
            _tempo = copyFrom.Tempo;						    
            _beatsPerBar = copyFrom.BeatsPerBar;
            _beatDuration = copyFrom.BeatDuration;
            GetBeatDuration();
            ConvertDurationsFromBeatToTime();
        }

        //copy transition settings from another music
        public void CopyTransitionSettings(MusicComponent copyFrom)
        {
            _transitionInterval = copyFrom._transitionInterval;
            _gridDuration = copyFrom._gridDuration;
            _customPositions = copyFrom._customPositions;
            SetTransitionTime();      
        }

        //copy audio settings from another music
        private void CopyAudioSettings(MusicComponent copyFrom)
        {
            _volume = copyFrom._volume;
            _pan = copyFrom._pan;
            _highPassFilterCutoff = copyFrom._highPassFilterCutoff;
            _lowPassFilterCutoff = copyFrom._lowPassFilterCutoff;
        }

        //find the duration in real time of each beat and bar
        private void GetBeatDuration()
        {
            if (_beatsPerBar < 1) _beatsPerBar = 1;
            switch (_beatDuration)
            {
                case DurationOfBeat.QuarterNote:
                    _secondPerBeat = 60f / _tempo;
                    break;
                case DurationOfBeat.EighthNote:
                    _secondPerBeat = 30f / _tempo;
                    break;
                case DurationOfBeat.SixteenthNote:
                    _secondPerBeat = 15f / _tempo;
                    break;
            }            
            _secondPerBar = _secondPerBeat * _beatsPerBar;	
        }
        
        //converts bars/beats to seconds
        protected void ConvertDurationsFromBeatToTime()
        {                        		
            _trackDurationRealTime = BarAndBeatToRealTime(_trackDuration);
        
            //configure post-exit and pre-entry settings
            if (_postExit)
            {
                _exitPositionRealTime = BarAndBeatToRealTime(_exitPosition);
            }
            else
            {
                _exitPosition = _trackDuration;
                _exitPositionRealTime = _trackDurationRealTime;
            }                    
            if (_preEntry)
            {
                _entryPositionRealTime = BarAndBeatToRealTime(_entryPosition);                        
            }
            else
            {
                _entryPosition = BarAndBeat.Zero;
                _entryPositionRealTime = 0f;
            }
        }
        
        //set grid length according to transition types
        private void SetTransitionTime()
        {
            switch (_transitionInterval)
            {
                case TransitionType.Immediate:
                    _gridDuration = BarAndBeat.Zero;
                    _gridLengthRealTime = 0f;
                    break;
                case TransitionType.NextBeat:
                    _gridDuration = BarAndBeat.OneBeat;
                    _gridLengthRealTime = _secondPerBeat;
                    break;
                case TransitionType.NextBar:
                    _gridDuration = BarAndBeat.OneBar;
                    _gridLengthRealTime = _secondPerBar;
                    break;
                case TransitionType.NextGrid:
                    _gridLengthRealTime = BarAndBeatToRealTime(_gridDuration);
                    break;
                case TransitionType.ExitCue:
                    _gridLengthRealTime = _exitPositionRealTime - _entryPositionRealTime;
                    _gridDuration = RealTimetoBarAndBeat(_gridLengthRealTime);
                    break;
                case TransitionType.CustomPositions:
                    _customPositionsRealTime.Clear();
                    foreach (var pos in _customPositions)
                    { //register all the custom positions to real time list
                        _customPositionsRealTime.Add(BarAndBeatToRealTime(pos) - _entryPositionRealTime);
                    }    
                    break;
            }                        
        }        
                   
        //called every time inspector is updated
        protected virtual void OnValidate()
        {                        
            Initialize();
            foreach (var child in _childComponents)
            {                
                if (!child._overrideParentAudio)
                    child.CopyAudioSettings(this);
                if (!child._overrideParentRhythm)                
                    child.CopyRhythmSettings(this);                                    
                if (!child._overrideParentTransition)
                    child.CopyTransitionSettings(this);                
                if (!child._overrideParentLoop)
                    child._loopCount = _loopCount;
                
                child.OnValidate(); //recursively apply the settings
            }            
        }        
        
        //calculate stuff when starting the game
        protected virtual void Start()
        {
            _playingStatus = PlayStatus.Idle;
            if (_exitPosition > _trackDuration)
            {
                //make sure exit position can't be later than track duration
                _exitPosition = _trackDuration;
                _postExit = false;
            }

            Initialize();
            _originalName = gameObject.name; //keep track of the original name
            if (_playOnStart)
            {
                FadeTime = _fadeInTime;
                Invoke("Play", 0.1f);
            }
        }
        
        //check if the tempo and transition is overriden
        private void Initialize()
        {
            //find the children music components
            _childComponents.Clear();
            StopCallback = null;            
            foreach (Transform child in transform)
            {
                var track = child.GetComponent<MusicComponent>();
                if (track)
                {
                    _childComponents.Add(track);
                    StopCallback += track.Stop; //make sure all children stops when this stops
                }                
            }
            _childClips = GetComponentsInChildren<MusicClip>();
            //check tempo and transition override settings to match parent
            var parent = transform.parent;
            if (!parent)
                AlwaysOverride();
            else 
            {
                var parentMusic = parent.GetComponent<MusicComponent>();
                if (parentMusic)
                {                    
                    if (!_overrideParentRhythm)                    
                        CopyRhythmSettings(parentMusic); 
                    if (!_overrideParentAudio)
                        CopyAudioSettings(parentMusic);
                    if (parentMusic is MusicRandom)
                    {
                        _overrideParentLoop = false;
                        _loopCount = parentMusic._loopCount;
                    } else if (!_overrideParentLoop)
                        _loopCount = parentMusic._loopCount;

                    if (parentMusic is MusicSequence || parentMusic is MusicRandom)
                    {
                        _overrideParentTransition = true;
                        _transitionInterval = TransitionType.ExitCue;                         
                    }                    
                    else if (!_overrideParentTransition)
                        CopyTransitionSettings(parentMusic);
                }
                else                
                    AlwaysOverride(); //if on highest hierarchy, always override
            }                           
            
            GetBeatDuration();
            ConvertDurationsFromBeatToTime();	
            SetTransitionTime();          
        }
        #endregion

        #region PLAYBACK              
        //when a play event is received
        public virtual void Play()
        {
            if (PlayCallback != null) PlayCallback();
            _playheadPosition = BarAndBeat.Zero;
            if (_debugLog) Debug.Log(Time.time + ": " + _originalName + " starts playing");
            RemainingLoops = _loopCount;   
            PreEntryStage();                     
        }

        //overload for manually setting fade time
        public void Play(float fadeInTime)
        {
            FadeTime = fadeInTime;
            Play();
        }

        //entering the pre-entry stage
        protected virtual void PreEntryStage()
        {
            InvokeRepeating("OnBeat", _secondPerBeat, _secondPerBeat);
            if (_preEntry)
            {
                _playingStatus = PlayStatus.PreEntry;
                Invoke("LoopStart", _entryPositionRealTime);
                gameObject.name = "#Pre-Entry# " + _originalName;
            }
            else
            {
                LoopStart();
            }                        
        }
		
        //entering the main looping body
        protected virtual void LoopStart()
        {	            
            if (LoopCallback != null) LoopCallback();
            if (_transitionInterval != TransitionType.Immediate && _transitionInterval != TransitionType.CustomPositions &&
                _playingStatus != PlayStatus.Playing)
                InvokeRepeating("UpdateTimeStamp", 0f, _gridLengthRealTime);
            if (RemainingLoops != 0) RemainingLoops--; //decrement the loop count
            _playingStatus = PlayStatus.Playing;              

            if (_transitionInterval == TransitionType.CustomPositions) //getting each exit point time stamp if custom exit positions
            {
                _customPositionIndex = 0;
                UpdateTimeStamp();
                foreach (var delayTime in _customPositionsRealTime)
                {                    
                    Invoke("UpdateTimeStamp", delayTime);
                }
            }
            Invoke("LoopEnd", _exitPositionRealTime - _entryPositionRealTime);
            

            gameObject.name = "#Playing# " + _originalName;
            if (_debugLog) Debug.Log(Time.time + ": " + _originalName + " enters main loop stage");            
        }        	        
        
        //check if the music reaches the loooping point
        protected virtual void LoopEnd()
        {                        
            if ((_loopCount > 1 && RemainingLoops > 0) || _loopCount == 0) //if still need to loop
            {               
                CancelInvoke("OnBeat"); 
                ResetPlayhead();
                InvokeRepeating("OnBeat", _secondPerBeat, _secondPerBeat);
            }
            else                            
                PostExitStage();               
        }

        //entering the post-exit stage
        protected void PostExitStage()
        {
            if (ExitCallback != null) ExitCallback();
            if (_postExit)
            {
                _playingStatus = PlayStatus.PostExit;
                Invoke("Stop", _trackDurationRealTime - _exitPositionRealTime);
                gameObject.name = "#Post-Exit# " + _originalName;
                if (_debugLog) Debug.Log(Time.time + ": " + _originalName + " enters post-exit");
            }
            else
            {
                Stop();
            }
        }       
        
        //when a stop event is received
        public virtual void Stop()
        {
            if (StopCallback != null) StopCallback();
            _playingStatus = PlayStatus.Idle;
            CancelInvoke("OnBeat");             
            CancelInvoke("LoopEnd");                      
            CancelInvoke("TransitionExit");
            _playheadPosition = BarAndBeat.Zero;
            gameObject.name = _originalName;            
            if (_debugLog) Debug.Log(Time.time + ": " + _originalName + " stops playing");
        }
        
        //overload for manually setting fade time
        public void Stop(float fadeOutTime)
        {
            FadeTime = fadeOutTime;
            Stop();
        }
        #endregion
        
        #region TRANSITION
        //when a transition event is received
        public void TransitionTo(MusicComponent target)
        {
            if (!target) //if it is getting a stop on grid event
            {
                if (_transitionInterval == TransitionType.Immediate)                                                        
                {
                    Stop();
                    Debug.LogWarning(MusicManager.Instance.GetObjectPath(gameObject) + 
                        ": Transition is set to immediate, stop on grid event will function the same as normal stop event!");
                }
                else //stop on next grid
                    Invoke("TransitionExit", _gridLengthRealTime - (Time.time - _transitionCheckTimeStamp));                
                return;
            }
            
            _transitionTarget = target;            
            foreach (var segment in MusicManager.Instance.transitionSegments) //check if the destination has any segment to play
            {
                if (segment.Destination == target && segment.Origin == this)
                {
                    if (segment.Clip) //if no clip, treat as direct transition
                        _transitionTarget = segment; //make the segment the target of transition                     
                    break;
                }                         
                if (!segment.Origin && segment.Destination == target)
                    _transitionTarget = segment;
                if (segment.Origin == this && !segment.Destination)
                {
                    _transitionTarget = segment;
                    segment._transitionTarget = target;
                }
                    
            }

            if (_transitionInterval == TransitionType.Immediate)
            {
                _transitionTarget.Play();
                Stop();
            }
            else
            {
                CancelInvoke("LoopEnd"); //make sure this track doesn't loop
                //find the remaining time to next transition point
                float transitionTime = _gridLengthRealTime - (Time.time - _transitionCheckTimeStamp) - _transitionTarget._entryPositionRealTime; 
                if (transitionTime > 0f)                
                    Invoke("TransitionExit", transitionTime);                
                else if (_transitionInterval == TransitionType.ExitCue || _transitionInterval == TransitionType.CustomPositions)                                                                                                
                    TransitionExit(); //if already passed the entry position
                else
                    Invoke("TransitionExit", transitionTime + _gridLengthRealTime); //wait for next grid
                
                if (_debugLog)
                    if (_debugLog) Debug.Log(Time.time + ": " + _originalName + " will transition to " + _transitionTarget._originalName);                
            }
            
        }

        //update time stamp of each transition check point
        protected void UpdateTimeStamp()
        {
            _transitionCheckTimeStamp = Time.time; 
            
            if (_transitionInterval == TransitionType.CustomPositions)
            {
                if (_customPositionIndex == 0) //between loop start and the first point
                {
                    _gridLengthRealTime = BarAndBeatToRealTime(_customPositions[0]) - _entryPositionRealTime;                    
                }
                else if (_customPositionIndex < _customPositions.Count) //get time between two custom positions
                {
                    _gridLengthRealTime = BarAndBeatToRealTime(_customPositions[_customPositionIndex]) -
                                         BarAndBeatToRealTime(_customPositions[_customPositionIndex - 1]);                                        
                }
                else //between loop end and the last check point
                {
                    _gridLengthRealTime = _exitPositionRealTime - BarAndBeatToRealTime(_customPositions[_customPositions.Count - 1]);
                }
                _customPositionIndex++; //go to the next custom position
            }
        }
        
        //called every transition grid
        private void TransitionExit()
        {                                    
            if (!_transitionTarget) //if stop on grid event is received
            {
                Stop();
                if (_debugLog) Debug.Log(Time.time + ": " + _originalName + " stops on grid");
                return;
            }                       
            
            _transitionTarget.Play(); //go to the next track
            //if transition at exit cue position, end the current one with normal post-exit
            if (_transitionInterval == TransitionType.ExitCue || _playheadPosition == _exitPosition)                                             
                Invoke("PostExitStage", _transitionTarget._entryPositionRealTime);                                                              
            else //if transition in the middle of a track, stop directly           
                Invoke("Stop", _transitionTarget._entryPositionRealTime);            

            if (_debugLog) Debug.Log(Time.time + ": " + _originalName + " now transitions to " + _originalName);
        }
        #endregion
        
        #region PLAYHEAD
        //update the playhead position every beat 
        private void OnBeat()
        {         
            _playheadPosition.Beat++;
            if (_playheadPosition.Beat == _beatsPerBar)
            {
                _playheadPosition.Beat = 0;
                _playheadPosition.Bar++;                
            }                      
        }

        //reset the play head to entry position every time a track loops
        protected void ResetPlayhead()
        {
            _playheadPosition = _entryPosition;
            LoopStart();
        }
        #endregion
        
        #region CONTROLS
        //mute the music
        public virtual void Mute()
        {            
            foreach (var clip in _childClips)
            {
                clip.Mute();
            }
        }

        //overload mute with fade time
        public void Mute(float fadeTime)
        {
            _fadeTime = fadeTime;
            Mute();
        }
        
        //unmute the music
        public virtual void UnMute()
        {            
            foreach (var clip in _childClips)
            {
                clip.UnMute();
            }
        }
        
        //overload unmute with fade time
        public void UnMute(float fadeTime)
        {
            _fadeTime = fadeTime;
            UnMute();
        }
        
        //change output mixer bus
        public virtual void ChangeOutputBus(AudioMixerGroup bus)
        {
            foreach (var clip in _childClips)
            {
                clip.ChangeOutputBus(bus);
            }
        }
        
        //change volume along with all of its children
        public virtual void ChangeVolume(float v)
        {            
            _volume = v;
            foreach (var child in _childComponents)
            {
                child.ChangeVolume(v);
            }
        }
        
        //change pan along with all of its children
        public virtual void ChangePan(float p)
        {            
            _pan = p;
            foreach (var child in _childComponents)
            {
                child.ChangePan(p);
            }
        }
        
        //change low pass filter cutoff along with all of its children
        public virtual void SetLPFCutoff(float cutoff)
        {            
            _lowPassFilterCutoff = cutoff;
            foreach (var child in _childComponents)
            {
                child.SetLPFCutoff(cutoff);
            }
        }
        
        //change high pass filter cutoff along with all of its children
        public virtual void SetHPFCutoff(float cutoff)
        {            
            _highPassFilterCutoff = cutoff;
            foreach (var child in _childComponents)
            {
                child.SetHPFCutoff(cutoff);
            }
        }
	
        //conversion between Bar/Beat and real time
        protected float BarAndBeatToRealTime(BarAndBeat b)
        {
            return b.Bar * _secondPerBar + b.Beat * _secondPerBeat;
        }
        
        protected BarAndBeat RealTimetoBarAndBeat(float t)
        {
            int bar = Mathf.FloorToInt(t / _secondPerBar);
            int beat = Mathf.RoundToInt((t - bar * _secondPerBar) / _secondPerBeat);
            return new BarAndBeat(bar, beat);
        }
        #endregion
    }
}