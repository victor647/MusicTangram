using UnityEngine;

namespace InteractiveMusicPlayer
{
    public class MusicLayers : MusicComponent
    {	                
        //for the editor to automatically find the max length of the clips
        public void CalculateTrackDuration()
        {
            BarAndBeat maxDuration = BarAndBeat.Zero;
            foreach (var child in _childComponents)
            {                
                child.CopyTransitionSettings(this);                
                if (child.TrackDuration > maxDuration) //update the duration when finding a longer sub-track
                {
                    maxDuration = child.TrackDuration;
                    ExitPosition = child.ExitPosition;        
                }
            }
            
            TrackDuration = maxDuration;            
            ConvertDurationsFromBeatToTime();
        }
        
        protected override void Start()
        {                                    
            if (transform.childCount < 2)
            {
                Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": Layered music must have at least 2 tracks!");
                return;                
            }                                                               
            
            gameObject.name += " (layer)";
            base.Start();              
            foreach (var child in _childComponents) //register play events for each sub-track
            {                
                PlayCallback += child.Play;
                StopCallback += child.Stop;
            }       
        }
    }
}