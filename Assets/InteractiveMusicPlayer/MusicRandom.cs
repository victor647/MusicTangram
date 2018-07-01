using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InteractiveMusicPlayer
{
    public class MusicRandom : MusicComponent {

        [System.Serializable]
        public class RandomElement
        {
            public MusicComponent Music;
            public float Percentage;
            [HideInInspector] public float RangeIndex;
            [HideInInspector] public bool HasPlayed;

            public RandomElement(MusicComponent m) //initialize a random element
            {
                Music = m;
                Percentage = 1f;
                RangeIndex = 0f;
                HasPlayed = false;
            }
        }

        private enum RandomMode
        {
            Random, Shuffle
        }

        [Header("Random Settings")]
        [SerializeField]
        private RandomMode _randomMode = RandomMode.Random;
        [EnumHide("_randomMode", 0, true)]
        [SerializeField]
        private bool _avoidRepeat = true;    
        [Tooltip("Pick a new track to play every loop")]
        [SerializeField]
        private bool _randomOnLoop = true;
        private MusicComponent _currentTrack;
        private MusicComponent _nextTrack;
        [SerializeField]
        private List<RandomElement> _randomList = new List<RandomElement>();

        private int _playTimes;	//keep track of the shuffle cycle
        
        
        
        //for the editor to populate random list with child tracks
        public void FillWithChildren()
        {            
            _randomList.Clear();
            foreach (Transform child in transform)
            {                
                _randomList.Add(new RandomElement(child.GetComponent<MusicComponent>()));
            }     
            RecalculateWeightedRandom();
        }
                
        protected override void Start()
        {                        
            if (_randomList.Count < 2) //check if user forgets to populate tracks into random list
            {
                FillWithChildren();
                if (_randomList.Count < 2) //if still can't find sub-tracks
                {
                    Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": Random music needs to have at least 2 tracks!");
                    return;
                }
            }                        
            RecalculateWeightedRandom();
            gameObject.name += " (random)"; 
            base.Start();                                   
        }

        #region RANDOM
        //convert user-input to 100 based random
        private void RecalculateWeightedRandom()
        {
            float totalPercentage = 0f;
            float currentIndex = 0;
            foreach (var item in _randomList)
            {
                totalPercentage += item.Percentage;
            }
            foreach (var element in _randomList)
            {
                element.Percentage *= 100f / totalPercentage;
                currentIndex += element.Percentage; //for example if there are 2 tracks, A will be 0-x, B will be x-100;
                element.RangeIndex = currentIndex;                
            }
        }       
        
        //generate a random track to play
        private MusicComponent PickRandomMusic()
        {
            RandomElement generatedItem = _randomList[0]; //default to the first track
            switch (_randomMode)
            {
                case RandomMode.Random:
                    do
                    {
                        float randomNumber = Random.Range(0f, 100f);
                        foreach (var track in _randomList)
                        {
                            if (randomNumber <= track.RangeIndex)
                            {                                
                                generatedItem = track;
                                break;
                            }
                        }
                    } while (_avoidRepeat && generatedItem.Music == _currentTrack); //choose another one if it's the same as last one

                    break;
                
                case RandomMode.Shuffle:
                    if (_playTimes == _randomList.Count) //reset after shuffle cycle is finished
                    {
                        foreach (var track in _randomList)
                        {
                            track.HasPlayed = false;
                        }
                        _playTimes = 0;
                    }
                    do
                    {
                        float randomNumber = Random.Range(0f, 100f);
                        foreach (var track in _randomList)
                        {
                            if (randomNumber <= track.RangeIndex)
                            {
                                generatedItem = track;                                
                                break;
                            }
                        }
                    } while (generatedItem.HasPlayed || generatedItem.Music == _currentTrack); //generate another random number if the current one has been played
                    generatedItem.HasPlayed = true;
                    _playTimes++;
                    break;
            }
            return generatedItem.Music;            
        }
        #endregion
                
        #region PLAYBACK
        public override void Play()
        {
            _currentTrack = PickRandomMusic(); //randomly choose a new track to play
            CopyRhythmSettings(_currentTrack);
            CopyPositionSettings(_currentTrack); //sync the position with the chosen track            
            PlayCallback = _currentTrack.Play; //register play and stop events to chosen track                                             
            base.Play();            
        }
        
        protected override void LoopStart()
        {
            base.LoopStart();            
            Invoke("GetNextTrack", 1f); //find the next track when this track is playing
        }

        //called slightly after main loop starts each time
        private void GetNextTrack()
        {
            if (LoopCount != 0 && RemainingLoops == 0) return; //go back if no more loop is needed
            
            _nextTrack = _randomOnLoop ? PickRandomMusic() : _currentTrack;  //find the next track
            
            if (_currentTrack != _nextTrack)
            {
                _currentTrack.ExitCallback += UpdateCurrentTrack; //when this track finishes, update to next track
                _currentTrack.LoopCallback -= ResetPlayhead; //no longer want this track to reset play head
                _nextTrack.LoopCallback += ResetPlayhead; //make the next track reset playhead when entering
                _currentTrack.TransitionTo(_nextTrack);    
            }
            else
            {
                foreach (var segment in MusicManager.Instance.transitionSegments)
                {
                    if (segment.Destination == _currentTrack && segment.Origin == _currentTrack)
                    {
                        _currentTrack.TransitionTo(_nextTrack);
                        _currentTrack.CancelInvoke("LoopEnd");
                        break;
                    }
                }
            }            

            if (_debugLog) Debug.Log(Time.time + ": " + "The next track will be " + _nextTrack.gameObject.name);
        }

        //called each loop when the current track exits
        private void UpdateCurrentTrack()
        {
            _currentTrack.ExitCallback -= UpdateCurrentTrack; //get rid of the last track            
            _currentTrack = _nextTrack; //make the next track now the current track
            CopyRhythmSettings(_currentTrack);
            CopyPositionSettings(_currentTrack); //sync the position with the chosen track
            PlayCallback = _currentTrack.Play;                       
        }

        protected override void LoopEnd()
        {
            if (LoopCount != 0 && RemainingLoops == 0){
                PostExitStage();
                return;
            }
            
            if (_currentTrack == _nextTrack || !_randomOnLoop) //when there is no segment                           
                base.LoopEnd();           
        }
        #endregion
    }		  
}