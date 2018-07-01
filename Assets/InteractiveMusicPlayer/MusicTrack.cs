using System;
using UnityEngine;
using UnityEngine.Audio;

namespace InteractiveMusicPlayer
{
    public class MusicTrack : MusicClip 
    {      
        [Tooltip("If this clip should be copy and pasted through the duration of track")]
        [SerializeField]
        private bool _repeatThroughTrack = false;                           
        
        #region INITIALIZATION

        protected override void OnValidate()
        {            
            if (_repeatThroughTrack)
            {
                PreEntry = false;
                PostExit = false;
            }                
            if (Clip) gameObject.name = Clip.name;
            base.OnValidate();
        }
        #endregion
        
        #region PLAYBACK    

        protected override void PlaySource()
        {            
            if(!_muted && FadeTime == 0f)
                _source.volume = _volume; 
            if (_repeatThroughTrack) //use built-in loop for repeating clips or non-post-exit loops
            {
                _source.Play();
                _source.loop = true;                                   
            }
            else //play the clip once every time it should loop            
                _source.PlayOneShot(Clip, 1f);    
            _isFadingOut = false;            
            MusicManager.Instance.voicePlaying++;
        }        
        
        public override void Stop()
        {                                                         
            base.Stop();  
            CancelInvoke("PreEntryBeforeLoopEnd");            
        }

        protected override void LoopStart()
        {            
            if (PreEntry)
                Invoke("PreEntryBeforeLoopEnd", ExitTime - 2 * EntryTime);
            base.LoopStart();  
        }

        //if the track has pre-entry, it should play before the exit position
        private void PreEntryBeforeLoopEnd()
        {
            if ((LoopCount > 1 && RemainingLoops > 0) || LoopCount == 0)                            
                _source.PlayOneShot(Clip, 1f);                        
        }
        
        //play the clip again when reaching loop point and has remaining loop counts
        protected override void LoopEnd()
        {               
            if (_repeatThroughTrack)            
                _source.Stop();
            base.LoopEnd();            
            if ((LoopCount > 1 && RemainingLoops > 0) || LoopCount == 0)
            {
                if (!_repeatThroughTrack && !PreEntry) 
                    _source.PlayOneShot(Clip, 1f);
                if (_repeatThroughTrack)                
                    _source.Play();                 
            }
        }
        #endregion
    }
}