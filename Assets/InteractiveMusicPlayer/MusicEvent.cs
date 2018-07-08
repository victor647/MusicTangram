using UnityEngine;

namespace InteractiveMusicPlayer
{
    public class MusicEvent : MonoBehaviour {

        public enum EventType
        {
            Play, Stop, StopOnGrid, Mute, Unmute, TransitionTo, ChangeParameterValue, SetSwitch, TriggerStinger
        }

        [Tooltip("The reference of this event in Music Manager")]
        public string eventName;        
        public EventType eventType;
        [Tooltip("If false, the target will be the music component on the same game object")]
        public bool manuallySetTarget;
        [EnumHide("eventType", 5, true)] public MusicComponent transitionFrom;		
        [ConditionalHide("manuallySetTarget", true)] public MusicComponent eventTarget;
        [EnumHide("eventType", 6, true)] public string parameterName;
        [EnumHide("eventType", 6, true)] public float parameterValue;        
        [EnumHide("eventType", 7, true)] public string switchName;
        
        public enum TriggerCondition
        {
            None, Start, OnDestroy, OnEnable, OnDisable, 
            OnTriggerEnter, OnTriggerExit, OnCollisionEnter, OnCollisionExit, 
            OnMouseDown, OnMouseUp, OnMouseEnter, OnMouseExit
        }

        public TriggerCondition triggerCondition = TriggerCondition.None;

        [Tooltip("For collision/trigger, if a tag is used. Leave blank for all tags")]
        public string colliderTag;
        [Tooltip("Fade in/out time for volume events")]
        public float fadeTime;
        [Tooltip("If the event should be delayed to trigger")]
        public float delayTime;
        public bool debugLog;
        
        //for the editor to manually trigger an event to test
        public void ManualTrigger()
        {
            Invoke("Activate", delayTime);
        }

        //for the editor to automatically generate event name based on type and target
        public void GenerateEventName()
        {
            if (!manuallySetTarget)
                eventName = eventType + "_" + gameObject.name;
            else if (eventTarget)
                eventName = eventType + "_" + eventTarget.gameObject.name;
            else 
                Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": Can't find event target, generate event name failed!");
        }
        
        private void Start ()
        {
            if (!manuallySetTarget)
                eventTarget = GetComponent<MusicComponent>();
            
            if (eventName == "" && eventTarget)
            {
                GenerateEventName();
            }
            
            if (triggerCondition == TriggerCondition.Start)
            {
                if (delayTime < 1f) //music rhythm will mess up if triggered too early
                    delayTime = 1f;                
                Invoke("Activate", delayTime);
            }                 
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggerCondition == TriggerCondition.OnTriggerEnter)
            {
                if (colliderTag != "")
                {
                    if (other.CompareTag(colliderTag)) Invoke("Activate", delayTime);
                }
                else
                {
                    Invoke("Activate", delayTime);
                }                
            }                 
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggerCondition == TriggerCondition.OnTriggerEnter)
            {
                if (colliderTag != "")
                {
                    if (other.CompareTag(colliderTag)) Invoke("Activate", delayTime);
                }
                else
                {
                    Invoke("Activate", delayTime);
                }                
            }  
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (triggerCondition == TriggerCondition.OnTriggerExit)
            {
                if (colliderTag != "")
                {
                    if (other.CompareTag(colliderTag)) Invoke("Activate", delayTime);
                }
                else
                {
                    Invoke("Activate", delayTime);
                }                
            }  
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (triggerCondition == TriggerCondition.OnTriggerExit)
            {
                if (colliderTag != "")
                {
                    if (other.CompareTag(colliderTag)) Invoke("Activate", delayTime);
                }
                else
                {
                    Invoke("Activate", delayTime);
                }                
            }  
        }

        private void OnCollisionEnter(Collision other)
        {
            if (triggerCondition == TriggerCondition.OnCollisionEnter)
            {
                if (colliderTag != "")
                {
                    if (other.collider.CompareTag(colliderTag)) Invoke("Activate", delayTime);
                }
                else
                {
                    Invoke("Activate", delayTime);
                }                
            }  
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (triggerCondition == TriggerCondition.OnCollisionEnter)
            {
                if (colliderTag != "")
                {
                    if (other.collider.CompareTag(colliderTag)) Invoke("Activate", delayTime);
                }
                else
                {
                    Invoke("Activate", delayTime);
                }                
            }
        }
        
        private void OnCollisionExit(Collision other)
        {
            if (triggerCondition == TriggerCondition.OnCollisionExit)
            {
                if (colliderTag != "")
                {
                    if (other.collider.CompareTag(colliderTag)) Invoke("Activate", delayTime);
                }
                else
                {
                    Invoke("Activate", delayTime);
                }                
            }
        }
        
        private void OnCollisionExit2D(Collision2D other)
        {
            if (triggerCondition == TriggerCondition.OnCollisionExit)
            {
                if (colliderTag != "")
                {
                    if (other.collider.CompareTag(colliderTag)) Invoke("Activate", delayTime);
                }
                else
                {
                    Invoke("Activate", delayTime);
                }                
            }
        }

        private void OnMouseDown()
        {
            if (triggerCondition == TriggerCondition.OnMouseDown) 
                Invoke("Activate", delayTime);
        }

        private void OnMouseUp()
        {
            if (triggerCondition == TriggerCondition.OnMouseUp) 
                Invoke("Activate", delayTime);
        }
        
        private void OnMouseEnter()
        {
            if (triggerCondition == TriggerCondition.OnMouseEnter) 
                Invoke("Activate", delayTime);
        }
        
        private void OnMouseExit()
        {
            if (triggerCondition == TriggerCondition.OnMouseExit) 
                Invoke("Activate", delayTime);
        }

        private void OnEnable()
        {
            MusicManager.Instance.AddEvent(this);
            if (triggerCondition == TriggerCondition.OnEnable) 
                Invoke("Activate", delayTime);
        }

        private void OnDisable()
        {            
            if (MusicManager.Instance)
                MusicManager.Instance.RemoveEvent(this);
            if (triggerCondition == TriggerCondition.OnDisable) 
                Activate();
        }
        
        private void OnDestroy()
        {
            if (triggerCondition == TriggerCondition.OnDestroy) 
                Activate();
        }

        //for external calls
        public void Activate()
        {
            if (!eventTarget && eventType != EventType.ChangeParameterValue)
            {
                Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": Event has no target!");
                return;
            }
            
            if (eventType != EventType.ChangeParameterValue) eventTarget.FadeTime = fadeTime;
            
            switch (eventType)
            {
                case EventType.Play:                    
                    eventTarget.Play();
                    break;
                case EventType.Stop:                    
                    eventTarget.Stop();
                    break;
                case EventType.StopOnGrid:                    
                    eventTarget.TransitionTo(null);
                    break;
                case EventType.Mute:                    
                    eventTarget.Mute();
                    break;
                case EventType.Unmute:                    
                    eventTarget.UnMute();
                    break;
                case EventType.TransitionTo:
                    transitionFrom.FadeTime = fadeTime;
                    transitionFrom.TransitionTo(eventTarget);
                    break;
                case EventType.ChangeParameterValue:
                    MusicManager.Instance.FindParameter(parameterName).ChangeTargetValue(parameterValue, true);               
                    break;
                case EventType.SetSwitch:                    
                    var swch = eventTarget as MusicSwitch;
                    if (swch)                    
                        swch.SetSwitch(switchName);                    
                    else
                        Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": Event target is not a music switch!");
                    break;
                case EventType.TriggerStinger:
                    
                    var stgr = eventTarget as MusicStinger;
                    if (!stgr)
                    {
                        Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": Event target is not a music stinger!");
                        return;
                    }                                        
                    stgr.TriggerStinger();                   
                    break;
            }
            if (debugLog) Debug.Log("Music event " + eventName + " is activated!");   
        }
    }
}