using System.Collections.Generic;
using UnityEngine;

namespace InteractiveMusicPlayer
{
	public class MusicSwitch : MusicComponent {

		[System.Serializable]
		private struct SwitchAssignment
		{
			public string SwitchName;
			public MusicComponent Music;

			public SwitchAssignment(MusicComponent music) //generate switch based on game object name
			{
				Music = music;
				SwitchName = music.gameObject.name;
			}
		}
		private MusicComponent _currentMusic;
		public MusicComponent CurrentMusic
		{
			get { return _currentMusic; }			
		}

		[Header("Switch Settings")] 
		[Tooltip("If switch is changed, transition to the same playback position of target track")]
		[SerializeField]
		private bool _switchToSamePosition = false;		
		[SerializeField]
		private List<SwitchAssignment> _switchList = new List<SwitchAssignment>();
		[SerializeField]
		private string _currentSwitch;		

		private MusicClip[] _subTracks;
		
		//for the editor to populate switch with child tracks
		public void FillWithChildren()
		{
			_switchList.Clear();
			foreach (Transform child in transform)
			{                
				_switchList.Add(new SwitchAssignment(child.GetComponent<MusicComponent>()));
			}			
		}

		protected override void Start()
		{			
			if (_switchList.Count < 2)
			{
				Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": Switch music must have at least 2 destinations!");
				return;				
			}
			gameObject.name += " (switch)";			

			base.Start();
			if (_switchToSamePosition) //make sure all music are same length
			{
				if (TrackDuration != BarAndBeat.Zero) //copy positions to all children
					CopySettingsToChildren();
				else
				{
					CopyPositionSettings(_switchList[0].Music);
					LoopCount = _switchList[0].Music.LoopCount;
				}

				_subTracks = GetComponentsInChildren<MusicClip>();
				foreach (var child in _childComponents)
				{
					PlayCallback += child.Play;
				}

				foreach (var track in _subTracks)
				{
					track.Mute();
				}
			}			
			SetSwitch(_currentSwitch);
		}	
		
		public void SetSwitch(string switchName)
		{			
			_currentSwitch = switchName;	
			if (_debugLog) Debug.Log(Time.time + ": " + Name + " will switch to " + switchName);
			if (_switchToSamePosition)
			{
				if (_transitionInterval == TransitionType.Immediate)				
					SetMutes();									
				else
				{
					//find the remaining time to next transition point
					float delayTime = GridLength - (Time.time - _transitionCheckTimeStamp);
					if (delayTime > 0f)
						Invoke("SetMutes", delayTime);
				}					
			}
			else
			{
				if (_currentSwitch == "") return;
				foreach (var track in _switchList)
				{
					if (track.SwitchName == _currentSwitch)
					{						
						if (PlayingStatus != PlayStatus.Idle) //if a track is already playing, trigger the transition
						{
							_currentMusic.TransitionTo(track.Music);
						}
						if (_currentMusic != null) _currentMusic.LoopCallback -= ResetPlayhead;
						_currentMusic = track.Music; //update the switched music after transition event
						_currentMusic.LoopCallback += ResetPlayhead;
						CopyPositionSettings(_currentMusic);
						PlayCallback = _currentMusic.Play; //set the track to play as the switched track
						StopCallback = _currentMusic.Stop;						
						return;
					}
				}				
				Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": Can't find music by switch name " + _currentSwitch);	
			}					
		}

		//overload for manually set fade time
		public void SetSwitch(string switchName, float crossFadeTime)
		{
			FadeTime = crossFadeTime;
			SetSwitch(switchName);
		}
		
		//delayed setting of mutes
		private void SetMutes()
		{			
			MusicClip[] activeChildrenTracks; //find all music tracks playing
			if (_currentMusic != null)
			{
				activeChildrenTracks = _currentMusic.GetComponentsInChildren<MusicClip>();
				foreach (var track in activeChildrenTracks) //mute inactive music
				{
					track.Mute();					
				}
			}	
			
			if (_currentSwitch == "") return; //empty switch mutes all music
			
			foreach (var item in _switchList)
			{				
				if (item.SwitchName == _currentSwitch)
				{
											
					_currentMusic = item.Music; 
					activeChildrenTracks = _currentMusic.GetComponentsInChildren<MusicClip>();
					foreach (var track in activeChildrenTracks) //unmute selected music
					{
						track.UnMute();							
					}								 
					return;
				}
			}
			//if no switch is found
			Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": Can't find music by switch name " + _currentSwitch);
		}
	}	
}
