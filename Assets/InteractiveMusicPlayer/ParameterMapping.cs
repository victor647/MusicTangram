using UnityEngine;

namespace InteractiveMusicPlayer
{
    public class ParameterMapping : MonoBehaviour
    {
        [SerializeField]
        private string _parameterName = "";

        private enum TargetType
        {
            Volume, Pan, LowPassFilterCutoff, HighPassFilterCutoff
        }
        [Tooltip("Which property of target this parameter is mapped to")]
        [SerializeField]
        private TargetType _targetType = TargetType.Volume;

        [Tooltip("Large values result in more dramatic changes in the first half")]
        [Range(0.25f, 4f)]
        [SerializeField]
        private float _curveExponent = 1f;

        [SerializeField]
        private float _minTargetValue = 0f;
        [SerializeField]
        private float _maxTargetValue = 1f;
        [Tooltip("True if mapping range is different from the full range of parameter")]
        [SerializeField]
        private bool _customMappingRange = false;
        public bool CustomMappingRange
        {
            get { return _customMappingRange; }
        }
        
        [ConditionalHide("_customMappingRange", true)]        
        public float MinParameterValue;
        
        [ConditionalHide("_customMappingRange", true)]        
        public float MaxParameterValue;

        [SerializeField]
        private bool _inverseMinMaxMapping = false;        

        private MusicComponent _affectedMusic;
        private MusicParameter _parameter;
        
        public delegate void ModifyTarget(float value);
        public ModifyTarget Modify;

        private void Start()
        {                        
            _affectedMusic = GetComponent<MusicComponent>();
            switch (_targetType)
            {
                case TargetType.Volume:
                    Modify = _affectedMusic.ChangeVolume;
                    break;
                case TargetType.Pan:
                    Modify = _affectedMusic.ChangePan;
                    break;
                case TargetType.LowPassFilterCutoff:
                    Modify = _affectedMusic.SetLPFCutoff;
                    break;
                case TargetType.HighPassFilterCutoff:
                    Modify = _affectedMusic.SetHPFCutoff;
                    break;
            }

            _parameter = MusicManager.Instance.FindParameter(_parameterName);
            _parameter.RegisterMapping(this); //register this mapping to parameter                       
        }

        public void ApplyParameterToMusic(float value)
        {
            float parameterPercentage = (Mathf.Clamp(value, MinParameterValue, MaxParameterValue) - MinParameterValue) 
                / (MaxParameterValue - MinParameterValue);

            if (_inverseMinMaxMapping)
                parameterPercentage = 1f - parameterPercentage;            
            float targetValue = Mathf.Lerp(_minTargetValue, _maxTargetValue, Mathf.Pow(parameterPercentage, _curveExponent));            
            Modify(targetValue);                     
        }          
    }
}