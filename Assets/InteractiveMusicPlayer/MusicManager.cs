using System;
using UnityEngine;
using System.Collections.Generic;

namespace InteractiveMusicPlayer
{
    public class MusicManager : MonoBehaviour
    {
        private static MusicManager _instance;
        public static MusicManager Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = (MusicManager) FindObjectOfType(typeof(MusicManager));
                    if (!_instance)
                        Debug.LogError("Can't find music manager in scene!");                    
                }                                       
                return _instance;
            }            
        }

        
        [Tooltip("The number of music voices playing now")][ReadOnly]
        public int voicePlaying;
        [Tooltip("The maximum number of voices playing at the same time (Includes SFX). If more than limit, lowest voice will be send to virtual.")]
        public int voiceLimit = 32;
        [Tooltip("Buffer size for the audio system. Higher results in more latency but less CPU load. Must be power of 2")]
        public int bufferSize = 1024;
        
        [Tooltip("When event gets called, output to Unity console")]
        public bool eventLog;
        
        [Tooltip("If this music manager should not be destroyed when loading a different scene")]
        public bool stayBetweenScenes;
        
        [HideInInspector] public List<MusicEvent> musicEvents;
        [HideInInspector] public List<MusicTransitionSegment> transitionSegments;
        private Dictionary<string, MusicParameter> parameters = new Dictionary<string, MusicParameter>();
        private Dictionary<string, MusicComponent> musicInstances = new Dictionary<string, MusicComponent>();

               
        #region MACRO
        private void OnValidate()
        {
            gameObject.name = "Music Manager";
        }
        
        private void Awake ()
        {
            CheckInstance();            
            
            AudioConfiguration config = AudioSettings.GetConfiguration();
            config.dspBufferSize = bufferSize;
            config.numRealVoices = voiceLimit;
            AudioSettings.Reset(config);
            if (stayBetweenScenes)
                DontDestroyOnLoad(gameObject);
        }

        private void CheckInstance()
        {
            if (!_instance)
            {
                _instance = this;
            }
            else if (_instance != this)
            {
                Debug.LogWarning(GetObjectPath(_instance.gameObject) + ": An existing music manager is found!");
                Destroy(_instance.stayBetweenScenes ? gameObject : _instance.gameObject);
            }
        }

        //find hierarchy for nested music objects
        public string GetObjectPath(GameObject obj)
        {
            string path = "/" + obj.name;
            while (obj.transform.parent != null)
            {
                obj = obj.transform.parent.gameObject;
                path = "/" + obj.name + path;
            }
            return path;
        }
        
        //to stop all the music playing
        public static void StopAll(float fadeOutTime)
        {
            var music = FindObjectsOfType<MusicComponent>();
            foreach (var m in music)
            {                
                m.Stop(fadeOutTime);
            }
        }
        #endregion
        
        #region MUSICEVENT
        public void AddEvent(MusicEvent e)
        {
            musicEvents.Add(e);
        }
        
        public void RemoveEvent(MusicEvent e)
        {
            musicEvents.Remove(e);
        }

        //for external calls by string
        public static void PostEvent(string eventName)
        {            
            foreach (var e in Instance.musicEvents)
            {
                if (e.eventName == eventName)
                {
                    e.Activate();
                    if (Instance.eventLog) Debug.Log("Music event " + eventName + " is activated!");   
                    //if the event doesn't allow other events to have the same name, only activate that one
                    if (!e.allowDuplicate) 
                        return;
                }
            }
            Debug.LogError("Music Manager: can't find event with name of " + eventName);
        }
        #endregion
        
        #region MUSICINSTANCE
        public void RegisterMusicInstance(string name, MusicComponent instance)
        {
            if (!musicInstances.ContainsKey(name))            
                musicInstances.Add(name, instance);            
            else            
                Debug.LogError("A music instance of the name " + name + " has already been registered!");            
        }
        
        public void UnRegisterMusicInstance(string name)
        {
            if (!musicInstances.ContainsKey(name))          
                Debug.LogError("Can't find music instance with name " + name + "!");                             
            else            
                musicInstances.Remove(name);                           
        }

        //find the music instance registered by name
        public static MusicComponent FindMusicByName(string name)
        {
            if (Instance.musicInstances.ContainsKey(name))
            {
                return Instance.musicInstances[name];
            }
            Debug.LogError("Can't find music instance with name " + name + "!");
            return null;
        }
        #endregion

        #region MUSICPARAMETER
        //register the parameters to a list
        public void RegisterParameter(string parameterName, MusicParameter parameter)
        {
            if (!parameters.ContainsKey(parameterName))            
                parameters.Add(parameterName, parameter);
            else 
                Debug.LogError("Music Manager: An existing music parameter with same name of " + parameterName + " is found!");
        }

        //unregister the parameters to a list
        public void UnRegisterParameter(string parameterName)
        {
            if (parameters.ContainsKey(parameterName))            
                parameters.Remove(parameterName);
        }
        
        //find the parameter component by name
        public MusicParameter FindParameter(string parameterName)
        {
            if (parameters.ContainsKey(parameterName))
                return parameters[parameterName];
            //if cannot find the parameter
            Debug.LogError("Music Manager: can't find parameter with name of " + parameterName);
            return null;            
        }
        
        //directly set parameter value from script
        public static void SetParameterValue(string parameterName, float value)
        {
            if (Instance.parameters.ContainsKey(parameterName))
                Instance.parameters[parameterName].ChangeTargetValue(value, true);
            else
                Debug.LogError("Music Manager: can't find parameter with name of " + parameterName);
        }
        #endregion
        
        #region MUSICSWITCH
        //directly set switch from script
        public static void SetSwitch(string eventName, string switchName)
        {
            foreach (var e in Instance.musicEvents)
            {
                if (e.eventName == eventName && e.eventAction == MusicEvent.EventType.SetSwitch)
                {
                    e.switchName = switchName;
                    e.Activate();
                    return;
                }
            }
            Debug.LogError("Music Manager: can't find switch with name of " + switchName);
        }
        #endregion

    }
}
