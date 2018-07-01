using System;
using UnityEngine;
using UnityEngine.Audio;

namespace InteractiveMusicPlayer
{
    public class MusicTransitionSegment : MusicClip
    {
        [Header("Transition Segment Settings")]
        [SerializeField]
        private MusicComponent _origin = null;
        public MusicComponent Origin
        {
            get { return _origin; }
        }
        
        [SerializeField]
        private MusicComponent _destination = null; 
        public MusicComponent Destination
        {
            get { return _destination; }
        }    
		
        #region INITIALIZATION

        //make sure loop is not available and transition interval has to be exit cue
        protected override void OnValidate()
        {
            LoopCount = 1;
            _transitionInterval = TransitionType.ExitCue;
            _playOnStart = false;
            base.OnValidate();
            if (Clip) gameObject.name = Clip.name + " (transition segment)"; 
        }

        protected override void Start()
        {                                                    
            if (TrackDuration == BarAndBeat.Zero && Clip) //if user forgets to input track duration                
                GetDurationFromClip();  
            if (_destination)
                _transitionTarget = _destination;
            MusicManager.Instance.transitionSegments.Add(this); //register this segment to the origin
            InitializeFilter();			            
        }

        #endregion
        
        #region PLAYBACK
        public override void Play()
        {
            base.Play();
            _source.Play();
            MusicManager.Instance.voicePlaying++;
        }

        public override void Stop()
        {
            if (!_source.isPlaying) return; //prevent origin music from stopping segments playing
            base.Stop();            
        }
        
        //enter transition segment main part
        protected override void LoopStart()
        {
            PlayingStatus = PlayStatus.Playing;      
            if (EntryPosition == ExitPosition)
                PlayDestination();
            else
                Invoke("PlayDestination", GridLength - _transitionTarget.EntryTime);
            gameObject.name = "#Playing# " + Name;            
        }
		
        //enter the destination music
        private void PlayDestination()
        {            
            _transitionTarget.Play();            
            Invoke("PostExitStage", _transitionTarget.EntryTime);                    
        }
        #endregion
    }
}