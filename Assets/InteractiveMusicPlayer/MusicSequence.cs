using System.Collections.Generic;
using UnityEngine;

namespace InteractiveMusicPlayer
{
    public class MusicSequence : MusicComponent
    {        
        private MusicComponent _currentTrack;        
        private int _currentTrackIndex;

        // Use this for initialization
        protected override void Start()
        {                                            
            gameObject.name += " (sequence)";
            base.Start();
            CalculateSequenceDuration();    
        }

        public void CalculateSequenceDuration()
        {
            if (_childComponents.Count < 2)
            {
                Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": sequence must have at least 2 tracks!");
                return;
            }

            CopyRhythmSettings(_childComponents[0]);
            CopyPositionSettings(_childComponents[0]);
            for (int i = 1; i < _childComponents.Count; i++)
            {                                
                if (i == _childComponents.Count - 1 && _childComponents[i].PostExit) //sync sequence exit position with the last track
                {
                    PostExit = true;
                    ExitPosition = RealTimetoBarAndBeat(TrackLength + _childComponents[i].ExitTime);
                }

                //if there is infinite loop treat that as one
                int adjustedLoopCount = 1;
                if (_childComponents[i].LoopCount != 0)
                    adjustedLoopCount = _childComponents[i].LoopCount;                                    
                //add the duration of each track to the sequence
                TrackLength += (_childComponents[i].ExitTime - _childComponents[i].EntryTime) * adjustedLoopCount;
                TrackDuration = RealTimetoBarAndBeat(TrackLength);
            }
        }
        
        public override void Play()
        {
            _currentTrackIndex = 0; //reset the sequence
            _currentTrack = _childComponents[0];
            _currentTrack.LoopCallback += SyncStart;
            _currentTrack.Play(); //play the first track in the sequence            
            base.Play();
        }

        private void PlayNextTrack()
        {
            //end the sequence if reaching the last one (list element is 1 less than count)
            if (_currentTrackIndex == _childComponents.Count - 1)
            {
                if (LoopCount == 0 || RemainingLoops > 0) //if this sequence needs to loop
                    _currentTrackIndex = -1;
                else //if this sequence finishes playing                                 
                    return;                
            }            
            _currentTrackIndex++;            
            _currentTrack.TransitionTo(_childComponents[_currentTrackIndex]); //generate the exit-cue transition event to the next track
            _currentTrack = _childComponents[_currentTrackIndex];
            _currentTrack.LoopCallback += SyncStart;
        }

        private void SyncStart()
        {   
            CopyRhythmSettings(_currentTrack);
            if (_currentTrack.LoopCount == 0)
            {
                CopyPositionSettings(_currentTrack);
                _playheadPosition = _currentTrack.EntryPosition;    
            }                                        
            else if (_currentTrack.RemainingLoops == 1)
            {
                //Get the info of the next track in sequence each time a track plays
                _currentTrack.LoopCallback -= SyncStart;
                Invoke("PlayNextTrack", 0.1f);
            }                                                                                        
        }

        public override void Stop()
        {
            base.Stop();
            if (_currentTrack.LoopCount == 0)
            {
                CalculateSequenceDuration();                   
            }     
        }

    }
}