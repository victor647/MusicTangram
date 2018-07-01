using UnityEngine;
using UnityEngine.Audio;

namespace  InteractiveMusicPlayer
{
    public class MusicStinger : MusicClip {

        [Header("Stinger Settings")]        
        [Tooltip("From where the stinger can be triggered")]
        [SerializeField]
        private MusicComponent _hostMusic = null;                          

        #region INITIALIZATION
        protected override void OnValidate()
        {                                       
            LoopCount = 1;               
            _playOnStart = false;
            base.OnValidate();
            if (Clip) gameObject.name = Clip.name + " (stinger)";
        }                              
        
        protected override void Start()
        {            
            if (!_hostMusic)
            {
                Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": Stinger has no host to trigger!");
                return;
            }            
                        
            CopyRhythmSettings(_hostMusic); //make sure the tempo always sync with the host
            if (_transitionInterval == TransitionType.CustomPositions)
                _hostMusic.LoopCallback += StartCheckingGrid;
            else
                _hostMusic.PlayCallback += StartCheckingGrid;
            _hostMusic.StopCallback += StopCheckingGrid;                        
            base.Start();            
        }
        #endregion
        
        #region PLAYBACK
        //for registering to the delegate of host
        private void StartCheckingGrid()
        {
            if (_transitionInterval == TransitionType.CustomPositions) //getting each exit point time stamp if custom exit positions
            {
                _customPositionIndex = 0;
                UpdateTimeStamp();
                foreach (var delayTime in _customPositionsRealTime)
                {
                    Invoke("UpdateTimeStamp", delayTime);
                }
                return;
            }            
            if (_transitionInterval != TransitionType.Immediate)
                InvokeRepeating("UpdateTimeStamp", 0f, GridLength);
        }        
        
        //for registering to the delegate of host
        private void StopCheckingGrid()
        {
            CancelInvoke("UpdateTimeStamp");            
        }

        //play event from external call
        public void TriggerStinger()
        {
            if (_transitionInterval == TransitionType.Immediate)
            {
                Play();                
            }
            else
            {                                                                                                                                                      
                float transitionTime = GridLength - (Time.time - _transitionCheckTimeStamp) - EntryTime;

                if (transitionTime > 0f)                
                    Invoke("Play", transitionTime);                         
                else if (_transitionInterval == TransitionType.ExitCue || _transitionInterval == TransitionType.CustomPositions)                                                                                                
                    Play();//if already passed the entry position
                else
                    Invoke("TransitionExit", transitionTime + GridLength); //wait for next grid                                                                                                 
                    
                if (_debugLog) Debug.Log(Time.time + ": " + Name + " is triggered");
            }
        }        

        //play the stinger
        public override void Play()
        {
            Stop(); //if triggered repeatedly, override playhead                        
            base.Play();
            _source.PlayOneShot(Clip, 1f); 
	    MusicManager.Instance.voicePlaying++;            
        }
        
        //do not invoke repeat transition check in stinger
        protected override void LoopStart()
        {
            PlayingStatus = PlayStatus.Playing;  
            Invoke("PostExitStage", ExitTime - EntryTime);                                                             
            gameObject.name = "#Playing# " + Name;            
        }
                
        //lite version of stop
        public override void Stop()
        {            
            PlayingStatus = PlayStatus.Idle;
            CancelInvoke("OnBeat"); 
            CancelInvoke("PostExitStage");
            gameObject.name = Name;     
	    MusicManager.Instance.voicePlaying--;                   
        }
        #endregion
    }

}