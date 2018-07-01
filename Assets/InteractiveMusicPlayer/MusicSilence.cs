using UnityEngine;

namespace InteractiveMusicPlayer
{
    public class MusicSilence : MusicComponent
    {        
        protected override void Start()
        {            
            if (TrackDuration == BarAndBeat.Zero)
                Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": Silence track duration is zero!");
            else
                ChangeName();
            base.Start();
        }

        protected override void OnValidate()
        {
            PostExit = false;
            PreEntry = false;
            _overrideParentAudio = false;
            base.OnValidate();
            if (TrackDuration != BarAndBeat.Zero)
                ChangeName();
        }

        //set game object name according to silence duration
        private void ChangeName()
        {
            gameObject.name = TrackDuration.Bar + " bars " + TrackDuration.Beat + " beats (silence)";
        }
    }
}