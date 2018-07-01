using UnityEngine;

namespace InteractiveMusicPlayer
{
    public class Metronome : MonoBehaviour
    {

        public AudioClip clickDownBeat;
        public AudioClip clickNormal;
        [Range(0f, 1f)] public float volume = 1f;
        private AudioSource source;
        private MusicComponent hostMusic;
        private bool isPlaying;

        private int beatCounter;


        private void Start ()
        {
            hostMusic = GetComponent<MusicComponent>();
            if (!hostMusic){
                Debug.LogError("Can't find a coexisting music track!");			
            }

            source = gameObject.AddComponent<AudioSource>();
        }

        private void FixedUpdate () {
            if (hostMusic.PlayingStatus == MusicComponent.PlayStatus.Idle && isPlaying)
            {
                isPlaying = false;
                CancelInvoke("BarTimer");
                CancelInvoke("BeatTimer");
                beatCounter = 0;
            }
        
            if (hostMusic.PlayingStatus != MusicComponent.PlayStatus.Idle && !isPlaying)
            {                
                isPlaying = true;
                InvokeRepeating("BarTimer", 0, hostMusic.BarLength);
                InvokeRepeating("BeatTimer", 0, hostMusic.BeatLength);
            }            
        }

        //play every down beat
        private void BarTimer()
        {
            source.PlayOneShot(clickDownBeat, volume);
        }

        private void BeatTimer()
        {
        
            if (beatCounter != 0) //make sure it doesn't play on down beat
            {
                source.PlayOneShot(clickNormal, 0.8f * volume);            
            }                
            beatCounter++;
            
            if (beatCounter == hostMusic.BeatsPerBar) //reset the counter every bar
            {
                beatCounter = 0;
            }                          
        }
    }
}