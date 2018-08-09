using UnityEngine;

namespace InteractiveMusicPlayer
{
    public class MusicEvent : MonoBehaviour {
        
        [Tooltip("The reference of this event in Music Manager")]
        public string eventName;
        [Tooltip("If multiple events of same name is allowed and called together")]
        public bool allowDuplicate;
        
        public enum EventType
        {
            Play, Stop, StopOnGrid, Mute, Unmute, TransitionTo, 
            ChangeParameterValue, SetSwitch, TriggerStinger
        }        
        [Tooltip("What action this event does")]
        public EventType eventAction;
        
        [Tooltip("If false, the target will be the music component on the same game object")]
        public bool manuallySetTarget;
        [EnumHide("eventAction", 5, true)] 
        public MusicComponent transitionFrom;		
        [ConditionalHide("manuallySetTarget", true)] 
        public MusicComponent eventTarget;
        
        [Tooltip("The music parameter this event is linked to")]
        [EnumHide("eventAction", 6, true)] 
        public string parameterName;
        [Tooltip("The music parameter this event sets value")]
        [EnumHide("eventAction", 6, true)]  
        public float parameterValue;        
        [Tooltip("The switch this event will set to")]
        [EnumHide("eventAction", 7, true)] 
        public string switchName;
        
        public enum TriggerCondition
        {
            None, Start, OnDestroy, OnEnable, OnDisable, 
            OnTriggerEnter, OnTriggerExit, OnCollisionEnter, OnCollisionExit, 
            OnMouseDown, OnMouseUp, OnMouseEnter, OnMouseExit
        }
        [Tooltip("When this music event will be triggered")]
        public TriggerCondition triggerCondition = TriggerCondition.None;

        [Tooltip("For collision/trigger, if a tag is used. Leave blank for all tags")]
        public string colliderTag;
        [Tooltip("Fade in/out time for volume events")]
        public float fadeTime;
        [Tooltip("If the event should be delayed to trigger. Not applicable to OnDestroy")]
        public float delayTime;        

        #region SETUP               
        //for the editor to manually trigger an event to test
        public void ManualTrigger()
        {
            Invoke("Activate", delayTime);
        }

        //for the editor to automatically generate event name based on type and target
        public void GenerateEventName()
        {
            if (!manuallySetTarget)
                eventName = eventAction + "_" + gameObject.name;
            else if (eventTarget)
                eventName = eventAction + "_" + eventTarget.gameObject.name;
            else 
                Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": Can't find event target, generate event name failed!");
        }
        #endregion
        
        #region TRIGGER
        private void Start ()
        {                   
            if (triggerCondition == TriggerCondition.Start)
            {
                if (delayTime < 0.1f) //music might mess up if triggered too early
                    delayTime = 0.1f;                
                Invoke("Activate", delayTime);
            }                 
        }

        private void OnTriggerEnter(Collider other)
        {
            if (triggerCondition == TriggerCondition.OnTriggerEnter)
            {
                if (colliderTag != "") //if user sets specific collider tag
                {
                    if (other.CompareTag(colliderTag)) 
                        Invoke("Activate", delayTime);
                }
                else                
                    Invoke("Activate", delayTime);                                
            }                 
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (triggerCondition == TriggerCondition.OnTriggerEnter)
            {
                if (colliderTag != "")
                {
                    if (other.CompareTag(colliderTag)) 
                        Invoke("Activate", delayTime);
                }
                else                
                    Invoke("Activate", delayTime);                                
            }  
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (triggerCondition == TriggerCondition.OnTriggerExit)
            {
                if (colliderTag != "")
                {
                    if (other.CompareTag(colliderTag)) 
                        Invoke("Activate", delayTime);
                }
                else                
                    Invoke("Activate", delayTime);                                
            }  
        }
        
        private void OnTriggerExit2D(Collider2D other)
        {
            if (triggerCondition == TriggerCondition.OnTriggerExit)
            {
                if (colliderTag != "")
                {
                    if (other.CompareTag(colliderTag)) 
                        Invoke("Activate", delayTime);
                }
                else                
                    Invoke("Activate", delayTime);                                
            }  
        }

        private void OnCollisionEnter(Collision other)
        {
            if (triggerCondition == TriggerCondition.OnCollisionEnter)
            {
                if (colliderTag != "")
                {
                    if (other.collider.CompareTag(colliderTag)) 
                        Invoke("Activate", delayTime);
                }
                else                
                    Invoke("Activate", delayTime);                                
            }  
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (triggerCondition == TriggerCondition.OnCollisionEnter)
            {
                if (colliderTag != "")
                {
                    if (other.collider.CompareTag(colliderTag)) 
                        Invoke("Activate", delayTime);
                }
                else                
                    Invoke("Activate", delayTime);                               
            }
        }
        
        private void OnCollisionExit(Collision other)
        {
            if (triggerCondition == TriggerCondition.OnCollisionExit)
            {
                if (colliderTag != "")
                {
                    if (other.collider.CompareTag(colliderTag)) 
                        Invoke("Activate", delayTime);
                }
                else                
                    Invoke("Activate", delayTime);                                
            }
        }
        
        private void OnCollisionExit2D(Collision2D other)
        {
            if (triggerCondition == TriggerCondition.OnCollisionExit)
            {
                if (colliderTag != "")
                {
                    if (other.collider.CompareTag(colliderTag)) 
                        Invoke("Activate", delayTime);
                }
                else                
                    Invoke("Activate", delayTime);                                
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
            if (MusicManager.Instance)
                MusicManager.Instance.AddEvent(this);
            if (!manuallySetTarget)
                eventTarget = GetComponent<MusicComponent>();                            
            if (eventName == "" && eventTarget) //if the user forgets to write event name          
                GenerateEventName();                            
            if (triggerCondition == TriggerCondition.OnEnable) 
                Invoke("Activate", delayTime);
        }

        private void OnDisable()
        {                        
            if (triggerCondition == TriggerCondition.OnDisable) 
                Invoke("Activate", delayTime);
            if (MusicManager.Instance)
                MusicManager.Instance.RemoveEvent(this);
        }
        
        private void OnDestroy()
        {
            if (triggerCondition == TriggerCondition.OnDestroy) 
                Activate();
        }
        #endregion
        
        //activate the event
        public void Activate()
        {
            if (!eventTarget && eventAction != EventType.ChangeParameterValue)
            {
                Debug.LogError(MusicManager.Instance.GetObjectPath(gameObject) + ": Music event has no target!");
                return;
            }
            //set fade time for volume related events
            if (eventAction != EventType.ChangeParameterValue) eventTarget.FadeTime = fadeTime;
            
            switch (eventAction)
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
        }
    }
}